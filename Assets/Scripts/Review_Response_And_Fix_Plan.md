# 评审意见落地情况汇报与修改计划

> **日期**: 2026-01-14
> **状态**: 待确认修改方案

---

## 汇总概览

| # | 问题 | 状态 | 风险等级 |
|---|------|------|----------|
| 1 | YarnMarkupConverter 启用范围 | ⚠️ 需明确 | 中 |
| 2 | 单标签动作被错转成成对标签 | ⚠️ 设计缺陷 | 高 |
| 3 | `{}` 与 Yarn 插值冲突 | ⚠️ 待验证 | 中 |
| 4 | 混用两套标记系统位置偏移 | ⚠️ 待验证 | 高 |
| 5 | Skip 时 onTextShowed 不触发会死等 | ❌ **确认命中** | 高 |
| 6 | 同一帧 Skip+Next 连发 | ✅ 已规避 | 低 |
| 7 | 放弃 Yarn IActionMarkupHandler | ⚠️ 需确认历史资产 | 中 |
| 8 | Skip 时事件一次性触发一堆 | ⚠️ 待验证 | 中 |
| 9 | 名字/文本拆分带入标签 | ✅ 已规避 | 低 |
| 10 | 多 presenter 重复响应 | ⚠️ 需确认配置 | 中 |
| 11 | TimeScale 暂停时计时行为 | ⚠️ 待验证 | 低 |
| 12 | 验证清单覆盖不足 | ❌ **确认命中** | 高 |

---

## 逐条分析

### 1) YarnMarkupConverter 启用范围

**当前状态**: ⚠️ 需明确

**实际情况**:
- `YarnMarkupConverter.cs` 已创建但**未被任何代码调用**
- `TALinePresenter.RunLineAsync()` 直接使用 `line.TextWithoutCharacterName.Text`，未经过 converter
- 使用指南推荐"直接在 Yarn 脚本写 TA 标签"

**结论**: Converter 目前是**死代码**，实际采用"全迁移"策略

**建议修改**:
1. 明确文档：Converter 为可选扩展，默认不启用
2. 或删除 Converter 以避免混淆
3. 若保留，需在文档中限定使用场景

---

### 2) Converter 是否把单标签动作错转成成对标签

**当前状态**: ⚠️ 设计缺陷（但未实际启用）

**代码证据** (`YarnMarkupConverter.cs:163-174`):
```csharp
private void InsertTag(...) {
    result.Insert(endPos, $"</{tagName}>");  // ❌ 总是插入闭合标签
    result.Insert(attr.Position, startTag);
}
```

**问题**: `<waitfor>` 这类单标签会被转成 `<waitfor=0.5></waitfor>`，可能导致：
- TA 解析警告
- 持续态异常

**建议修改** (若启用 Converter):
```csharp
[System.Serializable]
public class TagMapping {
    public string yarnTag;
    public string taTag;
    public TagType type; // 新增: Wrap/Single/Event
}

public enum TagType { Wrap, Single, Event }
```

- `Wrap`: 成对包裹 `<tag>...</tag>`
- `Single`: 单标签 `<tag=value>`
- `Event`: 转为事件 `<?tag=value>`

---

### 3) `{fade}` 与 Yarn 插值冲突

**当前状态**: ⚠️ 待验证

**风险点**:
- Yarn 使用 `{$variable}` 做插值
- TA 使用 `{fade}` 做入场效果
- 同一行混用可能冲突

**验证方案**:
```yarn
Test: 你有{$count}个道具，{fade}看这里{/fade}
```

**建议修改**:
1. 验证后确认是否冲突
2. 若冲突，文档明确：
   - 禁用 `{}` 入场效果
   - 改用 `<fade>` 持续效果替代

---

### 4) 混用两套标记系统时位置索引错误

**当前状态**: ⚠️ 待验证（但当前不会触发）

**分析**:
- 当前 `TALinePresenter` **不调用 Converter**
- 直接传递 `Text` 给 TA，Yarn 的 `MarkupAttribute` 被忽略
- 因此位置偏移问题**当前不会发生**

**如果启用 Converter 会命中**:
```
原文: "Hello <shake>world</shake>"
Yarn 标记位置基于"Hello world"（无TA标签）
Converter 插入时位置会错
```

**建议修改**:
- 维持当前策略：脚本只用 TA 标签，禁用 Converter
- 或：Converter 只处理**纯 Yarn 标记的脚本**

---

### 5) Skip 时 onTextShowed 不触发会死等

**当前状态**: ❌ **确认命中**

**代码证据** (`TALinePresenter.cs:136-145`):
```csharp
using var hurryUpRegistration = token.HurryUpToken.Register(() => {
    if (isShowingLine && typewriter != null) {
        typewriter.SkipTypewriter();  // 只调用 Skip
    }
});

await textShowCompletionSource.Task;  // ❌ 无兜底，可能永远等待
```

**风险场景**:
- TA 的 `SkipTypewriter()` 在某些边界情况可能不触发 `onTextShowed`
- 场景切换/对象 Disable 时等待可能不会取消

**建议修改**:
```csharp
using var hurryUpRegistration = token.HurryUpToken.Register(() => {
    if (isShowingLine && typewriter != null) {
        typewriter.SkipTypewriter();
        // 兜底：确保 completion 被触发
        textShowCompletionSource.TrySetResult(true);
    }
});

// 添加 NextContentToken 取消支持
using var cancelRegistration = token.NextContentToken.Register(() => {
    textShowCompletionSource.TrySetResult(true);
});
```

---

### 6) 输入层同一帧 Skip+Next 连发

**当前状态**: ✅ 已规避

**代码证据** (`DialogueContinueHandler.cs:94-107`):
```csharp
public void RequestContinue() {
    if (linePresenter != null && linePresenter.IsShowingLine) {
        linePresenter.SkipCurrentLine();  // 正在显示 → 只跳过
    } else {
        dialogueRunner.RequestNextLine();  // 显示完成 → 继续
    }
}
```

**分析**: 鼠标点击走 `RequestContinue()`，已用 `IsShowingLine` 做门控

**但有瑕疵**: 
- 键盘 `continueKey` 也走 `RequestContinue()`（正确）
- 键盘 `skipKey` 直接 `RequestHurryUpLine()`（可能与点击冲突）

**建议修改**: 无，当前逻辑基本正确

---

### 7) 是否放弃了 Yarn 的 IActionMarkupHandler

**当前状态**: ⚠️ 需确认历史资产

**当前实现**:
- `TALinePresenter` 完全绕过 Yarn 的 `IActionMarkupHandler`
- 事件转由 `TADialogueEvents.onMessage` 处理

**需要确认**:
- 项目中是否已有基于 `ActionMarkupHandler` 的实现？
- 例如：`PauseEventProcessor`、自定义镜头控制等

**验证命令**:
```bash
grep -r "ActionMarkupHandler" Assets/Scripts/
grep -r "IActionMarkupHandler" Assets/Scripts/
```

**建议修改** (若有历史资产):
在 `TADialogueEvents` 中添加兼容层：
```csharp
// 映射 TA 事件到旧 handler
switch (marker.name) {
    case "pause": legacyPauseHandler?.OnPause(marker); break;
    // ...
}
```

---

### 8) Skip 时事件一次性触发一堆

**当前状态**: ⚠️ 待验证

**风险点**:
- TA 的 `SkipTypewriter()` 可能触发所有未到达的 `<?event>`
- 导致音效爆炸、镜头连续抖动

**验证方案**:
```yarn
Test: 开始<?sound1>中间<?sound2>结束<?sound3>
// 中途 Skip，观察触发次数
```

**建议修改** (若命中):
```csharp
private bool isSkipping = false;

private void OnMessage(EventMarker marker) {
    // Skip 期间可选屏蔽某些事件
    if (isSkipping && IsBlockedDuringSkip(marker.name)) return;
    
    onCustomMessage?.Invoke(marker.name);
}
```

---

### 9) 名字/文本拆分是否会带入标签

**当前状态**: ✅ 已规避

**代码证据** (`TALinePresenter.cs:104-117`):
```csharp
string characterName = line.CharacterName;  // Yarn 已拆分的干净名字
string displayText = line.TextWithoutCharacterName.Text;  // 不含名字前缀
```

**分析**: 使用 Yarn 提供的拆分字段，不手工 Split `:`

---

### 10) 多 presenter 并存时重复响应

**当前状态**: ⚠️ 需确认配置

**风险点**:
- 若 `dialoguePresenters` 列表中有多个会等待 `NextContentToken` 的 presenter
- 可能导致时序混乱

**验证方法**:
检查场景中 `DialogueRunner.dialoguePresenters` 配置

**建议修改**:
- 文档明确：只能有**一个**等待 `NextContentToken` 的 LinePresenter
- 旁路 presenter（日志、回放）应不阻塞

---

### 11) TimeScale 暂停时计时行为

**当前状态**: ⚠️ 待验证

**分析**:
- `TALinePresenter.FadeAlphaAsync` 使用 `Time.deltaTime`（受 TimeScale 影响）
- TA 的 `<waitfor>` 可能使用不同时间源

**验证方案**:
```csharp
Time.timeScale = 0;
// 触发 <waitfor=1>，观察是否继续推进
```

**建议修改**:
- 在文档中明确"对话使用 scaled/unscaled 时间"
- 或提供配置选项

---

### 12) 验证清单覆盖不足

**当前状态**: ❌ **确认命中**

**缺失的关键测试用例**:

| 缺失项 | 对应问题 |
|--------|----------|
| `<?event>` + Skip 语义 | #8 |
| `{}` 与 Yarn 插值冲突 | #3 |
| 场景切换/Disable 不卡死 | #5 |
| 多语言/长文本/富文本嵌套 | 通用 |

**建议修改**: 补充测试脚本

```yarn
title: TA_Regression_Test
---
// 测试 {} 与插值冲突
Test: 你有{$count}个，{fade}看这里{/fade}

// 测试 Skip 事件语义
Test: 事件1<?e1>事件2<?e2>事件3<?e3>

// 测试长文本嵌套闭合
Test: <shake><wave>嵌套<rainbow>效果</rainbow>测试</wave></shake>

// 测试本地化占位
Test: {0}说了{1}
===
```

---

## 必须修改项汇总

| 优先级 | 修改项 | 影响文件 |
|--------|--------|----------|
| P0 | Skip 时 `TrySetResult` 兜底 | `TALinePresenter.cs` |
| P0 | 补充回归测试脚本 | `TestDialogue.yarn` |
| P1 | Converter 单标签处理（若启用） | `YarnMarkupConverter.cs` |
| P1 | 明确 Converter 启用策略文档 | 使用指南 |
| P2 | TimeScale 行为验证与文档化 | 待验证后决定 |
| P2 | Skip 事件过滤策略（若验证命中） | `TADialogueEvents.cs` |

---

## 待确认事项

请确认以下问题以便确定最终修改方案：

1. **项目中是否有基于 Yarn `ActionMarkupHandler` 的现有实现？**
2. **是否需要支持 `YarnMarkupConverter`，还是完全采用"脚本迁移"策略？**
3. **对话时间应使用 scaled 还是 unscaled 时间？**
4. **Skip 时事件应该：全部触发 / 全部不触发 / 分类处理？**


改进建议：

整体评价：**这份自查与修复计划质量不错，可以直接进入“落地执行”**，但我建议你在执行前把里面的几处“待确认/待验证”改成**明确的决策与验收标准**，否则很容易修着修着又回到“看起来没问题、线上才炸”的状态。

下面我按“已足够落地 / 还需要补强的点 / 我建议的最小改动”来评价（我会尽量用你这份计划里的条目编号对齐）。

---

## 已经可以直接落地执行的部分（做得对）

### ✅ #1/#2/#4：明确“Converter 是死代码、默认全迁移”非常关键

他把最容易引爆的混用路径（converter + 手写 TA 标签）明确成“当前不会触发”。这一步会**显著降低风险面**。 

### ✅ #6：输入门控用了 `IsShowingLine`，规避同一帧 Skip+Next

这条是实打实的“行为层”规避，不是口头承诺。

### ✅ #9：名字/正文取 Yarn 拆分字段，避免 Split `:`

这在多语言、角色名含冒号时非常重要。他已对齐了正确做法。

### ✅ #5/#12：把“死等”与“测试缺失”都标为 P0

优先级排序也合理：先保命（不卡死），再补回归覆盖。

---

## 仍建议优化后再落地的点（但不是要你推翻重做）

### 1) #5 的兜底修复方向对，但他给的“NextContentToken 也 TrySetResult”不够严谨

他打算在 `HurryUpToken` 里 `TrySetResult(true)`，这很好，能消掉大部分“Skip 不触发 onTextShowed”死等。

但他又建议“NextContentToken 取消也 TrySetResult(true)”。这会让一个边界场景变得不清晰：

* `NextContentToken` 通常是“进入下一句”的信号，如果你用它去提前完成“文字展示完成”的 task，可能会出现“还没显示完也算完成”的语义混淆（尤其以后加淡入淡出或事件暂停时更危险）。

**我建议的最小改动**：

* `textShowCompletionSource` 只代表“文字已经完整呈现或被强制完成”，它的兜底触发只应该来自：

  * `onTextShowed`
  * `HurryUpToken`（Skip）
  * `OnDisable/OnDestroy`（避免场景切换卡死）
* `NextContentToken` 不要参与 `textShowCompletionSource`，它应该只用于第二阶段 `await NextContentToken`。

也就是把“两个 await 的语义”彻底隔离：

1. `await textShown`（保证显示完）
2. `await next`（等待用户继续）

这样最不容易越修越乱。

---

### 2) #8（Skip 时事件爆发）现在还是“待验证”，但它其实是高概率会遇到的真实坑

他把 #8 评为“待验证，中”。
我会更保守一点：如果你们会用 `<?sound>`、`<?camera>` 这类事件，**Skip 时爆发几乎必踩**（不同 TA 配置只是“爆多少”的区别）。

**建议的最小改动（不用等验证也能先加）**：

* 在 `SkipCurrentLine()` 或 HurryUp 回调里设置 `isSkipping = true`，并且把它持续到“文字已展示完”那一刻再归零。
* `OnMessage` 里至少支持一个“白名单策略”：

  * Skip 期间只允许 `pause`（通常你会跳过 pause）、或允许 `emotion`（不致命），但屏蔽 `sound`/`camera` 这类“会叠加”的事件。
* 同时把策略写进文档，变成团队约定。

这属于“加个保险丝”，成本低，收益高。

---

### 3) #3 `{}` 与 Yarn 插值冲突：验证方案很好，但要加一个“结论落地动作”

他给了验证用例，也给了替代建议（改用 `<fade>`）。
但落地时经常会出现“验证了没问题，然后策划开始混用更复杂的 `{$var}` + `{fade}` + 本地化占位，最终又出问题”。

**建议的最小改动**：

* 无论验证结果如何，都在写作规范里明确：**对话脚本里 `{}` 只允许 Yarn 插值，TA 的 fade 一律用 `<...>`**。
  这是最稳定的规范化路径（否则以后你得持续兼容语法冲突）。

---

### 4) #7：是否放弃 `IActionMarkupHandler`，他把它放在“待确认”，但这里应该尽早定方向

现在实现是“完全绕过 Yarn 的 handler，走 TA 事件”。
这会影响你们历史资产与未来扩展方式，所以不适合一直悬而未决。

**建议的最小改动**：

* **方案推荐**：明确“我们彻底迁移到 TA 事件”，然后给一份迁移规则：原先的 Yarn handler 全部对应到哪些 `<?event>`。


如果你们已经有不少 Yarn handler 资产，我更建议 B；如果现在几乎没有历史资产，就选 A，别背兼容成本。

---

## 能否直接执行？

**可以执行**

---

## 最后一句评价

他的计划已经从“概念方案”进化成了“工程自查清单 + 明确 P0”，**整体可以落地**；你只需要把我上面提的 4 个“决策/语义边界”补上（尤其是 #5 的 await 语义隔离、#8 的 Skip 事件保险丝、#7 的战略定案），就能变成一套相对稳定、可交付、可扩展的集成方案。
