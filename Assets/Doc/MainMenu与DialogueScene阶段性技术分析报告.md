# ITC 项目阶段性技术分析报告（MainMenu + DialogueScene）

版本：v1.0  
日期：2026-02-16  
面向对象：项目高层、技术负责人、制作人

## 1. 报告目标与范围

本报告聚焦当前已落地的两条运行入口：

1. `MainMenu` 场景
2. `DialogueScene` 场景

重点覆盖：

1. 工作流（从场景启动到对话运行）
2. 构建组成（场景、Prefab、AssetBundle、Yarn 数据）
3. WebGL 适配与当前风险
4. 对话系统程序架构（Yarn + QFramework + Text Animator）
5. 与行业最佳实践对比及下一阶段建议

---

## 2. 高层结论（先看这个）

### 2.1 当前阶段已完成的关键能力

1. 已形成“双入口、同一对话核心”的结构：`MainMenu` 与 `DialogueScene` 最终都走向同一套 `ITC DialogueSystem` 运行时。
2. 已形成 WebGL 友好的资源加载主路径：`ResKit.InitAsync()` + AssetBundle 分包 + StreamingAssets 发布。
3. 对话链路已打通：Yarn 节点、角色立绘/背景切换命令、打字机表现、输入继续/跳过、事件桥接全部可运行。
4. 项目在结构上具备“继续扩内容”的基础（Yarn 数据节点与变量体系已具规模）。

### 2.2 当前阶段主要风险

1. 发布治理风险：`EditorBuildSettings` 当前仅包含测试场景，未纳入 `MainMenu/DialogueScene`，流程依赖人工记忆。  
证据：`ProjectSettings/EditorBuildSettings.asset:7`, `ProjectSettings/EditorBuildSettings.asset:9`
2. 异步容错风险：UI 异步打开链路缺少失败回调与超时兜底，资源缺失时存在“等待不返回”风险。  
证据：`Assets/QFramework/Toolkits/UIKit/Scripts/UIKit.cs:101`, `Assets/QFramework/Toolkits/UIKit/Scripts/UIKit.cs:104`, `Assets/QFramework/Toolkits/SupportOldQF/Scripts/UIKitWithResKitInit.cs:53`
3. WebGL 发布策略风险：异常级别不是最小（`webGLExceptionSupport: 1`），发布包体与运行开销偏高。  
证据：`ProjectSettings/ProjectSettings.asset:563`
4. 交互预期风险：主菜单“退出”按钮在 WebGL 上不会真正退出页面（当前逻辑仅 `Application.Quit()`）。  
证据：`Assets/Scripts/MainMenu/MainMenuExitButton.cs:76`

### 2.3 阶段判断

当前可评估为：**可继续推进内容生产（APPROVE_WITH_RISKS）**。  
建议在下一阶段先补“发布与容错治理”，再扩大剧情和玩法体量。

---

## 3. 系统总览（程序与引擎层）

### 3.1 运行结构总图

```text
[MainMenu Scene] --------------------\
                                      -> ResKit.InitAsync -> UIKit.OpenPanelAsync -> ITC DialogueSystem(prefab)
[DialogueScene Scene] ---------------/

ITC DialogueSystem 内部：
DialogueRunner(Yarn) + TALinePresenter(Text Animator) + ITCDialoguePanel(视觉切换命令) + DialogueContinueHandler(输入)
```

### 3.2 架构分层（QFramework 视角）

1. 架构根：`MainMenuApp`  
证据：`Assets/Scripts/MainMenu/MainMenuApp.cs:3`
2. 状态模型：`MainMenuStateModel`（`BindableProperty`）  
证据：`Assets/Scripts/MainMenu/MainMenuStateModel.cs:5`
3. 状态变更路径：通过 `AbstractCommand` 触发 UI 打开/关闭  
证据：`Assets/Scripts/MainMenu/MainMenuCommands.cs:13`
4. UI 加载：`UIKit` + `ResKitPanelLoaderPool`  
证据：`Assets/Scripts/MainMenu/MainMenuUIKitConfig.cs:8`

整体上满足 QFramework “命令驱动状态变更”的主方向，当前阶段模型简单、可理解性较好。

---

## 4. MainMenu 工作流拆解

## 4.1 场景组成

`MainMenu.unity` 只有两个 Root：

1. `ITC Main Camera`（Prefab）
2. `MainMenuFlowLauncher`（启动脚本）

证据：`Assets/Scenes/MainMenu.unity:229`

### 4.2 启动时序

1. `Awake` 注入 UIKit 配置（ResKit 面板加载池）  
证据：`Assets/Scripts/MainMenu/MainMenuFlowLauncher.cs:15`
2. `Start` 执行 `ResKit.InitAsync()`  
证据：`Assets/Scripts/MainMenu/MainMenuFlowLauncher.cs:29`
3. 发送 `MarkMainMenuResReadyCommand` 与 `OpenMainMenuPanelCommand`
4. 异步打开 `MainMenuPanel`（bundle=`menu_core`）  
证据：`Assets/Scripts/MainMenu/MainMenuCommands.cs:25`
5. 可选演示流（默认关闭）：等待菜单面板打开后自动请求打开对话面板  
证据：`Assets/Scenes/MainMenu.unity:151`

### 4.3 主菜单交互机制

`MainMenuPanel.prefab` 的 3 个可点击按钮都挂 `MainMenuExitButton`，通过名称关键词决定行为：

1. 含“退出”关键词 -> `Application.Quit()`
2. 含“开始新游戏”关键词 -> 发送 `RequestOpenDialoguePanelCommand`
3. 其他按钮 -> 当前无动作

证据：`Assets/Scripts/MainMenu/MainMenuExitButton.cs:52`

### 4.4 从主菜单进入对话

`RequestOpenDialoguePanelCommand`：

1. 检查资源初始化与重复请求状态
2. `UIKit.OpenPanelAsync<ITCDialoguePanel>`（bundle=`dialogue_ui`）
3. 成功后关闭 `MainMenuPanel`
4. 标记 `DialoguePanelOpened = true`

证据：`Assets/Scripts/MainMenu/MainMenuCommands.cs:43`

---

## 5. DialogueScene 工作流拆解

## 5.1 场景组成

`DialogueScene.unity` 同样是极简两 Root：

1. `ITC Main Camera`（同一个摄像机 prefab）
2. `DialogueSceneFlowLauncher`

证据：`Assets/Scenes/DialogueScene.unity:228`

### 5.2 启动时序

1. `Awake` 设置同一套 UIKit 配置
2. `Start` 先 `ResKit.InitAsync()`
3. 默认延时 `0.2s` 后直接打开 `ITCDialoguePanel`
4. `StartNode` 默认 `ITC_Start`

证据：`Assets/Scripts/Dialogue/DialogueSceneFlowLauncher.cs:24`

结论：`DialogueScene` 是“无菜单直入对话”的快速入口，适合剧情开发/测试。

---

## 6. 对话系统构建与运行机制

## 6.1 运行时核心 Prefab

`ITC DialogueSystem.prefab` 包含：

1. `ITCDialoguePanel`（业务主控）
2. `DialogueRunner`（Yarn 运行器）
3. `TALinePresenter`（文本呈现）
4. `DialogueContinueHandler`（输入）
5. `TADialogueEvents`（事件桥）
6. UI 层：背景图、立绘图、文本区、姓名区

证据：`Assets/Prefabs/UI/ITC DialogueSystem.prefab:986`

### 6.2 Yarn 运行链路

1. `DialogueRunner` 绑定 `TestProject.yarnproject`  
证据：`Assets/Prefabs/UI/ITC DialogueSystem.prefab:1461`
2. 本地化行提供器语言为 `zh`  
证据：`Assets/Prefabs/UI/ITC DialogueSystem.prefab:1513`
3. Yarn 主入口节点为 `ITC_Start`  
证据：`Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn:1`

### 6.3 命令驱动视觉切换

`ITCDialoguePanel` 在运行时注册 Yarn 命令：

1. `itc_bg` 切背景
2. `itc_npc` 切 NPC 立绘
3. `itc_pc` 切 PC 立绘
4. `itc_npc_hide` / `itc_pc_hide` 隐藏立绘

证据：`Assets/Scripts/Dialogue/ITCDialoguePanel.cs:375`

Yarn 脚本中已大量使用该命令体系。  
证据：`Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn:4`

### 6.4 文本表现（Text Animator）

`TALinePresenter` 实现关键能力：

1. 行开始/结束渐隐渐显
2. 打字机完成、Skip、继续二阶段等待
3. 解析前导 `|tag|` 并合入默认效果标签
4. 与 Yarn `LineCancellationToken` 协作

证据：`Assets/Scripts/Dialogue/TALinePresenter.cs:116`

### 6.5 输入与事件桥接

1. `DialogueContinueHandler`：空格继续、回车跳过、鼠标左键继续  
证据：`Assets/Scripts/Dialogue/DialogueContinueHandler.cs:59`
2. `TADialogueEvents`：在 Skip 期间按黑白名单过滤高风险事件（如 `playsound`, `shake`）  
证据：`Assets/Scripts/Dialogue/TADialogueEvents.cs:69`

---

## 7. 构建组成（Assets + Bundle + 场景）

## 7.1 AssetBundle 标记与构建工具

项目已提供专用编辑器工具：

1. `Tools/ITC/Dialogue/Apply AssetBundle Labels`
2. `Tools/ITC/Dialogue/Build WebGL ResKit Bundles`

证据：`Assets/Editor/ITCDialogueResKitBuildTools.cs:17`

已确认关键资产标签：

1. `MainMenuPanel.prefab -> menu_core`
2. `ITC DialogueSystem.prefab -> dialogue_ui`
3. `TestProject.yarnproject` + `.yarn -> dialogue_script`
4. 背景图 -> `dialogue_bg`
5. 立绘图 -> `dialogue_portrait`

### 7.2 Bundle 依赖关系（已构建产物）

1. `menu_core`：包含 `MainMenuPanel.prefab`，无依赖  
证据：`Assets/StreamingAssets/AssetBundles/WebGL/menu_core.manifest:44`
2. `dialogue_script`：包含 `TestProject.yarnproject` + `ITC_YARN_DATA_TEXT_ANIM.yarn`  
证据：`Assets/StreamingAssets/AssetBundles/WebGL/dialogue_script.manifest:33`
3. `dialogue_ui`：包含 `ITC DialogueSystem.prefab`，依赖 `dialogue_bg` + `dialogue_portrait` + `dialogue_script`  
证据：`Assets/StreamingAssets/AssetBundles/WebGL/dialogue_ui.manifest:123`

### 7.3 当前包体体量（WebGL 目录）

1. `dialogue_bg`: 56,089,355 bytes（约 53.5 MiB）
2. `dialogue_ui`: 3,716,662 bytes（约 3.55 MiB）
3. `dialogue_portrait`: 177,549 bytes（约 0.17 MiB）
4. `dialogue_script`: 17,775 bytes（约 0.02 MiB）
5. `menu_core`: 191,537 bytes（约 0.18 MiB）

结论：当前主要下载压力集中在背景资源包。

---

## 8. WebGL 适配现状评估

## 8.1 已落实的 WebGL 友好处理

1. 资源初始化统一使用 `ResKit.InitAsync()`，符合 WebGL 异步初始化约束。  
证据：`Assets/Scripts/MainMenu/MainMenuFlowLauncher.cs:29`, `Assets/Scripts/Dialogue/DialogueSceneFlowLauncher.cs:32`
2. `webGLThreadsSupport: 0`，避免 COOP/COEP 部署复杂度。  
证据：`ProjectSettings/ProjectSettings.asset:576`
3. `webGLDataCaching: 1`，符合重复访问缓存优化。  
证据：`ProjectSettings/ProjectSettings.asset:566`
4. `webGLDecompressionFallback: 0`，默认走服务端压缩头策略。  
证据：`ProjectSettings/ProjectSettings.asset:577`

### 8.2 当前扫描结果（按业务脚本范围）

使用项目内 WebGL 自检脚本（仅 `Assets/Scripts/**/*.cs`）：

1. BLOCKER: 3（均来自 `TALinePresenter` 对 `TaskCompletionSource` 的使用）
2. WARNING: 1（`webGLExceptionSupport: 1`）

说明：规则引擎将 `System.Threading` 统一视作阻断项，但这里并未创建线程；是“保守规则命中”，不是必然运行时崩溃。

### 8.3 与最佳实践的偏差

1. 发布异常级别建议下调到最小（Release）以减小包体与开销。
2. 需补充部署侧文档（`wasm` MIME、压缩头、CORS/CSP），仓库内暂无成文配置模板。
3. “退出游戏”在 WebGL 需要明确产品行为（返回菜单/弹窗提示），不应依赖 `Application.Quit()`。

---

## 9. 行业最佳实践对比（阶段版）

| 维度 | 当前做法 | 行业常见最佳实践 | 结论 |
|---|---|---|---|
| 场景启动结构 | 极简场景 + 启动器脚本 + 异步开 UI | 同类项目常用“轻壳场景”降低切场景成本 | 优势 |
| 资源组织 | UI/脚本/背景/立绘分包，依赖清晰 | 按“变更频率 + 体量”分包 | 优势 |
| 对话引擎 | Yarn + 自定义命令驱动视觉 | 剧情系统与表现层解耦 | 优势 |
| WebGL 初始化 | 全量 `InitAsync` | WebGL 强制异步 IO | 优势 |
| 异步容错 | 缺少失败回调和超时 | 必须有“失败可观测 + 可恢复” | 明显短板 |
| 发布治理 | BuildSettings 未纳入正式场景 | 场景白名单和构建配置受控 | 明显短板 |
| 日志治理 | 业务层仍有 `Debug.Log` | 分级日志与开关化埋点 | 可改进 |
| 运行配置 | 例外策略（异常级别）未分环境 | Debug/Release 双配置 | 可改进 |

---

## 10. 下一阶段建议（按优先级）

### P0（立即）

1. 建立正式构建清单：将 `MainMenu` 与 `DialogueScene` 纳入可追踪构建配置。
2. 给 `OpenPanelAsync` 链路加失败回调与超时（避免资源丢失时卡死）。
3. 定义 WebGL“退出”交互（例如返回标题或弹提示）。
4. 增加 WebGL 发布配置分档（Debug/Release），至少调整异常策略。

### P1（近期）

1. 将 `ITCDialoguePanel` 的大量映射数据从代码内迁移到可维护数据资产（ScriptableObject/表）。
2. 为主流程补 2 类自动化验证：  
场景冒烟（能否进面板）  
Yarn 命令映射（`itc_bg/itc_npc/itc_pc`）
3. 增补 WebGL 部署手册（CDN/Header/CORS/CSP）。

### P2（中期）

1. 将 `DialogueScene` 的状态依赖从 `MainMenuApp` 中进一步解耦为独立域架构。
2. 对背景包做二次拆分与压缩策略优化（降低首屏带宽）。
3. 增加运行指标采集（首屏时长、首句时长、包命中率）。

---

## 11. 关键证据索引（可审计）

1. 启动入口：`Assets/Scripts/MainMenu/MainMenuFlowLauncher.cs:20`, `Assets/Scripts/Dialogue/DialogueSceneFlowLauncher.cs:24`
2. 命令流：`Assets/Scripts/MainMenu/MainMenuCommands.cs:13`
3. 菜单按钮行为：`Assets/Scripts/MainMenu/MainMenuExitButton.cs:52`
4. 对话面板控制器：`Assets/Scripts/Dialogue/ITCDialoguePanel.cs:247`
5. 行呈现器：`Assets/Scripts/Dialogue/TALinePresenter.cs:116`
6. 输入处理：`Assets/Scripts/Dialogue/DialogueContinueHandler.cs:59`
7. 事件桥接：`Assets/Scripts/Dialogue/TADialogueEvents.cs:32`
8. 场景结构：`Assets/Scenes/MainMenu.unity:229`, `Assets/Scenes/DialogueScene.unity:228`
9. 预制体与 Yarn 绑定：`Assets/Prefabs/UI/ITC DialogueSystem.prefab:1461`
10. WebGL 设置：`ProjectSettings/ProjectSettings.asset:562`
11. BuildSettings 现状：`ProjectSettings/EditorBuildSettings.asset:7`
12. Bundle 依赖：`Assets/StreamingAssets/AssetBundles/WebGL/dialogue_ui.manifest:123`

