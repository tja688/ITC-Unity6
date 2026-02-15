# DialogueScene WebGL 核心流程开发文档

## 1. 目标与范围
- 目标：从 `Assets/Scenes/MainMenu.unity` 的“开始新游戏”按钮进入 `Assets/Scenes/DialogueScene.unity`，自动启动 `Assets/Doc/ITC Doc/dialogue/TestProject.yarnproject` 对话演出。
- 对话演出要求：
  - 使用 `Assets/Prefabs/UI/ITC DialogueSystem.prefab`。
  - 在 `Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn` 中插入背景/立绘命令。
  - 支持占位流程跳过小游戏与下班循环，收敛到 Demo 结束节点。
- 资源策略：采用 QFramework ResKit 模拟打包路径（不做真实 AB 构建），但保持正式 Bundle 结构与加载接口。

## 2. 架构与职责（QFramework）

### 2.1 MainMenu 侧
- `MainMenuFlowLauncher`：场景入口，`ResKit.InitAsync()` 后打开主菜单面板。
- `MainMenuPanel`：`OnInit/OnOpen` 时扫描并绑定“开始新游戏”按钮组件。
- `MainMenuStartButton`：点击发送 `RequestEnterDialogueSceneCommand`。
- `RequestEnterDialogueSceneCommand`：
  - 优先使用 ResKit 场景路径加载 `DialogueScene`。
  - 若 ResKit 配置缺失则回退 `SceneManager.LoadSceneAsync`。
  - 全流程写入 `MainMenuStateModel` 状态与 `LogKit` 调试日志。
- `MainMenuStateModel` 新增状态：
  - `SceneTransitionInProgress`
  - `SceneTransitionTarget`
  - `SceneTransitionFallbackUsed`

### 2.2 DialogueScene 侧
- `DialogueSceneFlowLauncher`：
  - 通过 `RuntimeInitializeOnLoadMethod` 在进入 `DialogueScene` 时自动创建。
  - `ResKit.InitAsync()` 后加载 `ITC DialogueSystem` 预制体。
  - 预制体先 `SetActive(false)`，挂载视觉运行时组件后再激活，保证命令桥可用。
- `DialogueSceneApp` + `DialogueSceneStateModel`：
  - 管理 `ResKitReady`、`DialogueSystemSpawned`、`VisualLayerReady`、`DialogueRunning`、`SpawnFallbackUsed`、`ActiveBackgroundKey`、`ActivePortraitKey`。
- `ITCDialogueVisualRuntime`：
  - 提供 Yarn 命令：
    - `<<itc_bg key>>`
    - `<<itc_portrait key>>`
    - `<<itc_hide_portrait>>`
    - `<<itc_debug marker>>`
  - 在 `DialogueCanvas` 下自动创建 `ITCVisualLayer`，包含背景层和立绘层。
  - 通过 ResKit 根据 key 加载 Sprite，并写入状态模型。
- `ITCDialogueVisualCatalog`：
  - 维护背景/立绘 key 到 `(bundle, asset)` 的映射，实现脚本与素材解耦。

## 3. Yarn 改造说明
- 文件：`Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn`
- 已在关键节点插入：
  - `itc_debug`（流程埋点）
  - `itc_bg`（场景背景切换）
  - `itc_portrait`（角色立绘切换）
- 小游戏占位：
  - `ITC_Client1_TypewriterInput`
  - `ITC_Client2_TypewriterInput`
  - 改为“默认一次失误”占位逻辑，继续主流程。
- 下班环节占位：
  - `ITC_Act4_EndingEntry` 非结局分支直接跳 `ITC_End_Demo`。
  - `ITC_Act5_AfterWork` 改为占位并跳 `ITC_End_Demo`。

## 4. ResKit 模拟打包设计（正式路径）

### 4.1 Bundle 分层
- `entry_flow`：`MainMenu.unity`
- `dialogue_flow`：`DialogueScene.unity`
- `dialogue_core`：`ITC DialogueSystem.prefab`
- `dialogue_text`：`TestProject.yarnproject` + `ITC_YARN_DATA_TEXT_ANIM.yarn`
- `dialogue_art_bg`：占位背景原画
- `dialogue_art_portrait`：占位人物立绘
- 兼容已有：`menu_core`、`menu_fx`

### 4.2 一键配置工具
- 文件：`Assets/Editor/ITCDialogueWebGLPackagingTool.cs`
- 菜单：`ITC/Dialogue/Apply WebGL Simulation Packaging`
- 功能：
  - 批量写入上述 AB 名称到对应 `.meta`。
  - 更新 Build Settings 场景列表（MainMenu -> DialogueScene -> TestScene）。
  - 调用 `AssetBundleExporter.BuildDataTable(...)` 生成 `asset_bundle_config.bin`。

### 4.3 生成物
- `Assets/StreamingAssets/AssetBundles/WebGL/asset_bundle_config.bin` 已更新。
- `Assets/QFrameworkData/QAssets.cs` 已包含新增 bundle 常量（用于配置可读性与后续扩展）。

## 5. 调试打点设计
- MainMenu 阶段：
  - 按钮绑定数量、按钮点击、场景切换路径（ResKit/回退）。
- DialogueScene 阶段：
  - AutoBootstrap、ResKit 初始化、预制体加载结果、对话启动时机。
- Yarn 演出阶段：
  - `itc_debug` 标记日志。
  - 背景/立绘切换日志（key/bundle/asset）。
  - 资源查找失败日志（便于快速定位 key/AB 配置问题）。

## 6. WebGL 约束与扫描
- 运行了自检脚本：
  - `python .codex/skills/unity-webgl/scripts/webgl_self_check.py --repo-root . --json-out Temp/unity-webgl-check.json --md-out Temp/unity-webgl-check.md`
- 结果：
  - 报告存在大量 `BLOCKER`，主要来自现有编辑器工具与第三方代码（`Assets/Editor/*`, `Assets/MCPForUnity/*`, `Assets/QFramework/*`）中的线程/网络 API，非本次新增逻辑引入。
  - 本次新增运行时代码未引入新的 WebGL 线程或 `System.Net` 依赖。
  - `WARNING`：`webGLExceptionSupport` 非最小化（当前值 `1`）。

## 7. 验证步骤
1. 打开 `MainMenu` 场景并运行。
2. 点击“开始新游戏”按钮。
3. 确认进入 `DialogueScene` 并自动出现 `ITC DialogueSystem`。
4. 观察对话推进中背景与立绘变化。
5. 验证占位流程：
   - 打字机小游戏节点为占位自动路径。
   - 下班循环不再进入次日循环，最终停在 `ITC_End_Demo`。
6. 查看 Console 中 `LogKit` 埋点，确认链路完整。

## 8. 后续可扩展建议
- 将 `ITCDialogueVisualCatalog` 从代码常量升级为 ScriptableObject 配置资产，支持策划无代码替换素材。
- 立绘升级为多角色分层（左/中/右）并加入轻量过渡动画（保留 WebGL 低开销）。
- 若准备上线 WebGL，建议分离或条件编译掉编辑器/调试工具中的线程与网络实现，处理自检中的历史 BLOCKER。
