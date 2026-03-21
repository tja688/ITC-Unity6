#!/usr/bin/env python3
"""
psd_export.py — PSD 图层导出工具

将 PSD 中的指定图层导出为独立图像文件（PNG/WebP/JPEG），
并生成 manifest.json 记录每个图层的精确 bbox 和元信息，
方便 Web/Canvas 渲染时精确定位。

用法：
  py -3 -X utf8 psd_export.py "file.psd" -o ./output/
  py -3 -X utf8 psd_export.py "file.psd" -o ./output/ --layers "对话框" "背景"
  py -3 -X utf8 psd_export.py "file.psd" -o ./output/ --layers "头像" --match contains
  py -3 -X utf8 psd_export.py "file.psd" -o ./output/ --composite --format webp

依赖：pip install psd-tools Pillow
"""

import argparse
import json
import re
import sys
from datetime import datetime, timezone
from pathlib import Path

try:
    from psd_tools import PSDImage
except ImportError:
    print("错误：缺少 psd-tools。请运行：py -3 -m pip install psd-tools", file=sys.stderr)
    sys.exit(1)

try:
    from PIL import Image
except ImportError:
    print("错误：缺少 Pillow。请运行：py -3 -m pip install Pillow", file=sys.stderr)
    sys.exit(1)


# ── name sanitization ──────────────────────────────────────────

_UNSAFE_RE = re.compile(r'[^\w\-]', re.UNICODE)


def safe_name(original: str, layer_id: int | None = None) -> str:
    """Create a filesystem-safe name from a PSD layer name."""
    s = original.strip()
    s = s.replace(' ', '-').replace('_', '-')
    s = _UNSAFE_RE.sub('', s)
    s = re.sub(r'-{2,}', '-', s).strip('-')
    if not s:
        s = f'layer-{layer_id}' if layer_id is not None else 'unnamed'
    return s.lower()


# ── layer matching ─────────────────────────────────────────────

def layer_matches(layer, names: list[str], match: str) -> bool:
    """Check if a layer matches any of the given names."""
    if not names:
        return True  # no filter = match all

    for name in names:
        if match == 'exact':
            if layer.name == name:
                return True
        elif match == 'contains':
            if name.lower() in layer.name.lower():
                return True
    return False


def collect_matching_layers(psd_or_group, names: list[str], match: str,
                            visible_only: bool, expand_groups: bool,
                            depth: int = 0) -> list:
    """Recursively find layers that match the filter criteria.

    When names are specified, we look for exact/contains matches at any depth.
    When no names are specified, we export top-level layers.
    """
    results = []

    for layer in psd_or_group:
        if visible_only and not layer.visible:
            continue

        if names:
            # Search recursively for matching layers
            if layer_matches(layer, names, match):
                if expand_groups and layer.kind == 'group':
                    # Expand: export children individually
                    for child in layer:
                        if visible_only and not child.visible:
                            continue
                        results.append(child)
                else:
                    results.append(layer)
            else:
                # Recurse into groups to find matches deeper
                if hasattr(layer, '__iter__'):
                    results.extend(collect_matching_layers(
                        layer, names, match, visible_only, expand_groups, depth + 1
                    ))
        else:
            # No filter: export all top-level layers
            if expand_groups and layer.kind == 'group':
                for child in layer:
                    if visible_only and not child.visible:
                        continue
                    results.append(child)
            else:
                results.append(layer)

    return results


# ── export helpers ─────────────────────────────────────────────

def determine_filename(layer, used_names: dict, prefix: str, fmt: str) -> str:
    """Generate a unique filename for a layer."""
    layer_id = getattr(layer, 'layer_id', None)
    base = safe_name(layer.name, layer_id)
    if prefix:
        base = f"{prefix}{base}"

    ext = fmt if fmt != 'jpg' else 'jpg'

    if base in used_names:
        # Disambiguate with layer ID or counter
        if layer_id is not None:
            base = f"{base}-{layer_id}"
        else:
            used_names[base] = used_names.get(base, 0) + 1
            base = f"{base}-{used_names[base]}"

    used_names[base] = used_names.get(base, 0) + 1
    return f"{base}.{ext}"


def export_layer_image(layer, filepath: Path, fmt: str, quality: int) -> bool:
    """Composite and save a layer to an image file."""
    try:
        img = layer.composite()
    except Exception as e:
        print(f"  [SKIP] {layer.name} — composite 失败: {e}", file=sys.stderr)
        return False

    if img is None:
        print(f"  [SKIP] {layer.name} — 无像素内容", file=sys.stderr)
        return False

    try:
        if fmt == 'png':
            img.save(str(filepath), 'PNG', optimize=True)
        elif fmt == 'webp':
            img.save(str(filepath), 'WEBP', quality=quality, method=4)
        elif fmt in ('jpg', 'jpeg'):
            # JPEG doesn't support alpha, convert to RGB
            if img.mode in ('RGBA', 'LA', 'PA'):
                bg = Image.new('RGB', img.size, (0, 0, 0))
                bg.paste(img, mask=img.split()[-1])
                img = bg
            elif img.mode != 'RGB':
                img = img.convert('RGB')
            img.save(str(filepath), 'JPEG', quality=quality, optimize=True)
        else:
            img.save(str(filepath))

        return True
    except Exception as e:
        print(f"  [ERR] {layer.name} — 保存失败: {e}", file=sys.stderr)
        return False


# ── main ──────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(
        description='PSD 图层导出工具 — 导出 PNG/WebP + manifest.json',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog='示例：\n'
               '  py -3 -X utf8 psd_export.py "design.psd" -o ./assets/\n'
               '  py -3 -X utf8 psd_export.py "design.psd" -o ./assets/ --layers "对话框" "背景"\n'
               '  py -3 -X utf8 psd_export.py "design.psd" -o ./assets/ --composite --format webp\n'
    )
    parser.add_argument('psd_path', metavar='PSD_PATH', help='PSD 文件路径')
    parser.add_argument('-o', '--output-dir', required=True, help='输出目录')
    parser.add_argument('--layers', nargs='+', default=None, help='要导出的图层名列表')
    parser.add_argument('--match', choices=['exact', 'contains'], default='exact', help='匹配模式')
    parser.add_argument('--visible-only', action='store_true', help='仅导出可见图层')
    parser.add_argument('--composite', action='store_true', help='同时导出完整合成参考图')
    parser.add_argument('--flatten-groups', action='store_true', default=True,
                        help='组图层导出为合并后的单张图片（默认）')
    parser.add_argument('--expand-groups', action='store_true', default=False,
                        help='组图层展开为子图层分别导出')
    parser.add_argument('--format', choices=['png', 'webp', 'jpg'], default='png', help='输出格式')
    parser.add_argument('--quality', type=int, default=95, help='JPEG/WebP 质量 (1-100)')
    parser.add_argument('--no-manifest', action='store_true', help='不生成 manifest.json')
    parser.add_argument('--prefix', default='', help='文件名前缀')
    parser.add_argument('--verbose', action='store_true', help='详细日志')

    args = parser.parse_args()

    psd_path = Path(args.psd_path)
    if not psd_path.exists():
        print(f"错误：文件不存在 — {psd_path}", file=sys.stderr)
        sys.exit(1)

    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)

    print(f"读取 PSD: {psd_path}", file=sys.stderr)
    psd = PSDImage.open(str(psd_path))
    print(f"画布大小: {psd.width}×{psd.height}", file=sys.stderr)

    # Collect layers to export
    expand = args.expand_groups
    layers_to_export = collect_matching_layers(
        psd, args.layers or [], args.match, args.visible_only, expand
    )

    if not layers_to_export:
        print("未找到匹配的图层。", file=sys.stderr)
        sys.exit(0)

    print(f"将导出 {len(layers_to_export)} 个图层…\n", file=sys.stderr)

    # Export each layer
    manifest_layers = []
    used_names = {}

    for layer in layers_to_export:
        filename = determine_filename(layer, used_names, args.prefix, args.format)
        filepath = output_dir / filename

        if args.verbose:
            print(f"  导出: {layer.name} → {filename}", file=sys.stderr)

        success = export_layer_image(layer, filepath, args.format, args.quality)
        if not success:
            continue

        layer_id = getattr(layer, 'layer_id', None)
        entry = {
            'id': safe_name(layer.name, layer_id),
            'originalName': layer.name,
            'file': filename,
            'kind': layer.kind,
            'bbox': [layer.left, layer.top, layer.right, layer.bottom],
            'size': [layer.width, layer.height],
        }
        manifest_layers.append(entry)

        w, h = layer.width, layer.height
        print(f"  [OK] {layer.name} → {filename} ({w}×{h})", file=sys.stderr)

    # Export full composite if requested
    if args.composite:
        print(f"\n导出完整合成图…", file=sys.stderr)
        composite_name = f"{args.prefix}full-composite.{args.format}"
        composite_path = output_dir / composite_name
        try:
            full = psd.composite()
            if args.format == 'png':
                full.save(str(composite_path), 'PNG', optimize=True)
            elif args.format == 'webp':
                full.save(str(composite_path), 'WEBP', quality=args.quality)
            elif args.format in ('jpg', 'jpeg'):
                rgb = full.convert('RGB') if full.mode != 'RGB' else full
                rgb.save(str(composite_path), 'JPEG', quality=args.quality)
            else:
                full.save(str(composite_path))
            print(f"  [OK] full-composite → {composite_name} ({full.size[0]}×{full.size[1]})", file=sys.stderr)
        except Exception as e:
            print(f"  [ERR] 合成失败: {e}", file=sys.stderr)

    # Write manifest
    if not args.no_manifest and manifest_layers:
        manifest = {
            'psd_file': psd_path.name,
            'psd_size': [psd.width, psd.height],
            'exported_at': datetime.now(timezone.utc).isoformat(),
            'format': args.format,
            'layers': manifest_layers,
        }
        manifest_path = output_dir / 'manifest.json'
        manifest_path.write_text(
            json.dumps(manifest, ensure_ascii=False, indent=2),
            encoding='utf-8'
        )
        print(f"\nManifest → {manifest_path}", file=sys.stderr)

    print(f"\n完成！共导出 {len(manifest_layers)} 个图层到 {output_dir}", file=sys.stderr)


if __name__ == '__main__':
    main()
