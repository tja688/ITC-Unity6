# 02 符文控制板 QTE - 代码文档与操作说明（可直接落地）

> 适用范围：`RuneTyping` 小游戏白模与主线接入  
> 目标：保证“可独立测试、可一键重建、可回写 Yarn 变量”

---

## 1. 代码结构（本次落地）

### 1.1 业务层（QFramework）

- `Assets/Scripts/Contracting/Common/ContractingTypes.cs`  
  - 新增 `RuneTypingConfig / RuneTypingRoundConfig / RuneTypingPanelData / RuneTypingResultPayload`
- `Assets/Scripts/Contracting/Common/ContractFlowStateModel.cs`  
  - 新增 `RouteQteErrorCount / RuneTypingGridSize / RuneTypingRunning / RuneTypingCompleted`
- `Assets/Scripts/Contracting/Common/ContractClientConfigModel.cs`  
  - 新增 `RuneTypingRuleConfig` 与 `BuildRuneTypingRoundConfig`
- `Assets/Scripts/Contracting/Common/ContractCommands.cs`  
  - `BeginRuneTypingCommand`：开局写入基线状态  
  - `SubmitRuneTypingResultCommand`：统一收口写回 `RouteQteErrorCount`

### 1.2 UI 层（Panel）

- `Assets/Scripts/Contracting/RuneTyping/RuneTypingPanel.cs`  
  - 状态机：`Countdown -> Playing -> FeedbackCorrect/FeedbackError -> ComboResolve -> Completed`
  - 输入：`WASD + Enter/Space`，并保留屏幕方向按钮/确认按钮兜底
  - 判定：错误确认才计入 `errorCount`；边界外输入仅反馈不计错
  - 内嵌中文说明系统，支持一键开关
- `Assets/Scripts/Contracting/RuneTyping/RuneTypingGuideTag.cs`  
  - 说明标注对象的统一标记组件

### 1.3 流程桥接层（Yarn）

- `Assets/Scripts/Dialogue/ITCDialoguePanel.cs`  
  - 注册命令：`<<itc_rune_typing clientId gridSize>>`
  - 命令流程：打开面板 -> 等待完成 -> 回写 `$Route_QTEErrorCount` -> 关闭面板
- `Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn`  
  - `ITC_Client1_TypewriterInput` 与 `ITC_Client2_TypewriterInput` 从占位选项改为命令调用

---

## 2. 一键入口

### 2.1 白模 Prefab 重建（编辑器菜单）

- 菜单：`ITC/Contracting/重建 02符文控制板 白模Prefab`
- 实现：`Assets/Editor/Contracting/RuneTypingPanelPrefabBuilder.cs`
- 生成目标：`Assets/Prefabs/UI/RuneTypingPanel.prefab`

### 2.2 快速测试启动器

- 运行菜单：`ITC/Contracting/一键运行 02符文控制板全流程测试`
- 停止菜单：`ITC/Contracting/一键停止 02符文控制板全流程测试`
- 编辑器工具：`Assets/Editor/Contracting/RuneTypingQuickTestTools.cs`
- 运行时组件：`Assets/Scripts/Contracting/RuneTyping/RuneTypingQuickTestLauncher.cs`

---

## 3. 白模关键对象（运行时）

- `TargetSequenceRoot`：目标序列条
- `GridRoot`：5x5 复用网格（运行时按 4x4/5x5 开关）
- `CursorFrame`：光标框
- `OnScreenButtons`：屏幕按钮输入区
- `StatusText`：状态进度文本
- `ErrorCountText`：错误计数文本
- `GuideToggleButton`：说明开关

---

## 4. 变量与判定写回

- 写回变量：`$Route_QTEErrorCount`
- 不在本面板直接写：`$Sign_mistake` / `$satisfaction`
- 后续惩罚逻辑继续由既有 Yarn 判定节点处理（`C2_D155`）

---

## 5. 约束说明

- 保留 WebGL 兜底输入（屏幕按钮）
- 计时与状态流转使用协程，不使用线程与 `Task.Run`
- 面板关闭时注销输入监听，避免后续流程抢输入
- 白模符文显示统一使用 `上/下/左/右` 文本，避免字体缺字导致 `◀/▶` 显示为方框造成目标歧义
