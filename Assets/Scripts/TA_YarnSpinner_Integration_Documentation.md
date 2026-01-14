# Text Animator + Yarn Spinner 集成综合文档（唯一事实来源）

> 适用版本：Text Animator 3.1+ / Yarn Spinner Unity 3.x  
> 本文为集成后的**唯一事实文档**。所有 QA 与后续修订以此为基准。

---

## 1. 统一结论与规范（必须遵守）

1. **`{}` 只用于 Yarn 插值/替换**  
   - 允许：`{$var}`、`{0}`  
   - 禁止：`{fade}...{/fade}` 等 Text Animator 效果
2. **Text Animator 标签统一使用 `<>`**  
   - 例：`<shake>...</shake>`、`<waitfor=1.5>`、`<?playSound=ding>`
3. **不混用两套标记系统**  
   - 若启用 `YarnMarkupConverter`：Yarn 脚本只写 `[]` 标记  
   - 默认策略：**不启用 Converter**，脚本直接写 TA 标签
4. **事件触发以 `<?event>` 形式**  
   - 事件仅在 Typewriter 启用时触发

---

## 2. 架构概览（当前实际）

```
DialogueRunner
  ├─ TALinePresenter (替代 Yarn LinePresenter)
  │    ├─ TextAnimator_TMP (文本与顶点动画)
  │    └─ TypewriterComponent (打字机显示与事件)
  ├─ OptionsPresenter (选项显示，原生保留)
  └─ TADialogueEvents (事件桥接与 Skip 过滤)
```

关键组件与路径：
- `Assets/Scripts/Dialogue/TALinePresenter.cs`
- `Assets/Scripts/Dialogue/TADialogueEvents.cs`
- `Assets/Scripts/Dialogue/DialogueContinueHandler.cs`
- `Assets/Scripts/Dialogue/YarnMarkupConverter.cs`（可选，不默认启用）

---

## 3. 运行流程（RunLineAsync 实际逻辑）

1. `DialogueRunner` 调用 `TALinePresenter.RunLineAsync(...)`
2. 处理角色名与 `line.TextWithoutCharacterName.Text`
3. `textAnimator.SetText(displayText, true)`（隐藏文本，避免淡入时残留上一行）
4. `CanvasGroup` 淡入（Time.deltaTime）
5. `typewriter.StartShowingText(true)`
6. `HurryUpToken` 触发 `SkipTypewriter` 并兜底完成
7. 等待 `onTextShowed` 完成后进入“等待继续阶段”
8. 等待 `NextContentToken` 或自动前进
9. 淡出并结束当前行

> 注：`line.TextWithoutCharacterName.Text` 会**移除 Yarn `[]` 标记**，但会保留 `<>` 文本，因此 TA 标签必须使用 `<>` 才能到达 Text Animator。

---

## 4. 输入与推进逻辑

### 4.1 Continue 行为（当前实现）

`DialogueContinueHandler.RequestContinue()` 逻辑：

- **行仍在打字** → `SkipCurrentLine()`  
- **行已完整显示** → `DialogueRunner.RequestNextLine()`  
- **无活动行** → `DialogueRunner.RequestNextLine()`

相关文件：
- `Assets/Scripts/Dialogue/DialogueContinueHandler.cs`
- `Assets/Scripts/Dialogue/TALinePresenter.cs`

### 4.2 Skip 行为

`RequestSkip()` 调用 `DialogueRunner.RequestHurryUpLine()`，触发 `LineCancellationToken.HurryUpToken`，由 `TALinePresenter` 执行 `SkipTypewriter()`。

---

## 5. 事件系统与 Skip 过滤

### 5.1 事件来源

Text Animator Typewriter 提供事件：
- `onCharacterVisible`
- `onTextShowed`
- `onMessage`（`<?event>`）

事件只在 Typewriter 启用时触发。

### 5.2 事件桥接

`TADialogueEvents` 订阅 Typewriter 与 DialogueRunner 事件，并对外提供 UnityEvent：
- `onCharacterShown`
- `onTypewriterComplete`
- `onCustomMessage`
- `onDialogueStarted` / `onDialogueEnded`
- `onNodeStarted` / `onNodeEnded`

### 5.3 Skip 过滤策略

`TADialogueEvents` 支持 Skip 过滤，避免快进时事件爆炸：

- `blockedEventsOnSkip`：Skip 期间屏蔽事件
- `allowedEventsOnSkip`：Skip 期间允许事件
- `skipEventFilterMode`：Blacklist / Whitelist / BlockAll

> Text Animator 本身支持 `triggerEventsOnSkip` 设置（会一次性触发剩余事件）。若启用，需配合上述过滤避免逻辑爆炸。

---

## 6. 文本标记规范（Yarn 脚本）

### 6.1 示例（正确）

```yarn
title: Demo
---
角色: 你有{$count}个道具，<fade>看这里</fade>
角色: <shake>抖动</shake> + <wave>波浪</wave>
角色: 暂停<waitfor=1.5>继续
角色: 请点击继续...<waitinput>感谢点击
角色: <speed=0.5>慢<speed=2>快
角色: <rainb>彩虹</rainb>
角色: <?playSound=ding>触发事件
===
```

### 6.2 示例（禁止）

```yarn
角色: {fade}错误用法{/fade}
```

### 6.3 常用标签（来自 Text Animator）

**动作类**
- `<waitfor=秒>`：暂停指定秒数
- `<waitinput>`：等待输入
- `<speed=倍率>`：打字速度乘数
  - 动作标签为单标签，不需要闭合

**效果类（依赖当前数据库配置）**
- `<shake>` `<wave>` `<wiggle>` `<bounce>` `<fade>` `<rainb>`

> 效果标签是否可用取决于 Text Animator 的数据库配置；若新增或重命名，请同步更新本文与脚本示例。

---

## 7. YarnMarkupConverter（可选，不默认启用）

### 7.1 功能

将 Yarn `[]` 标记转换为 Text Animator `<>` 或事件 `<?>`：

- `[pause=500]` → `<waitfor=0.5>`
- `[waitinput]` → `<waitinput>`
- `[playsound=ding]` → `<?playsound=ding>`

### 7.2 使用约束

**启用 Converter 时，脚本中只允许 `[]` 标记，不允许 `<>` 标签。**  
混用会导致索引错位，插入位置错误。

---

## 8. QA 测试与脚本路径

统一使用：

- `Assets/Projects/Test/TestDialogue.yarn`

脚本包含：
1. 基础效果测试
2. 回归测试（Skip、事件、长文本、嵌套闭合）
3. 边界测试（特殊字符、速度边界、占位符）

---

## 9. 测试场景配置（SampleScene）

- 场景：`Assets/Scenes/SampleScene.unity`
- `DialogueSystem/DialogueCanvas/DialoguePanel`
  - `TALinePresenter`
    - `showCharacterName = 1`
    - `useFadeEffect = 1`
    - `fadeInDuration = 0.25`
    - `fadeOutDuration = 0.15`
    - `autoAdvance = 0`
    - `autoAdvanceDelay = 2`
  - `DialogueContinueHandler`
    - `useMouseClick = 1`
    - `useKeyboard = 1`
    - `continueKey = 1`（InputSystem Key 枚举序列化值）
    - `skipKey = 2`（InputSystem Key 枚举序列化值）
    - `continueButton = ContinueButton`
    - `skipButton = SkipButton`
- `DialogueSystem/DialogueCanvas/DialoguePanel/LineText/Text (TMP)`
  - `TextMeshProUGUI` + `TextAnimator_TMP` + `TypewriterComponent`
  - `TypewriterComponent.timingsScriptableBase = Assets/Projects/Test/New My Custom Typewriter Speed.asset`
  - `TypewriterComponent.localSettings`
    - `useTypeWriter = 1`
    - `startTypewriterMode = 0`
    - `hideAppearancesOnSkip = 0`
    - `hideDisappearancesOnSkip = 0`
    - `triggerEventsOnSkip = 1`
    - `resetTypingSpeedAtStartup = 1`
- `TADialogueEvents`：未挂载（Skip 事件过滤不生效）

---

## 10. 已知限制与注意事项

1. `TALinePresenter` 不处理选项，选项由 `OptionsPresenter` 负责。
2. Yarn 的 `[]` 标记在当前默认策略下不会生效（未启用 Converter）。
3. `CanvasGroup` 淡入淡出基于 `Time.deltaTime`，时间缩放会影响 UI 过渡。

---

## 11. 文件清单（当前核心）

- `Assets/Scripts/Dialogue/TALinePresenter.cs`
- `Assets/Scripts/Dialogue/TADialogueEvents.cs`
- `Assets/Scripts/Dialogue/DialogueContinueHandler.cs`
- `Assets/Scripts/Dialogue/YarnMarkupConverter.cs`
- `Assets/Projects/Test/TestDialogue.yarn`
- `Assets/Plugins/Febucci/docs.febucci.com.md`
- `Library/PackageCache/dev.yarnspinner.unity@040b0f4542b5/Runtime/Views/DialoguePresenterBase.cs`
