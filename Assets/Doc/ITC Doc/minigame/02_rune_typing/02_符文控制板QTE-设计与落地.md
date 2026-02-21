# 02 符文控制板 QTE（RuneTyping）设计与落地说明

## 第一部分：落地设计

### 1. 定位与边界

- 流程位置：文书审核后，节点 `ITC_ClientX_TypewriterInput`
- 玩法目标：以“输入准确性”影响满意度/失误，不中断主流程
- 当前 Yarn 判定：
  - `Route_QTEErrorCount == 2` -> `satisfaction-1`
  - `Route_QTEErrorCount >= 3` -> `Sign_mistake=1`

---

### 2. 面板形态与入口

- Panel：`RuneTypingPanel : UIPanel`
- UILevel：`PopUI`
- 打开命令：`<<itc_rune_typing clientId gridSize>>`
- 数据输入：
  - `gridSize`（4/5）
  - `targetSequence`
  - `runeGridLayout`
  - `tuning`

---

### 3. 交互模型

#### 3.1 输入通道

- 主输入：`WASD + Confirm(Space/Enter)`
- 备选输入（WebGL焦点丢失兜底）：屏幕方向按钮 + 点击确认
- 输入缓冲：`80ms`
- 白模阶段符文文案推荐使用 `上/下/左/右`（而非特殊箭头字形），确保目标提示在常见 TMP 字体下可读

#### 3.2 错误定义

- 光标移动到错误符文并确认 -> 记 1 次错误
- 非法输入（边界外）不计错，但给边界反馈

#### 3.3 状态机

- `Countdown`
- `Playing`
- `FeedbackCorrect`
- `FeedbackError`
- `ComboResolve`
- `Completed`

---

### 4. 节奏与手感

#### 4.1 单局目标时长

- 4x4：12-20s
- 5x5：18-28s

#### 4.2 关键反馈时序

- 光标移动响应：`<= 50ms`
- 正确命中发光：`120ms`
- 错误闪烁：`100ms * 2`
- 完成收束动画：`700ms`（含打字机联动）

#### 4.3 手感参数建议

- 光标移动补间：`OutQuad, 0.08s`
- 连续输入节拍容忍：相邻确认最短间隔 `90ms`
- 错误时轻微屏幕震动：振幅 `2-4px`, 时长 `0.09s`

---

### 5. 判定与写回

- `$Route_QTEErrorCount = errorCount`
- 不在面板中直接写 `Sign_mistake/satisfaction`
- 统一由后续判定节点（`C2_D155` 或 Command 收口）处理

---

### 6. 可配置项（SO）

`RuneTypingConfig`：

- `gridWidth`
- `gridHeight`
- `targetSequenceLength`
- `targetSequence[]`
- `inputBufferMs`
- `cursorMoveDuration`
- `errorFlashDuration`
- `maxDisplayHintSeconds`
- `showOnScreenButtonsInWebGL`

难度曲线建议：

- Day1：固定 4x4，序列长度 4-5
- Day2+：按客户难度切 4x4/5x5 + 干扰符文

---

### 7. SFX/VFX 预留点

SFX cue：

- `sfx.contract.rune.move`
- `sfx.contract.rune.confirm`
- `sfx.contract.rune.error`
- `sfx.contract.rune.finish`

VFX cue：

- `vfx.contract.rune.correct_glow`
- `vfx.contract.rune.error_blink`
- `vfx.contract.rune.energy_flow`

---

### 8. UIKit/ResKit/WebGL 约束

- 网格格子复用池化，禁止每次开局动态new一批对象
- 序列提示文本使用预分配字符串模板
- WebGL 下不使用线程/Task.Run；计时全部走协程
- 面板关闭时清理输入监听，避免和后续面板抢输入

---

### 9. 独立测试（白模）

白模对象：

- NxN 方格
- 光标框（黄色）
- 顶部目标序列条
- 错误计数器

验收：

1. 4x4/5x5 都能跑
2. 错误计数与变量一致
3. 边界输入不穿透
4. WebGL 模拟低帧（30fps）下依然可完成

---

### 10. 与上下游接口

- 上游读 `clientId/gridSize`
- 下游输出 `Route_QTEErrorCount`
- 结束后可插入 `RuneVerify` 概率判定，或直接进 `Stamp`

---

## 第二部分：代码实施与操作说明

### 11. 代码结构（本次落地）

#### 11.1 业务层（QFramework）

- `Assets/Scripts/Contracting/Common/ContractingTypes.cs`  
  - 新增 `RuneTypingConfig / RuneTypingRoundConfig / RuneTypingPanelData / RuneTypingResultPayload`
- `Assets/Scripts/Contracting/Common/ContractFlowStateModel.cs`  
  - 新增 `RouteQteErrorCount / RuneTypingGridSize / RuneTypingRunning / RuneTypingCompleted`
- `Assets/Scripts/Contracting/Common/ContractClientConfigModel.cs`  
  - 新增 `RuneTypingRuleConfig` 与 `BuildRuneTypingRoundConfig`
- `Assets/Scripts/Contracting/Common/ContractCommands.cs`  
  - `BeginRuneTypingCommand`：开局写作基线状态  
  - `SubmitRuneTypingResultCommand`：统一收口写回 `RouteQteErrorCount`

#### 11.2 UI 层（Panel）

- `Assets/Scripts/Contracting/RuneTyping/RuneTypingPanel.cs`  
  - 状态机：`Countdown -> Playing -> FeedbackCorrect/FeedbackError -> ComboResolve -> Completed`
  - 输入：`WASD + Enter/Space`，并保留屏幕方向按钮/确认按钮兜底
  - 判定：错误确认才计入 `errorCount`；边界外输入仅反馈不计错
  - 内嵌中文说明系统，支持一键开关
- `Assets/Scripts/Contracting/RuneTyping/RuneTypingGuideTag.cs`  
  - 说明标注对象的统一标记组件

#### 11.3 流程桥接层（Yarn）

- `Assets/Scripts/Dialogue/ITCDialoguePanel.cs`  
  - 注册命令：`<<itc_rune_typing clientId gridSize>>`
  - 命令流程：打开面板 -> 等待完成 -> 回写 `$Route_QTEErrorCount` -> 关闭面板
- `Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn`  
  - `ITC_Client1_TypewriterInput` 与 `ITC_Client2_TypewriterInput` 从占位选项改为命令调用

---

### 12. 一键入口

#### 12.1 白模 Prefab 重建（编辑器菜单）

- 菜单：`ITC/Contracting/重建 02符文控制板 白模Prefab`
- 实现：`Assets/Editor/Contracting/RuneTypingPanelPrefabBuilder.cs`
- 生成目标：`Assets/Prefabs/UI/RuneTypingPanel.prefab`

#### 12.2 快速测试启动器

- 运行菜单：`ITC/Contracting/一键运行 02符文控制板全流程测试`
- 停止菜单：`ITC/Contracting/一键停止 02符文控制板全流程测试`
- 编辑器工具：`Assets/Editor/Contracting/RuneTypingQuickTestTools.cs`
- 运行时组件：`Assets/Scripts/Contracting/RuneTyping/RuneTypingQuickTestLauncher.cs`

---

### 13. 白模关键对象（运行时）

- `TargetSequenceRoot`：目标序列条
- `GridRoot`：5x5 复用网格（运行时按 4x4/5x5 开关）
- `CursorFrame`：光标框
- `OnScreenButtons`：屏幕按钮输入区
- `StatusText`：状态进度文本
- `ErrorCountText`：错误计数文本
- `GuideToggleButton`：说明开关

---

### 14. 变量与判定写回

- 写回变量：`$Route_QTEErrorCount`
- 不在本面板直接写：`$Sign_mistake` / `$satisfaction`
- 后续惩罚逻辑继续由既有 Yarn 判定节点处理（`C2_D155`）

---

### 15. 约束说明

- 保留 WebGL 兜底输入（屏幕按钮）
- 计时与状态流转使用协程，不使用线程与 `Task.Run`
- 面板关闭时注销输入监听，避免后续流程抢输入
- 白模符文显示统一使用 `上/下/左/右` 文本，避免字体缺字导致 `◀/▶` 显示为方框造成目标歧义
