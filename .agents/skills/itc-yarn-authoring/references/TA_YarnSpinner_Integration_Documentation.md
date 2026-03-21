# ITC Yarn 文案编写规范（Text Animator 集成版｜唯一事实来源）

> 适用版本：Text Animator 3.1+ / Yarn Spinner Unity 3.x
> 本文是项目内 .yarn 文案的唯一规范，覆盖 Yarn 原生语法 + 本项目 Text Animator 集成行为。

---

## 0. 快速总则（必须遵守）

1. `{}` 只用于 Yarn 插值/替换
   - 允许：`{$var}`、`{0}`、`{1}` 等
   - **Text Animator Appearance/Disappearance 改用 `|` 符号**（见下文）
2. Text Animator Behavior 标签统一使用 `<>`
   - 例：`<shake>...</shake>`、`<waitfor=1.5>`、`<?playSound=ding>`
3. Text Animator Appearance/Disappearance 标签使用 `|`
   - 例：`|fade|`、`|writer|`
   - 解决了与 Yarn `{}` 的冲突，现可自由使用退场效果
4. Yarn 的 `[]` 标记体系默认禁止
   - 当前项目默认不启用 `YarnMarkupConverter`
5. 不混用两套标记系统
   - 若启用 `YarnMarkupConverter`：脚本只能写 `[]` 标签，禁止 `<>`
6. 事件只用 `<?event>` 触发
   - 事件只在 Typewriter 启用时触发

---

## 1. Yarn 文件结构与基本语法

### 1.1 节点结构

```yarn
title: NodeName
# tags: tag1 tag2 (可选)
---
内容...
===
```

- `title`：必填，唯一节点名。
- `tags`：可选，空格分隔（用于分析/定位，不影响显示）。
- `---` 与 `===`：节点头与正文的硬分隔，必须存在。

### 1.2 注释

```yarn
// 单行注释
```

### 1.3 变量与表达式

- 变量以 `$` 开头：`$flag`、`$count`。
- 声明/赋值：

```yarn
<<declare $count = 0>>
<<set $count = 3>>
```

- 条件分支：

```yarn
<<if $count > 0>>
    文本...
<<elseif $count == 0>>
    文本...
<<else>>
    文本...
<<endif>>
```

### 1.4 内联插值（与 `{}` 冲突规则）

- 仅用于 Yarn 插值/替换：
  - `{$var}`（变量或表达式）
  - `{0}`、`{1}`（本地化替换占位）
- **任何 Text Animator 标签不得使用 `{}`。**

---

## 2. 文本行规范（角色名、元数据、换行）

### 2.1 角色名行

```yarn
角色名: 这里是对话内容
```

- Yarn 会将 `角色名:` 解析为 `character` 标记。
- 项目中由 `TALinePresenter` 读取 `line.CharacterName`，并在 UI 上单独显示角色名。
- 没有角色名时直接写文本行。

### 2.2 行内元数据（可选）

```yarn
这是一行文本 #tag #mood
```

- `#tag` 会被作为元数据保存，不会显示在文本中。
- 当前 `TALinePresenter` 不使用该元数据，但可用于后续系统拓展。

### 2.3 换行

> [!IMPORTANT]
> Yarn Spinner **不支持** `\n` 转义换行。官方使用 `[br /]` 标记，但当前项目禁用了 `[]` 标记体系。

**推荐方案**：将需要换行的内容拆分为多个对话行（推荐）

```yarn
角色: 第一行
角色: 第二行
```

**备用方案**：如果确实需要同一 UI 框内换行，可在代码层（如 `TALinePresenter`）统一替换特定占位符为 `\n`，例如约定 `<br>` → 换行。

---

## 3. 选项与流程控制

### 3.1 快捷选项（推荐）

```yarn
-> 选项A
    选项A的内容
-> 选项B
    选项B的内容
```

- 子内容必须缩进（建议 4 空格）。

### 3.2 选项条件（可用性控制）

```yarn
-> 只有满足条件才可选 <<if $flag>>
```

- 当条件不满足时，`OptionsPresenter` 会将该选项标记为不可用。
- 是否显示不可用选项由 `OptionsPresenter.showUnavailableOptions` 控制。

### 3.3 跳转与结束

```yarn
<<jump SomeNode>>
<<stop>>
```

### 3.4 自定义命令/函数（需注册）

```yarn
<<commandName param1 param2>>
文本中插值：{$funcName($count)}
```

- 命令/函数必须在代码侧注册，否则运行时报错。
- 当前项目未内置固定命令列表，使用前必须确认注册并同步更新本文。

---

## 4. Text Animator 标签体系（项目默认启用）

### 4.1 动作类（Typewriter Actions）

> 动作标签只在 Typewriter 启用时有效。

| 标签 | 参数 | 说明 | 示例 |
| --- | --- | --- | --- |
| `waitfor` | 秒数（float） | 暂停指定秒数 | `<waitfor=0.5>` |
| `waitinput` | 无 | 等待玩家输入 | `<waitinput>` |
| `speed` | 倍率（float） | 修改打字速度 | `<speed=0.5>` |

规则：
- 动作标签 **大小写不敏感**。
- 数字小数使用 `.`，不可使用 `,`。

### 4.2 视觉效果（Effects，来自当前全局数据库）

> 以下为项目当前 Text Animator 数据库可用的效果标签（持久效果）。

| 标签 | 说明 | 备注 |
| --- | --- | --- |
| `shake` | 位置抖动 | 常用于惊讶/紧张 |
| `wiggle` | 随机摆动 | 轻微抖动 |
| `wave` | 波浪形位移 | 常用强调 |
| `bounce` | 弹跳 | 上下弹跳 |
| `dangle` | 垂坠 | 摆动感 |
| `swing` | 摇摆 | 摇摆效果 |
| `rot` | 旋转 | 字符旋转 |
| `incr` | 放大/缩放 | 逐字放大 |
| `expand` | 扩张 | 扩张效果 |
| `slideh` | 水平滑动 | 左右滑动 |
| `slidev` | 垂直滑动 | 上下滑动 |
| `fade` | 透明渐变 | 透明度变化 |
| `rainb` | 彩虹色 | 颜色循环 |

> 效果标签需要成对闭合：`<tag>...</tag>` 或 `</>`。

### 4.3 事件标签（Messages）

```yarn
<?eventName>
<?eventName=param1,param2>
```

- 事件标签 **大小写敏感**。
- 事件仅在 Typewriter 启用时触发。
- 事件由 `TADialogueEvents.onCustomMessage` 接收。

当前默认过滤（Skip 期间可能被屏蔽）：
- `playsound`
- `camerashake`
- `shake`
- `sound`

### 4.4 修饰符（Modifiers）

- 格式：`<tag key=value>` 或 `<tag key*multiplier>`
- 常见示例：
  - `<wave s=2>`（速度加倍）
  - `<shake a=2>`（幅度加倍）
- 修饰符是否生效取决于具体效果脚本与数据库配置。
- 同一标签内重复参数时，最后一个生效。

### 4.5 进场与退场效果（Appearances / Disappearances）

> 项目特殊配置：使用 `|` 作为解析符号，避免与 Yarn `{}` 冲突。

- **进场 (Appearances)**: 控制文字如何出现。
  - 语法：`|tag|` 或 `|tag=value|`
  - 示例：`|fade|` (淡入), `|typewriter|` (打字机), `|size|` (缩放进场)
  - 通常放在文本开头：`|fade|这是进场文本`

- **退场 (Disappearances)**: 控制文字如何消失。
  - 语法：`|tag|` (依据设置可能需包含 `#`，例如 `|#fade|` 或 `|fade s=0.5 #|`)
  - 注意：退场效果需配合 Text Animator 的 Disappearing 逻辑使用。

---

## 5. Yarn 与 Text Animator 冲突说明（已解决）

1. Yarn 的 `[]` 标记会被解析并从文本中移除（保持原状）。
2. `TALinePresenter` 使用 `line.TextWithoutCharacterName.Text`，导致 `[]` 标记无法传递给 Text Animator。
3. **Appearance/Disappearance 冲突已解决**：
   - 原冲突点：Text Animator 默认使用 `{}`，被 Yarn 占用。
   - **解决方案**：项目已将 Text Animator 的 Appearance/Disappearance 解析符号修改为 `|`（竖线）。
   - **结果**：现在可以自由使用 Text Animator 的进场/退场效果，无需“放弃能力”。
   - 语法示例：`|fade|` (进场), `|size|` (进场) 等。


---

## 6. 事件系统与 Skip 行为（项目实现）

### 6.1 事件桥接

`TADialogueEvents` 监听：
- Typewriter：`onCharacterVisible`、`onTextShowed`、`onMessage`
- DialogueRunner：`onDialogueStart`、`onDialogueComplete`、`onNodeStart`、`onNodeComplete`

### 6.2 Skip 过滤策略

- `blockedEventsOnSkip`：默认屏蔽 `playsound`、`camerashake` 等
- `allowedEventsOnSkip`：允许 `emotion`、`expression`、`setvar` 等
- `skipEventFilterMode`：Blacklist / Whitelist / BlockAll

---

## 7. YarnMarkupConverter（备用方案，不默认启用）

> 仅作为应急方案，正常情况下 **禁止使用**。

路径：`Assets/Scripts/Dialogue/YarnMarkupConverter.cs`

### 7.1 作用

将 Yarn `[]` 标记映射为 Text Animator `<>` 或事件 `<?>`。

### 7.2 映射表（当前默认）

| Yarn 标记 | Text Animator 标签 | 类型 | 备注 |
| --- | --- | --- | --- |
| `[pause=500]` | `<waitfor=0.5>` | 单标签 | ms -> s |
| `[waitinput]` | `<waitinput>` | 单标签 | 无参数 |
| `[shake]...[/shake]` | `<shake>...</shake>` | 包裹型 | 效果 |
| `[wave]...[/wave]` | `<wave>...</wave>` | 包裹型 | 效果 |
| `[wobble]...[/wobble]` | `<wiggle>...</wiggle>` | 包裹型 | 别名 |
| `[bounce]...[/bounce]` | `<bounce>...</bounce>` | 包裹型 | 效果 |
| `[rainbow]...[/rainbow]` | `<rainb>...</rainb>` | 包裹型 | 别名 |
| `[playsound=ding]` | `<?playsound=ding>` | 事件 | 事件标签 |
| `[camerashake]` | `<?camerashake>` | 事件 | 事件标签 |

**启用 Converter 时：脚本中只能使用 `[]`，禁止 `<>`。**

---

## 8. 运行时流程（TALinePresenter 关键逻辑）

1. 取 `line.TextWithoutCharacterName.Text`（Yarn 标记被移除，`<>` 保留）。
2. `TextAnimator.SetText(displayText, true)`（先隐藏文本）。
3. UI 淡入 → `Typewriter.StartShowingText(true)`。
4. 若文本无可见字符，直接完成（防止死等）。
5. Skip 触发 `typewriter.SkipTypewriter()`。
6. 文本完成后，等待用户继续或自动前进。

---

## 9. 测试用例

- 测试脚本：`TestDialogue.yarn`
- 覆盖项：
  - 变量/条件/跳转
  - 选项与可用性
  - Text Animator 效果、动作、事件、修饰符
  - 空文本标签行、嵌套标签、速度边界

---

## 10. 快速参考（最常用写法）

```yarn
角色: 你有{$count}个道具，<shake>注意这里</shake>
角色: <waitfor=0.5>暂停后继续
角色: <speed=0.5>慢 <speed=2>快
角色: <?playsound=ding>触发音效
角色: |fade|这是淡入进场的文本
```

---

## 11. 相关实现文件

- `Assets/Scripts/Dialogue/TALinePresenter.cs`
- `Assets/Scripts/Dialogue/TADialogueEvents.cs`
- `Assets/Scripts/Dialogue/DialogueContinueHandler.cs`
- `Assets/Scripts/Dialogue/YarnMarkupConverter.cs`
- `Assets/Projects/Test/TestDialogue.yarn`
- `Assets/Plugins/Febucci/docs.febucci.com.md`
- `Library/PackageCache/dev.yarnspinner.unity@040b0f4542b5/Runtime/LineProviders/LocalizedLine.cs`
