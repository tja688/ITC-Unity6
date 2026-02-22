---
description: 批量剧本转译Yarn与游戏接入流程 (Script to Yarn Pipeline)
---

# 批量剧本转译Yarn与游戏接入流程

当用户提供一段原始游戏剧本、或导演稿文本时，请完整遵循以下标准操作流程 (Workflow)，将文本可靠地转移为可直接在 Unity WebGL 架构下运行的 `.yarn` 脚本。

## 阶段 1：理解与映射 Yarn 剧本 (Drafting)

1. **阅读输入格式**：通读用户发来的原版剧本或者导演拆解稿件。
2. **结构转换**：
   - 为每个核心段落创建唯一的 Yarn 逻辑节点。
   - 使用 `<<itc_bg ...>>` 声明式加载和切换背景图。
   - 使用 `<<itc_npc_main ...>>`，`<<itc_pc_avatar ...>>`，`<<itc_npc_avatar ...>>` 调用立绘。
   - 参考《目前已有的素材清单.md》匹配可用资产。如果不确定或未开发好的图片文件，使用通用占位图作为托底。
3. **文本表现与禁塑规则 (CRITICAL)**：
   - ⚠️ **严禁**：不要使用针对局部文字的放大/形变/震动等 inline Text Animator tag（例如 `<incr>`, `<shake>`, `<wave>`, `<wiggle>`, `<size>`）。
   - ✅ **允许**：务必保留用于行内停顿和节奏的 `<waitfor=0.15>`，以及针对该行全局显示的 `|fade|` 或 `|typewriter|` 前缀。

## 阶段 2：小游戏与复杂逻辑的“占位符替代”

1. 用户如果在剧本提及了“开始小游戏”、“弹窗签约界面”、“操作引导”等复杂逻辑。
2. 除非有明确的预先集成指令，否则**一律采用简单占位节点**跳过。可以直接输出例如：
   ```yarn
   <<itc_npc_main_hide>>
   Narrator: |typewriter|（此处应该弹出一个小游戏/教程流程，目前暂用占位跳过）
   <<jump Next_Scene_Node>>
   ```
   以此保证当前脚本白盒测试阶段不会抛出无法解析的未知 Command 报错，阻塞测试。

## 阶段 3：本地编译校验 (Validation)

编写完毕保存到 `ITC_YarnWorkSpace` 目录后，务必在 PowerShell 中运行以下校验脚本，必须保证 Exit code 为 0：

```bash
# 执行 Yarn 语法与死胡同验证
.\.agent\skills\itc-yarn-authoring\scripts\check_yarn.ps1 -Path "Assets\Doc\ITC Doc\dialogue\ITC_YarnWorkSpace\你的文件名.yarn"
```

## 阶段 4：AB包重建与资源绑定治理 (Asset Bundling)

新增或变更了 Yarn 文件后，**必须**同步处理项目的 AssetBundle 分发机制才能使代码生效。

1. **白名单更新**：如果新建了 `yarn` 文件或者新引用的外部系统不在 `Editor/ITCDialogueResKitBuildTools.cs` 代码映射内，你需要确保新资产的 `assetBundleName` 设置为 `dialogue_script` （可通过工具脚本执行）。
2. **检查占位资产错误**：在 `Assets/Resources/Dialogue/DialogueVisualCatalog.asset` 中，如果有新增立绘无法匹配，自动 fallback 的图片若为切片图(例如 `通用标准立绘.png`)，必须在 YAML 文件中为其正确指定切片图名称 (`subSpriteName: 通用标准立绘_0`)。
3. **打包刷新操作**：
   - 调用 Unity MCP 执行菜单项: `Tools/ITC/Dialogue/Build WebGL ResKit Bundles`
   - 通过 `mcp_unityMCP_read_console` 查看日志。确保没有任何类似 `Build canceled because DialogueVisualCatalog validation failed` 等字样。
   - 若一切无误，表明 AB 包生成完毕。随时可用 Unity MCP 直接 Play 检查首帧视觉效果。

## 阶段 5：更新剧本测试清单 (Test Catalog Update)

每次成功转换/新增/修改完 `.yarn` 脚本后，负责产出的 AI **必须**将该脚本的核心信息手动登记到测试清单中，以保证 “Yarn快进跳转按钮” (`YarnDialogueFastForward` 测试组件关联了此数据) 能够获取最新节点。
目标资产文件为：`Assets/Doc/ITC Doc/dialogue/ITC_YarnWorkSpace/YarnTestCatalog.asset`。

操作规范如下：
1. **生成数据：** 你需要根据编写好的 `.yarn` 脚本，梳理以下信息：
   - `yarnFileName`: Yarn文件名称
   - `startNode`: 该文件入口节点名 
   - `endNode`: 结尾跳转节点名或者实际跳转的目标节点
   - `description`: 极其简短地概括该部分的重点剧情内容
   - `commandsUsed`: 涉及的关键指令（如bg, npc_main, load_scene 等）
   - `variableNodes`: 文件内部抛出/消耗所有Yarn变量的关键节点名列表

2. **追加写入：** 
由于这是 Unity `ScriptableObject`，建议你直接修改代码 `Assets/Editor/YarnTestCatalogGenerator.cs` 在它的 `catalog.files = new List<YarnTestFileInfo>` 里面增加最新这一个 `.yarn` 元素，然后执行该脚本提供的自定义菜单项方法： 
   - `mcp_unityMCP_execute_menu_item(menu_path: "Tools/ITC/Generate Yarn Test Catalog")`  
来重新生成和更新 `YarnTestCatalog.asset` 中的内容。

这个操作不仅描述了 Yarn 文件的情况以便理解，也自动化地使跳转系统获取最新的节点点位。
