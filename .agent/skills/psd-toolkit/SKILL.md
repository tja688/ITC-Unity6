---
name: psd-toolkit
description: |
  通用 PSD 处理工具集：分析图层树结构（→ JSON）、导出指定图层为 PNG/WebP（带精确 bbox manifest）。
  适用场景：将设计稿还原为 HTML/CSS/Canvas、提取 UI 素材、调试图层排布、生成游戏/应用所需的切图。
  支持：嵌套组、文本图层样式提取、图层名过滤、组合并导出、全画布合成参考图。
---

# PSD Toolkit

将 PSD 图层树**分析**为结构化 JSON，并将指定图层**导出**为独立图像 + 定位 manifest。

两个核心脚本：

| 脚本 | 用途 |
|------|------|
| `scripts/psd_analyze.py` | 读取 PSD → 输出图层树 JSON（结构、bbox、文本样式） |
| `scripts/psd_export.py` | 读取 PSD → 导出指定图层为 PNG + manifest.json |

---

## 前置依赖（每个环境仅需安装一次）

```bash
py -3 -m pip install psd-tools Pillow
```

> [!NOTE]
> Windows 下运行所有脚本时务必带 `-X utf8`，否则中文图层名可能报编码错误。

---

## 脚本 1：psd_analyze.py — 图层树分析

### 基本用法

```bash
# 导出完整图层树
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_analyze.py "path/to/file.psd"

# 保存为 JSON 文件
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_analyze.py "path/to/file.psd" -o layers.json

# 按名称过滤（精确匹配）
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_analyze.py "path/to/file.psd" --name "对话框"

# 按名称过滤（包含匹配）
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_analyze.py "path/to/file.psd" --name "头像" --match contains

# 仅显示可见图层
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_analyze.py "path/to/file.psd" --visible-only
```

### 命令行参数

| 参数 | 说明 |
|------|------|
| `PSD_PATH` | PSD 文件路径（必填） |
| `-o, --output` | JSON 输出路径，不指定则打印到 stdout |
| `--name` | 按图层名过滤，返回匹配的图层及其祖先路径 |
| `--match` | 匹配模式：`exact`（默认）或 `contains` |
| `--visible-only` | 仅包含可见图层 |
| `--max-depth` | 最大递归深度 |
| `--verbose` | 打印详细处理日志到 stderr |

### 输出 JSON 格式

```json
{
  "psd_size": [1980, 1080],
  "color_mode": "RGB",
  "layer_count": 25,
  "tree": [
    {
      "name": "safe-name",
      "originalName": "原始图层名",
      "layerId": 42,
      "kind": "pixel | group | type | shape | smartobject",
      "visible": true,
      "opacity": 255,
      "blendMode": "normal",
      "bbox": [0, 0, 1980, 1080],
      "size": [1980, 1080],
      "textInfo": null,
      "children": []
    }
  ]
}
```

**textInfo（仅 `kind == "type"` 时非 null）**：

```json
{
  "text": "完整文本内容",
  "fontSize": 24,
  "fontName": "NotoSansSC-Bold",
  "color": [255, 255, 255],
  "leading": 32,
  "runs": [
    { "text": "片段", "fontSize": 24, "fontName": "...", "color": [255, 255, 255] }
  ]
}
```

---

## 脚本 2：psd_export.py — 图层导出

### 基本用法

```bash
# 导出所有可见图层
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_export.py "path/to/file.psd" -o ./output/

# 导出指定图层（按名称）
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_export.py "path/to/file.psd" -o ./output/ \
  --layers "对话框" "头像组件" "背景"

# 包含匹配模式
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_export.py "path/to/file.psd" -o ./output/ \
  --layers "头像" --match contains

# 同时导出完整合成参考图
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_export.py "path/to/file.psd" -o ./output/ \
  --composite

# 指定输出格式
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_export.py "path/to/file.psd" -o ./output/ \
  --format webp --quality 90

# 仅可见图层 + 跳过manifest
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_export.py "path/to/file.psd" -o ./output/ \
  --visible-only --no-manifest
```

### 命令行参数

| 参数 | 说明 |
|------|------|
| `PSD_PATH` | PSD 文件路径（必填） |
| `-o, --output-dir` | 输出目录（必填），不存在时自动创建 |
| `--layers` | 要导出的图层名列表，不指定则导出所有顶层图层 |
| `--match` | 匹配模式：`exact`（默认）或 `contains` |
| `--visible-only` | 仅导出可见图层 |
| `--composite` | 同时导出完整 PSD 合成参考图 |
| `--flatten-groups` | 组图层导出为合并后的单张图片（默认行为） |
| `--expand-groups` | 组图层展开为子图层分别导出 |
| `--format` | 输出格式：`png`（默认）、`webp`、`jpg` |
| `--quality` | JPEG/WebP 质量 1-100（默认 95） |
| `--no-manifest` | 不生成 manifest.json |
| `--prefix` | 文件名前缀，如 `ui-` → `ui-dialogue-box.png` |
| `--verbose` | 打印详细处理日志 |

### 输出说明

每导出一个图层，会：
1. 保存图像文件（以安全化的图层名命名）
2. 在 `manifest.json` 中记录该图层的元信息

**manifest.json 格式**：

```json
{
  "psd_size": [1980, 1080],
  "exported_at": "2026-03-21T16:30:00",
  "format": "png",
  "layers": [
    {
      "id": "safe-name",
      "originalName": "原始图层名",
      "file": "safe-name.png",
      "kind": "pixel",
      "bbox": [0, 0, 1980, 1080],
      "size": [1980, 1080]
    }
  ]
}
```

> [!TIP]
> **Web 开发中使用 manifest 定位**：bbox 格式为 `[left, top, right, bottom]`，直接换算为百分比定位：
> ```js
> function bboxToCSS(bbox, psdW, psdH) {
>   const [x1, y1, x2, y2] = bbox;
>   return {
>     left:   `${(x1 / psdW) * 100}%`,
>     top:    `${(y1 / psdH) * 100}%`,
>     width:  `${((x2 - x1) / psdW) * 100}%`,
>     height: `${((y2 - y1) / psdH) * 100}%`,
>   };
> }
> ```

---

## 典型工作流

### 场景 A：PSD 前期调研
```bash
# 1. 先看完整图层树，了解结构
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_analyze.py "design.psd" -o layers.json

# 2. 过滤找到需要的图层
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_analyze.py "design.psd" --name "button" --match contains
```

### 场景 B：提取 UI 素材用于 Web 开发
```bash
# 1. 导出指定 UI 图层 + 合成参考图
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_export.py "design.psd" -o ./assets/ \
  --layers "对话框" "头像组件" "按钮" --composite

# 2. 在代码中使用 manifest.json 中的 bbox 定位图层
```

### 场景 C：导出全部图层用于 Canvas 分层渲染
```bash
py -3 -X utf8 .codex/skills/psd-toolkit/scripts/psd_export.py "scene.psd" -o ./canvas-layers/ \
  --visible-only --composite --format webp --quality 90
```

---

## 最佳实践

- **名称冲突**：当 PSD 中存在同名图层时，导出文件名会自动附加 `layerId`（如 `background-42.png`）
- **负坐标 bbox**：某些图层（如对话框衬底）的 bbox 可能超出画布范围（如 `[-230, 581, 2245, 1133]`），这是 PSD 的正常行为，manifest 中会如实记录
- **组图层导出**：默认将组合并为单张图片（`--flatten-groups`），适合直接使用；用 `--expand-groups` 可展开为子图层分别导出
- **文本图层**：分析脚本会提取 `textInfo`（字体、颜色、大小等），但导出脚本只导出栅格化后的像素，不保留可编辑文本
- **Windows 编码**：始终使用 `py -3 -X utf8` 运行，避免中文图层名报错
