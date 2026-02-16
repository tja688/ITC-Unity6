# ITC 对话视觉映射编辑器（策划使用手册）

## 1. 这个编辑器解决什么问题
这个编辑器用于把对话 key 和美术资源做可视化关联，目标是让策划可以直接拖拽配置，不需要写代码。

核心保证：
- 运行时走 `Sprite` 加载，不走 `Texture -> Sprite.Create`。
- 保留 Unity 导入元数据（`rect`、`pivot`、`pixelsPerUnit`、切片名）。
- 减少 WebGL 实际画面与美术场景摆放不一致的问题。

## 2. 如何打开
菜单：
- `Tools/ITC/Dialogue/Open Visual Catalog Editor`

也可以打开 `DialogueVisualCatalog.asset`，点击：
- `Open Planner Editor`

## 3. 推荐操作流程（每次改图都走一遍）
1. 点击 `Sync Keys From Yarn`，先把 Yarn 里新增 key 补齐。
2. 用拖拽把图片映射填上（单条拖拽或批量拖拽都可以）。
3. 检查 fallback 链路（失败时回退到谁）。
4. 点击 `Validate Visual Catalog`。
5. 点击 `Apply AssetBundle Labels`。
6. 需要打包时点击 `Build WebGL ResKit Bundles`。

## 4. 全局配置说明（必须理解）
窗口里这些配置都是完整可见、可直接编辑的：

`Default Bundle - Background`
- `Background` 槽位未填写 `Asset Bundle Name` 时，使用这个默认 bundle。

`Default Bundle - Portrait`
- `NpcMain`、`NpcAvatar`、`PcAvatar` 槽位未填写 bundle 时使用。

`Default Key - NpcMain`
- `itc_npc_main <key>` 失败时，自动尝试这个默认 key。

`Default Key - NpcAvatar`
- `itc_npc_avatar <key>` 失败时，自动尝试这个默认 key。

`Default Key - PcAvatar`
- `itc_pc_avatar <key>` 失败时，自动尝试这个默认 key。

`Missing Bundle / Missing Asset Name / Missing Sub Sprite / Missing Editor Asset Path`
- 全部 key 与 fallback 都失败后，最终缺图占位使用这里的配置。

## 5. 映射行字段说明（每条都可手改）
每一行是一个 `key + slot` 的映射关系。

`Key`
- 对话指令侧使用的逻辑名，需和 Yarn 命令参数一致。

`Slot`
- 四选一：`Background`、`NpcMain`、`NpcAvatar`、`PcAvatar`。

`Asset Bundle Name`
- 当前行单独指定 bundle。
- 为空时自动使用该槽位默认 bundle。

`Asset Name`
- Bundle 内主资源名（一般是纹理资源名，不是完整路径）。

`Sub Sprite Name`
- 多切片图（Multiple Sprite）必填对应切片名。
- 单图（Single Sprite）可留空。

`Fallback Key`
- 本 key 失败后回退到同槽位的哪个 key，可多级链式回退。

`Editor Asset Path`
- 编辑器模拟加载和校验使用的工程路径。

`Source Sprite (Drag To Auto Fill)`
- 把 Sprite 拖到这里会自动回填：
  - `Editor Asset Path`
  - `Asset Name`
  - `Sub Sprite Name`（如果源图是多切片）

## 6. 批量导入面板（最常用）
支持直接拖入：
- `Sprite`
- `Texture2D`
- 文件夹（会递归收集可用 Sprite）

批量参数：
- `Target Slot`：导入目标槽位。
- `Key Prefix`：可选 key 前缀。
- `Set Fallback To Slot Default`：自动把 fallback 设为槽位默认 key。
- `Fill Bundle With Slot Default`：自动写入默认 bundle。
- `Replace Existing Same Slot+Key`：遇到同槽位同 key 时覆盖旧映射，不新增重复项。

## 7. 构建前质量门槛
至少执行：
1. `Validate Visual Catalog`
2. `Apply AssetBundle Labels`

校验会检查：
- key+slot 唯一性
- Yarn 引用 key 是否全覆盖
- 资源是否存在且为 Sprite 类型
- Multiple Sprite 是否正确填写 `Sub Sprite Name`
- fallback 链是否闭合（无死链、无环）

## 8. 常见问题排查
`画面里人物特别小 / 比例不对`
- 常见原因是切片名错误，或引用到了错误 Sprite。
- 处理：在该行把正确 Sprite 拖到 `Source Sprite`，让系统自动回填字段。

`校验报 Asset not found`
- 优先检查 `Asset Bundle Name` 与 `Asset Name`。
- 如果 bundle 留空，确认全局默认 bundle 是否正确。

`校验报 Missing fallback target`
- 说明 `Fallback Key` 指向了不存在的同槽位 key。
- 新建目标 key，或改 fallback 指向。

`Yarn 里有 key 但 Catalog 没有`
- 先点 `Sync Keys From Yarn` 生成占位项，再补图。

## 9. 命名建议
- key 保持稳定、可读：例如 `character_pose_variant`。
- Yarn 已引用的 key 尽量不要频繁改名。
- 在美术迭代期，优先替换映射资源，不要改 key。

## 10. 为什么这套链路能改善 WebGL 一致性
- 运行时直接加载 `Sprite`（AB/Simulation），不重建 Sprite。
- 导入元数据沿用 Unity 美术设置。
- 避免运行时二次创建带来的比例和布局偏差。
