
## 这份落地里，已经明显“避开了”的点

1. **选项系统没有被他硬塞进 Line Presenter**
   他明确写了“选项不由此组件处理，保留 OptionsPresenter”。这点等于避免了我之前提到的“RunOptionsAsync 直接 NoOptionSelected 导致选项坏掉”的大坑。 

2. **跳过/继续的输入被拆成两种动作（很像我建议的状态机）**
   使用指南里把“继续键”和“跳过键”分开（Space/Enter），并且 `TALinePresenter` 暴露 `IsShowingLine` + `SkipCurrentLine()`。这基本能把“按一下既跳过又下一句”的体验问题压下去。  

3. **RunLineAsync 的主流程是正确的**
   他在 `RunLineAsync` 里用 `HurryUpToken.Register(()=>SkipTypewriter())`，并等待“打字机完成”和“NextContentToken”。主干思路没跑偏。 

---

## 但仍然“看不出是否避免”，甚至“依然很可能存在”的点

最大的不确定性来自两处：

* 他们一方面推荐 **直接在 Yarn 脚本里写 Text Animator 标签**（`<shake>`/`<waitfor>`/`<?event>` 等）。这会让很多“Yarn→TA 转换”的坑不发生，但代价是你要迁移脚本并放弃 Yarn 的一些内置能力。 
* 另一方面他们又保留了 **`YarnMarkupConverter`（可选）**，而且在方案报告里那段 converter 仍然是“无脑插 `<tag>` + `</tag>`”的写法——这会把我之前最担心的“单标签 action 被包裹化”问题重新带回来。 

所以：**如果你们完全不用 converter、脚本全迁移成 TA 标签**，那么我之前对“tag 形态”的担心大部分会被绕开；
**但如果 converter 参与了 pause/waitfor/speed/waitinput 之类 action 标签的转换**，问题仍然很可能存在。

---

# 自查问题清单（每条后面都给优化方向）

下面这份你直接丢给他，让他逐条“是否命中/怎么证明/怎么修”。

---

## 1) 是否启用了 `YarnMarkupConverter`？启用范围到底多大？

**要查什么**：项目里现在到底是“全脚本迁移成 TA 标签”，还是“仍保留 Yarn 的 `[pause]` 等，再靠 converter 转换”。
**怎么验证**：

* 全局搜索 Yarn 脚本：是否仍存在 `[` `]` 形式的标记（如 `[pause=500/]`）。
* 检查 `TALinePresenter` 是否对 `MarkupParseResult` 做过转换调用（报告里 converter 被列为可选，但没说明是否实际启用）。 
  **若命中（确实在用 converter）→建议**：把“哪些 tag 走转换、哪些必须脚本迁移”写成硬规则，不要混着用（否则后面第 4 条会炸）。

---

## 2) Converter 是否把 “单标签动作” 错转成了成对标签？

**要查什么**：方案里的 converter 明确是插入 `<tag...>` 和 `</tag>`。 
对 `<waitfor=…>`、`<waitinput>` 这种“指令式/单标签”而言，`</waitfor>` 可能是无效甚至副作用。
**怎么验证**：

* 如果 converter 映射里包含 `pause -> waitfor`（报告写过迁移目标）。 
* 运行用例：用 Yarn 标记写 pause（或走 converter 的等价标记），看实际是否暂停、是否报解析警告、是否出现奇怪的持续态。
  **若命中→建议优化方向**：
* 把映射表升级为三类：`Wrap`（成对包裹）、`Single`（单标签）、`Event`（转 `<?...>`）。
* `pause` 这类建议直接转 `<?pause=0.5>`，在 `onMessage` 里 `await`，比硬塞 `<waitfor>` 更可控。

---

## 3) `{fade}...{/fade}` 这种花括号标签，会不会和 Yarn 的插值/变量语法冲突？

你们示例里大量使用 `{fade}`。  
**要查什么**：Yarn 对 `{}` 的处理在不同版本/配置下可能用于变量/格式化；如果 Yarn 把 `{fade}` 当作插值或非法表达式，可能直接报错或被替换掉。
**怎么验证**：

* 用最小脚本跑：同一行同时包含 `{$var}`（Yarn 插值）和 `{fade}`（TA 标签），确认：

  1. Yarn 不报错；2) 最终显示仍包含 `{fade}` 给 TA 解析；3) 插值仍正确。
     **若命中→建议优化方向**：
* 尽量统一使用 TA 的 `<>` 标签体系，减少 `{}`。
* 或在 TA 侧改用 style 的 `<style=fade>` 这类等价写法（如果有），避免与 Yarn 语法重叠。

---

## 4) “混用两套标记系统”时，位置索引是否会错？

方案里写过“Yarn 和 TA 都会解析标签，建议在 Yarn 阶段完成转换再传给 TA”。 
但使用指南又建议“直接在 Yarn 脚本里写 TA 标签”。 
**要查什么**：如果你们同时满足：

* 既有 Yarn 标记（会产生 `MarkupParseResult.Attributes` 位置）
* 又在同一行里写了 TA 标签（`<shake>` 等会占字符位）
  那么 converter 的 `Position/Length` 插入点可能偏移，导致插入到 `<...>` 中间。
  **怎么验证**：
* 构造一行：前半段含 `<shake>`，中间含 Yarn 标记（如果还在用），看转换后字符串是否破坏标签结构。
  **若命中→建议优化方向**：
* 二选一：

  1. **彻底迁移**：脚本只用 TA 标签，禁用 converter；
  2. **彻底保留 Yarn 标记**：脚本里禁止手写 TA 标签，由 converter 统一生成 TA 标签。

---

## 5) Skip 时 `onTextShowed` 是否一定会触发？不触发会不会死等？

`RunLineAsync` 的骨架是：`ShowText -> 注册 HurryUp Skip -> await textShowCompletionSource -> await NextContent`。 
**要查什么**：如果某些情况下 `SkipTypewriter()` 不触发 `onTextShowed`，`textShowCompletionSource` 就可能永远不完成，整条对话卡死。
**怎么验证**：

* 播放过程中疯狂跳过（连续触发 HurryUp），以及在刚 ShowText 后立刻 Skip，确认不会卡住。
* 场景切换/对象 Disable 时，是否能安全取消等待。
  **若命中→建议优化方向**：
* 在 `HurryUpToken` 回调里不仅 `SkipTypewriter()`，还要 **兜底 `TrySetResult`**。
* 任何事件监听都用 `try/finally` 移除，防止多次注册叠加。

---

## 6) 输入层是否会出现“同一帧 Skip + Next”连发？

你们拆了 continueKey/skipKey，很好。 
**要查什么**：鼠标点击/手柄/触屏是否也走同一路径？如果点击既触发 skip 又触发 continue，仍可能“一次输入跳两步”。
**怎么验证**：

* 打字未完成时点击：应该只 Skip，不 Next。
* 打字完成后点击：应该 Next。
  **若命中→建议优化方向**：
* 输入层必须用 `TALinePresenter.IsShowingLine` 做门控：`if (IsShowingLine) Skip else Continue`。

---

## 7) 你们是否“放弃了 Yarn 的 IActionMarkupHandler”？

方案报告写了“统一用 TA 的 `<?event>` + onMessage”，并把 Yarn 的 `IActionMarkupHandler` 列为需要放弃的能力。 
**要查什么**：你现有项目里是否已经有基于 Yarn handler 的功能（比如 pause、镜头、角色动作、音效等）？
**怎么验证**：

* 搜项目：是否存在 `ActionMarkupHandler` 子类或实现。
* 如果存在，这套新方案是否仍能驱动它们？目前文档看起来是直接走 `TADialogueEvents`。 
  **若命中→建议优化方向**：
* 给 `TADialogueEvents.onCustomMessage` 加一层“兼容分发”：把 `<?event>` 映射回你们旧的 handler 调用（最起码保住历史资产）。
* 或写一个“脚本迁移规则”，明确旧 handler 全部改写为 `<?...>` 事件。

---

## 8) `<?event>` 在 Skip 时会不会被一次性触发一堆？

事件流明确依赖 `onMessage`（`<?event>`）。  
**要查什么**：SkipTypewriter 时 TA 可能会把尚未到达的事件也触发（不同配置行为不同）。这会导致“跳过一句话触发了整句所有事件”，比如音效爆炸、镜头连续抖。
**怎么验证**：

* 在一句里插多个 `<?playSound=...>`，中途 Skip，看触发次数是否符合预期。
  **若命中→建议优化方向**：
* 事件分两类：

  * “只要这句出现就必须触发”（允许 Skip 也触发）
  * “必须在播放到位置才触发”（Skip 时不触发）
* 在 `TADialogueEvents` 里加过滤策略（比如 Skip 期间屏蔽某些事件）。

---

## 9) 名字/文本拆分是否会把 TA 标签带进名字栏？

使用指南里 `TALinePresenter` 同时引用 `characterNameText` 和 `lineText`。 
**要查什么**：如果角色名来自 Yarn 的“character name + line text”结构，确保：

* 名字栏不会吃到 `<shake>` 等标签
* 文本栏不会残留 “角色:” 前缀
  **怎么验证**：
* 角色名含特殊字符、含富文本、含本地化时跑一遍。
  **若命中→建议优化方向**：
* 名字与正文的“取值来源”必须是 Yarn 已拆好的字段（不要手工 Split `:`）。

---

## 10) 多 presenter 并存时，是否会重复响应同一个 Line？

你们是把 `TALinePresenter` 加进 DialogueRunner 的 presenter 列表，同时移除原 LinePresenter。 
**要查什么**：是否还有其他 presenter（比如日志、字幕、回放系统）也会消耗同一条 line？如果有，`NextContentToken` 的等待/触发时序要非常小心。
**怎么验证**：

* 列出 DialogueRunner 的 dialoguePresenters 列表，确认只有一个 “会 WaitNext 的 LinePresenter”。
  **若命中→建议优化方向**：
* 把“展示”和“记录/旁路”拆分：旁路 presenter 不应阻塞 `NextContentToken`。

---

## 11) TimeScale/暂停时，`<waitfor>` 的计时是否符合设计？

方案报告提到“时间缩放可能不同，需要确保同步”。 
**要查什么**：游戏暂停（Time.timeScale=0）时 `<waitfor=1>` 是不是还在走？你要的是“真实时间”等待还是“游戏时间”等待？
**怎么验证**：

* 暂停游戏后触发 `<waitfor>`，观察是否继续推进。
  **若命中→建议优化方向**：
* 在 TA 或事件层统一规定：对话等待走 unscaled 还是 scaled，并写成全局设置。

---

## 12) 文档里“验证清单”覆盖太少，缺关键回归项

他们的验证清单只覆盖 shake/wave/waitfor/fade/skip/continue。 
**要查什么**：缺少这些会在真实项目爆炸的用例：

* `<?event>` + Skip 的事件语义（第 8 条）
* `{}` 与 Yarn 插值冲突（第 3 条）
* 场景切换/Disable 时等待不会卡死（第 5 条）
* 多语言本地化、长文本、富文本嵌套闭合正确
  **建议优化方向**：
* 把上述 4 类测试补进最小回归场景，做成“一键跑通”的 test yarn。
