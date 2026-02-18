# 02 符文控制板 QTE（RuneTyping）落地设计

## 1. 定位与边界

- 流程位置：文书审核后，节点 `ITC_ClientX_TypewriterInput`
- 玩法目标：以“输入准确性”影响满意度/失误，不中断主流程
- 当前 Yarn 判定：
  - `Route_QTEErrorCount == 2` -> `satisfaction-1`
  - `Route_QTEErrorCount >= 3` -> `Sign_mistake=1`

---

## 2. 面板形态与入口

- Panel：`RuneTypingPanel : UIPanel`
- UILevel：`PopUI`
- 打开命令：`<<itc_rune_typing clientId gridSize>>`
- 数据输入：
  - `gridSize`（4/5）
  - `targetSequence`
  - `runeGridLayout`
  - `tuning`

---

## 3. 交互模型

### 3.1 输入通道

- 主输入：`WASD + Confirm(Space/Enter)`
- 备选输入（WebGL焦点丢失兜底）：屏幕方向按钮 + 点击确认
- 输入缓冲：`80ms`

### 3.2 错误定义

- 光标移动到错误符文并确认 -> 记 1 次错误
- 非法输入（边界外）不计错，但给边界反馈

### 3.3 状态机

- `Countdown`
- `Playing`
- `FeedbackCorrect`
- `FeedbackError`
- `ComboResolve`
- `Completed`

---

## 4. 节奏与手感

### 4.1 单局目标时长

- 4x4：12-20s
- 5x5：18-28s

### 4.2 关键反馈时序

- 光标移动响应：`<= 50ms`
- 正确命中发光：`120ms`
- 错误闪烁：`100ms * 2`
- 完成收束动画：`700ms`（含打字机联动）

### 4.3 手感参数建议

- 光标移动补间：`OutQuad, 0.08s`
- 连续输入节拍容忍：相邻确认最短间隔 `90ms`
- 错误时轻微屏幕震动：振幅 `2-4px`, 时长 `0.09s`

---

## 5. 判定与写回

- `$Route_QTEErrorCount = errorCount`
- 不在面板中直接写 `Sign_mistake/satisfaction`
- 统一由后续判定节点（`C2_D155` 或 Command 收口）处理

---

## 6. 可配置项（SO）

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

## 7. SFX/VFX 预留点

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

## 8. UIKit/ResKit/WebGL 约束

- 网格格子复用池化，禁止每次开局动态new一批对象
- 序列提示文本使用预分配字符串模板
- WebGL 下不使用线程/Task.Run；计时全部走协程
- 面板关闭时清理输入监听，避免和后续面板抢输入

---

## 9. 独立测试（白模）

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

## 10. 与上下游接口

- 上游读 `clientId/gridSize`
- 下游输出 `Route_QTEErrorCount`
- 结束后可插入 `RuneVerify` 概率判定，或直接进 `Stamp`
