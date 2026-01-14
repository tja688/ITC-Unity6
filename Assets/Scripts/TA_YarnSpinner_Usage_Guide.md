# Text Animator + Yarn Spinner 使用指南

> **适用版本**: Text Animator 3.1+ / Yarn Spinner Unity 3.x

---

## 快速开始

### 1. 场景设置

```
场景结构示例：
├── DialogueSystem
│   ├── DialogueRunner (Yarn Spinner)
│   └── DialogueCanvas
│       ├── DialoguePanel
│       │   ├── CharacterNameText (TMP_Text)
│       │   └── LineText (TMP_Text + TextAnimator_TMP + TypewriterComponent)
│       └── ContinueButton (可选)
```

### 2. 组件配置

#### 步骤1：设置 Text Animator
1. 在对话文本 `TMP_Text` 上添加 `TextAnimator_TMP` 组件
2. 添加 [TypewriterComponent](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Plugins/Febucci/Text%20Animator%20for%20Unity/Scripts/Runtime/Components/Typewriter/_Core/TypewriterComponent.cs#30-469) 组件
3. 配置 Typewriter 的 Timings（选择 `TypingDelaysByCharacter` 或 `ByWord`）

#### 步骤2：替换 LinePresenter
1. 移除默认的 [LinePresenter](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Library/PackageCache/dev.yarnspinner.unity@040b0f4542b5/Runtime/Views/LinePresenter.cs#26-422)
2. 添加 [TALinePresenter](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TALinePresenter.cs#21-253) 组件
3. 配置引用：
   - `textAnimator`: 指向 TextAnimator_TMP
   - `typewriter`: 指向 TypewriterComponent
   - `lineText`: 指向对话 TMP_Text
   - `characterNameText`: 指向角色名 TMP_Text

#### 步骤3：配置 DialogueRunner
1. 在 `DialoguePresenters` 列表中添加 [TALinePresenter](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TALinePresenter.cs#21-253)
2. 移除原有的 LinePresenter

---

## Yarn 脚本编写

### 使用 Text Animator 标签

直接在 Yarn 脚本中使用 Text Animator 的富文本标签：

```yarn
title: Start
---
// 持续效果
角色: 我感觉<shake>非常紧张</shake>...

// 入场效果
角色: {fade}这段文字会淡入显示{/fade}

// 打字机控制
角色: 等一下...<waitfor=1.5>好了！

// 等待输入
角色: 准备好了吗？<waitinput>

// 速度控制
角色: <speed=0.5>慢慢说...</speed><speed=2>快快说！</speed>

// 组合效果
角色: <wave><rainbow>彩虹波浪！</rainbow></wave>
===
```

### 可用效果标签

| 标签 | 效果 | 示例 |
|------|------|------|
| `<shake>` | 抖动 | `<shake>颤抖</shake>` |
| `<wave>` | 波浪 | `<wave>起伏</wave>` |
| `<wiggle>` | 摆动 | `<wiggle>晃动</wiggle>` |
| `<bounce>` | 弹跳 | `<bounce>跳动</bounce>` |
| `<rainb>` | 彩虹色 | `<rainb>彩虹</rainb>` |
| `<rot>` | 旋转 | `<rot>旋转</rot>` |
| `<fade>` | 淡入淡出 | `<fade>渐变</fade>` |
| `<incr>` | 放大缩小 | `<incr>变大</incr>` |
| `<pend>` | 钟摆 | `<pend>摆动</pend>` |

### 可用动作标签

| 标签 | 功能 | 示例 |
|------|------|------|
| `<waitfor=秒>` | 暂停指定秒数 | `<waitfor=1.5>` |
| `<waitinput>` | 等待玩家输入 | `<waitinput>` |
| `<speed=倍率>` | 改变打字速度 | `<speed=2>` |

### 自定义事件

使用 `<?>` 触发自定义事件：

```yarn
角色: 看这个！<?playSound=ding>
角色: 要开始了<?cameraShake>准备好！
```

在 [TADialogueEvents](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TADialogueEvents.cs#18-189) 的 `onCustomMessage` 中监听。

---

## 重要设计规范 ⚠️

### 1. YarnMarkupConverter 默认不启用

当前方案采用**全迁移策略**：直接在 Yarn 脚本中写 Text Animator 标签，**不使用** `YarnMarkupConverter`。

- `YarnMarkupConverter.cs` 作为可选扩展保留，但默认不启用
- 如需启用 Converter，必须**二选一**：
  - 脚本只用 TA 标签，禁用 Converter
  - 脚本只用 Yarn 标记，由 Converter 统一生成 TA 标签
- **禁止混用两种标记系统**，否则会导致位置索引错误

### 2. `{}` 花括号使用规范

**对话脚本中 `{}` 只允许 Yarn 插值，TA 效果一律使用 `<>` 标签**

```yarn
// ✅ 正确写法
角色: 你有{$count}个道具，<fade>看这里</fade>

// ❌ 错误写法 - 禁止使用 {} 做入场效果
角色: 你有{$count}个道具，{fade}看这里{/fade}
```

**原因**：`{}` 与 Yarn 的变量插值语法可能冲突，统一使用 `<>` 可避免潜在问题。

### 3. Skip 事件过滤配置

Skip（快进）时可能会批量触发事件（如音效、镜头抖动），可在 `TADialogueEvents` 中配置过滤策略：

| 配置项 | 说明 |
|--------|------|
| `blockedEventsOnSkip` | Skip 期间要屏蔽的事件（如 playsound, shake） |
| `allowedEventsOnSkip` | Skip 期间允许的事件（如 emotion, setvar） |
| `skipEventFilterMode` | 过滤模式：Blacklist/Whitelist/BlockAll |

### 4. 时间行为

- 对话淡入淡出使用 `Time.deltaTime`（受 `TimeScale` 影响）
- `<waitfor>` 行为取决于 Text Animator 配置
- 如需暂停时对话继续推进，需额外配置

---

## 组件 API

### TALinePresenter

| 属性/方法 | 说明 |
|----------|------|
| `IsShowingLine` | 是否正在显示文本 |
| `IsSkipping` | 是否正在跳过（Skip 期间为 true） |
| [SkipCurrentLine()](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TALinePresenter.cs#235-245) | 跳过当前打字机效果 |

### DialogueContinueHandler

| 设置 | 说明 |
|------|------|
| `continueKey` | 继续对话按键（默认 Space） |
| `skipKey` | 跳过打字机按键（默认 Enter） |
| `useMouseClick` | 启用鼠标点击继续 |

### TADialogueEvents

| 事件 | 说明 |
|------|------|
| `onCharacterShown` | 每个字符显示时触发 |
| `onTypewriterComplete` | 打字机完成时触发 |
| `onCustomMessage` | 自定义消息触发 |

---

## 常见问题

### Q: 文字不显示动画效果
**A**: 确保：
1. `TextAnimator_TMP` 已添加到 TMP_Text
2. 标签格式正确（使用 `<>` 而非 `[]`）
3. 运行时检查控制台错误

### Q: 打字机不工作
**A**: 检查：
1. [TypewriterComponent](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Plugins/Febucci/Text%20Animator%20for%20Unity/Scripts/Runtime/Components/Typewriter/_Core/TypewriterComponent.cs#30-469) 已配置 Timings
2. [TALinePresenter](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TALinePresenter.cs#21-253) 引用正确

### Q: 如何添加打字音效
**A**: 配置 [TADialogueEvents](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TADialogueEvents.cs#18-189)：
1. 添加 `AudioSource`
2. 设置 `typingSounds` 数组
3. 启用 `playTypingSounds`

### Q: Skip 时音效/镜头效果爆炸
**A**: 在 `TADialogueEvents` 组件中配置 `blockedEventsOnSkip`，将会叠加的事件（如 playsound, shake）加入屏蔽列表。

### Q: 对话卡死不响应
**A**: 检查：
1. 是否在打字过程中禁用了对话对象
2. 是否有其他组件阻止了 NextContentToken
3. 查看控制台日志确认事件触发情况

---

## 文件清单

| 文件 | 说明 |
|------|------|
| [TALinePresenter.cs](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TALinePresenter.cs) | 核心 DialoguePresenter |
| [YarnMarkupConverter.cs](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/YarnMarkupConverter.cs) | 标记转换器（可选，默认不启用） |
| [DialogueContinueHandler.cs](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/DialogueContinueHandler.cs) | 输入处理 |
| [TADialogueEvents.cs](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TADialogueEvents.cs) | 事件桥接（含 Skip 事件过滤） |
| [TestDialogue.yarn](file:///C:/Users/jinji/Documents/GitHub/ITC-Unity6/Assets/Scripts/Dialogue/TestDialogue.yarn) | 回归测试脚本 |
