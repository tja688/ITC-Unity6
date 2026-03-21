#!/usr/bin/env python3
"""
psd_analyze.py — PSD 图层树结构分析工具

读取 PSD 文件，输出完整的图层树 JSON（含 bbox、textInfo、混合模式等元信息）。
支持按图层名过滤、仅可见图层、最大深度限制。

用法：
  py -3 -X utf8 psd_analyze.py "path/to/file.psd" -o layers.json
  py -3 -X utf8 psd_analyze.py "path/to/file.psd" --name "对话框"
  py -3 -X utf8 psd_analyze.py "path/to/file.psd" --name "头像" --match contains

依赖：pip install psd-tools
"""

import argparse
import json
import re
import sys
from pathlib import Path

try:
    from psd_tools import PSDImage
except ImportError:
    print("错误：缺少 psd-tools。请运行：py -3 -m pip install psd-tools", file=sys.stderr)
    sys.exit(1)


# ── name sanitization ──────────────────────────────────────────

_UNSAFE_RE = re.compile(r'[^\w\-]', re.UNICODE)


def safe_name(original: str, layer_id: int | None = None) -> str:
    """Create a filesystem/variable-safe name from a PSD layer name."""
    s = original.strip()
    s = s.replace(' ', '-').replace('_', '-')
    s = _UNSAFE_RE.sub('', s)
    s = re.sub(r'-{2,}', '-', s).strip('-')
    if not s:
        s = f'layer-{layer_id}' if layer_id is not None else 'unnamed'
    return s.lower()


# ── text info extraction ───────────────────────────────────────

def extract_text_info(layer) -> dict | None:
    """Extract text content and style information from a type layer."""
    if layer.kind != 'type':
        return None

    try:
        engine_data = layer.engine_dict
    except Exception:
        engine_data = None

    # Basic text content
    text = ''
    try:
        text = layer.text
    except Exception:
        pass

    if not text:
        return None

    info = {'text': text, 'runs': []}

    # Try to extract styled runs
    try:
        if engine_data:
            style_sheet = engine_data.get('StyleRun', {})
            run_array = style_sheet.get('RunArray', [])
            run_lengths = style_sheet.get('RunLengthArray', [])

            pos = 0
            for i, run_data in enumerate(run_array):
                length = run_lengths[i] if i < len(run_lengths) else len(text) - pos
                run_text = text[pos:pos + length]
                pos += length

                style = run_data.get('StyleSheet', {}).get('StyleSheetData', {})
                run_info = {'text': run_text}

                if 'FontSize' in style:
                    run_info['fontSize'] = round(style['FontSize'], 2)
                if 'FillColor' in style:
                    fc = style['FillColor'].get('Values', [])
                    if len(fc) >= 4:
                        run_info['color'] = [round(fc[1] * 255), round(fc[2] * 255), round(fc[3] * 255)]
                if 'AutoLeading' in style:
                    run_info['autoLeading'] = style['AutoLeading']
                if 'Leading' in style:
                    run_info['leading'] = round(style['Leading'], 2)

                # Font name from FontSet
                font_idx = style.get('Font', 0)
                try:
                    resource = engine_data.get('ResourceDict', {})
                    font_set = resource.get('FontSet', [])
                    if font_idx < len(font_set):
                        run_info['fontName'] = font_set[font_idx].get('Name', '')
                except Exception:
                    pass

                info['runs'].append(run_info)

            # Summary from first run
            if info['runs']:
                first = info['runs'][0]
                for key in ('fontSize', 'fontName', 'color', 'leading'):
                    if key in first:
                        info[key] = first[key]
    except Exception as e:
        if verbose_mode:
            print(f"  [WARN] 文本样式提取失败: {layer.name}: {e}", file=sys.stderr)

    return info


# ── layer tree walking ─────────────────────────────────────────

verbose_mode = False


def walk_layer(layer, depth: int = 0, max_depth: int | None = None, visible_only: bool = False) -> dict | None:
    """Recursively convert a layer into a JSON-serializable dict."""
    if visible_only and not layer.visible:
        return None

    if max_depth is not None and depth > max_depth:
        return None

    layer_id = getattr(layer, 'layer_id', None)
    blend_mode = 'normal'
    try:
        blend_mode = str(layer.blend_mode).split('.')[-1].lower()
    except Exception:
        pass

    node = {
        'name': safe_name(layer.name, layer_id),
        'originalName': layer.name,
        'layerId': layer_id,
        'kind': layer.kind,
        'visible': layer.visible,
        'opacity': layer.opacity,
        'blendMode': blend_mode,
        'bbox': [layer.left, layer.top, layer.right, layer.bottom],
        'size': [layer.width, layer.height],
        'textInfo': extract_text_info(layer),
        'children': [],
    }

    if hasattr(layer, '__iter__'):
        for child in layer:
            child_node = walk_layer(child, depth + 1, max_depth, visible_only)
            if child_node is not None:
                node['children'].append(child_node)

    return node


def build_tree(psd, max_depth=None, visible_only=False) -> list:
    """Build the full layer tree from a PSD image."""
    tree = []
    for layer in psd:
        node = walk_layer(layer, depth=0, max_depth=max_depth, visible_only=visible_only)
        if node is not None:
            tree.append(node)
    return tree


# ── name-based filtering ──────────────────────────────────────

def filter_tree(tree: list, name: str, match: str = 'exact') -> list:
    """Filter tree to only include branches that contain matching layers.

    Returns a pruned tree where matched layers are kept along with their
    ancestor path. Non-matching leaves are removed.
    """
    results = []

    for node in tree:
        is_match = False
        if match == 'exact':
            is_match = (node['originalName'] == name or node['name'] == name)
        elif match == 'contains':
            is_match = (name.lower() in node['originalName'].lower() or name.lower() in node['name'].lower())

        filtered_children = filter_tree(node.get('children', []), name, match)

        if is_match or filtered_children:
            pruned = dict(node)
            pruned['children'] = filtered_children if not is_match else node.get('children', [])
            results.append(pruned)

    return results


# ── dedup safe names ──────────────────────────────────────────

def dedup_names(tree: list, seen: dict | None = None):
    """Ensure all safe names are unique by appending layerId when needed."""
    if seen is None:
        seen = {}

    for node in tree:
        name = node['name']
        if name in seen:
            # Append layerId to make unique
            if node['layerId'] is not None:
                node['name'] = f"{name}-{node['layerId']}"
            else:
                count = seen.get(name, 0)
                node['name'] = f"{name}-{count + 1}"
        seen[name] = seen.get(name, 0) + 1

        dedup_names(node.get('children', []), seen)


# ── pretty terminal output ────────────────────────────────────

def print_tree(tree: list, indent: int = 0):
    """Print a human-readable layer tree to stderr."""
    for node in tree:
        prefix = '  ' * indent
        vis = 'V' if node['visible'] else 'H'
        kind = node['kind']
        name = node['originalName']
        bbox = node['bbox']
        size = node['size']
        text_hint = ''
        if node.get('textInfo'):
            t = node['textInfo']['text']
            text_hint = f'  text="{t[:30]}{"…" if len(t) > 30 else ""}"'

        print(f"{prefix}[{vis}] {kind:12s} | {name} | bbox={bbox} | {size[0]}×{size[1]}{text_hint}",
              file=sys.stderr)

        if node.get('children'):
            print_tree(node['children'], indent + 1)


# ── main ──────────────────────────────────────────────────────

def main():
    global verbose_mode

    parser = argparse.ArgumentParser(
        description='PSD 图层树分析工具 — 输出结构化 JSON',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog='示例：\n'
               '  py -3 -X utf8 psd_analyze.py "design.psd" -o layers.json\n'
               '  py -3 -X utf8 psd_analyze.py "design.psd" --name "对话框"\n'
               '  py -3 -X utf8 psd_analyze.py "design.psd" --name "btn" --match contains\n'
    )
    parser.add_argument('psd_path', metavar='PSD_PATH', help='PSD 文件路径')
    parser.add_argument('-o', '--output', default=None, help='JSON 输出路径（默认打印到 stdout）')
    parser.add_argument('--name', default=None, help='按图层名过滤')
    parser.add_argument('--match', choices=['exact', 'contains'], default='exact', help='匹配模式（默认 exact）')
    parser.add_argument('--visible-only', action='store_true', help='仅包含可见图层')
    parser.add_argument('--max-depth', type=int, default=None, help='最大递归深度')
    parser.add_argument('--verbose', action='store_true', help='输出详细日志到 stderr')

    args = parser.parse_args()
    verbose_mode = args.verbose

    psd_path = Path(args.psd_path)
    if not psd_path.exists():
        print(f"错误：文件不存在 — {psd_path}", file=sys.stderr)
        sys.exit(1)

    if verbose_mode:
        print(f"正在读取: {psd_path}", file=sys.stderr)

    psd = PSDImage.open(str(psd_path))
    tree = build_tree(psd, max_depth=args.max_depth, visible_only=args.visible_only)
    dedup_names(tree)

    if args.name:
        tree = filter_tree(tree, args.name, args.match)
        if not tree:
            print(f"未找到匹配图层: {args.name} (mode={args.match})", file=sys.stderr)
            sys.exit(0)

    result = {
        'psd_size': [psd.width, psd.height],
        'color_mode': str(psd.color_mode).split('.')[-1] if hasattr(psd, 'color_mode') else 'unknown',
        'layer_count': sum(1 for _ in psd.descendants()),
        'tree': tree,
    }

    json_str = json.dumps(result, ensure_ascii=False, indent=2)

    if args.output:
        out_path = Path(args.output)
        out_path.parent.mkdir(parents=True, exist_ok=True)
        out_path.write_text(json_str, encoding='utf-8')
        print(f"已保存: {out_path} ({len(tree)} 个顶层节点)", file=sys.stderr)
    else:
        print(json_str)

    if verbose_mode or not args.output:
        print(f"\n--- PSD: {psd.width}×{psd.height}, {result['layer_count']} 图层 ---", file=sys.stderr)
        print_tree(tree)


if __name__ == '__main__':
    main()
