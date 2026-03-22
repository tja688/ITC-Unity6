# TestScene_sign UI test 新签约流程迁移方案

## Summary

把旧场景 `TestScene_sign mian` 里的 Sign 专属流程控制，从“单个 `SignDialogueSlotRuntime` 大脚本 + 固定槽位复用”迁移为“保留 Yarn 主桥、改成多脚本协作、每句面板独立实例化”的新实现，落地目标场景是 `Assets/Tests/Test scene/TestScene_sign UI test.unity`，新脚本全部放在 `Assets/Tests/Test scripts/新签约场景脚本测试/`。

迁移后的核心规则固定为：

- 保留 `DialogueRunner` 作为 Yarn 主驱动，保留 `TALinePresenter` 和 `DialogueContinueHandler` 作为主桥。
- NPC 当前时间段内每句都保留自己的真实面板实例；切换到新 NPC 时，清掉上一 NPC 的全部实例和历史。
- `对话框前置` 只承载 NPC 最新一句，`对话框后置` 只承载 NPC 上一句/历史浏览窗口；两者都不再复用旧内容。
- 玩家对白、心理活动、选项都进入玩家框语义；选项改为内嵌到玩家框实例内部，不再使用场景里原有的独立 `OptionsPanel` 视觉。
- 小游戏继续占位，不做正式玩法重写，只保留命令触发、显示/阻塞、关闭后恢复推进。

## Implementation Changes

### 1. 运行时结构拆分

新增以下运行时脚本，挂载到 `TestScene_sign UI test` 的新 Sign 场景根对象上：

- `SignDialogueSceneBootstrap`
  负责集中绑定场景引用、初始化子模块、注册 Yarn 命令、统一暴露给旧桥接脚本。
- `SignDialogueRuntimeFacade`
  作为 `TALinePresenter` / `DialogueContinueHandler` 的新对接入口，只提供 `RouteLine`、`IsContinueInputBlocked`、`ShouldSuppressLegacyPresenterVisuals` 这类薄接口，本身不承载具体业务。
- `SignDialoguePanelInstance`
  挂到 `对话框前置` / `对话框后置` / `玩家框` prefab 根节点，负责：
  设置内嵌 TMP 文本；
  调用子节点 `弹出` / `回缩` 的 `DOTweenAnimation`；
  通知回缩结束后销毁；
  可选创建运行时内容容器给选项使用。
- `SignDialoguePanelFactory`
  负责实例化 prefab、设置父节点、对齐到锚点/遮罩、填充文本、返回实例控制器。
- `SignDialogueNpcTrack`
  负责当前 NPC 时间段的状态：
  当前前置实例；
  当前后置显示实例；
  当前 NPC 的历史实例列表；
  NPC 切换时的清理。
- `SignDialogueHistoryBrowser`
  负责 `对话框后置遮罩` 区域内的滚轮浏览，只切换“后置显示的历史实例”，鼠标移出后回到默认上一句。
- `SignDialoguePlayerTrack`
  负责玩家对白、心理活动、选项框实例的生命周期；默认不保留玩家历史，只保留当前显示实例。
- `SignDialogueOptionsPresenter`
  作为新的 Yarn 选项 presenter，接 `OptionSet` 后创建一个新的玩家框实例，并把选项按钮生成到该实例内部的运行时容器。
- `SignDialogueCommandBridge`
  负责 `itc_sign_npc_enter` / `itc_sign_npc_exit` / `itc_sign_role` / `itc_sign_minigame`。
- `SignDialoguePlaceholderMinigameBridge`
  负责小游戏占位 token 到现有场景控件的映射，以及小游戏期间的继续输入阻塞。

### 2. 面板实例化规则

NPC 新句到来时固定执行：

- 创建一个新的前置面板实例，挂到 `SignUI/纯背景/对话框前置遮罩`，填充该句文本，播放 `弹出`。
- 如果上一句存在：
  额外创建一个新的后置历史实例，文本复制自“上一句前置内容”；
  该实例加入当前 NPC 的历史列表；
  该实例成为默认后置显示对象，挂到 `SignUI/纯背景/对话框后置遮罩`，播放 `弹出`。
- 被替换掉的旧前置实例不搬运、不复用，直接播放 `回缩` 后销毁。
- 历史浏览只在“历史实例列表”里切换后置显示对象，不去改写前置，也不复用旧对象内容。

玩家对白 / 心理活动时固定执行：

- 创建新的玩家框实例，挂到 `SignUI/纯背景/玩家框遮罩`，填充文本，播放 `弹出`。
- 上一个玩家框实例若存在，播放 `回缩` 后销毁。
- 不做玩家历史列表。

选项态固定执行：

- `SignDialogueOptionsPresenter` 收到 Yarn 选项后，创建一个新的玩家框实例。
- 隐藏或清空该实例的文本区域，在实例内部创建选项容器，继续使用现有 `OptionButton.prefab` 作为按钮资源。
- 只有点击选项按钮才推进；点击外部区域继续被阻塞。
- 选项结束后，该玩家选项实例播放 `回缩` 并销毁。

### 3. 场景与桥接调整

场景 `TestScene_sign UI test` 做如下结构性迁移：

- 保留 `DialogueRunner`。
- 保留 `TALinePresenter` 与 `DialogueContinueHandler`，但新增对 `SignDialogueRuntimeFacade` 的支持；旧 `SignDialogueSlotRuntime` 引用逻辑保留为 fallback，避免旧场景立即失效。
- `TALinePresenter` 在 Sign 模式下继续关闭 legacy 文本显示，但路由目标改为新 facade。
- `DialogueContinueHandler` 的阻塞判定改为优先查询新 facade。
- `DialogueRunner` 的 options presenter 从现有 `OptionsPresenter` 切到 `SignDialogueOptionsPresenter`。
- 场景内旧 `签约特制对话系统/DialogueCanvas/DialoguePanel/OptionsPanel` 不再承担最终视觉，只保留为桥接节点或被简化为无视觉 presenter 宿主。
- 使用现有锚点/遮罩：
  `对话框前置遮罩`
  `对话框后置遮罩`
  `玩家框遮罩`
  `对话框掉落位置`
  `玩家框隐藏位置`

### 4. 对外接口与约束

需要新增/调整的公共契约如下：

- `TALinePresenter`
  新增对 `SignDialogueRuntimeFacade` 的可选引用与自动发现逻辑；旧 `SignDialogueSlotRuntime` 仍保留兼容。
- `DialogueContinueHandler`
  同上，阻塞源改为“新 facade 优先，旧 runtime 兜底”。
- `SignDialoguePanelInstance`
  约定 prefab 根下必须能解析：
  一个主文本 TMP；
  一个名为 `弹出` 的 `DOTweenAnimation`；
  一个名为 `回缩` 的 `DOTweenAnimation`。
- `SignDialogueOptionsPresenter`
  替换目标场景中的 `OptionsPresenter` 角色，但不改 Yarn 剧本写法。
- `SignDialoguePlaceholderMinigameBridge`
  约定 token 先支持当前测试剧本里的 `doc_review` 和 `rune`，其余 token 走统一占位显示和输入阻塞。

## Test Plan

### 1. 主链验证

在 `TestScene_sign UI test` 播放 `Assets/Tests/Test dialogue/TestMinigames.yarn`，逐项验证：

- 第一位 NPC 的首句只出现前置。
- 第二句出现时：
  新前置为最新句；
  新后置为上一句；
  旧前置实例已回缩销毁。
- 后续多句时，历史列表持续增长，但场上只保留一个前置显示和一个后置显示。
- 鼠标在后置区域滚轮时，可在当前 NPC 历史实例间切换。
- 鼠标移出后，后置自动回到默认上一句。

### 2. 玩家与选项验证

- 玩家对白进入玩家框实例，点击任意区域可继续。
- 心理活动同样走玩家框实例。
- 选项出现时，玩家框实例内部生成按钮。
- 点击非选项区域不推进。
- 点击选项后正常回到 Yarn 主流程，选项实例回缩销毁。

### 3. 小游戏占位验证

- `itc_sign_minigame doc_review` 和 `itc_sign_minigame rune` 都能触发占位显示。
- 占位期间继续输入被阻塞。
- 占位结束后恢复正常推进。
- 当前场景已有的 `契约管道_Ctrl` / `电话_Ctrl` / `罐头管道_Ctrl` / `打字机_Ctrl` / `印章动画` 不做深改，只验证触发和显示链路。

### 4. NPC 切换与兼容验证

- `itc_sign_npc_enter 审核员B` 后，审核员 A 的所有 NPC 实例被清理。
- 新 NPC 的历史从空开始重新累计。
- 旧场景 `TestScene_sign mian` 仍能通过旧 `SignDialogueSlotRuntime` 跑通，不因为桥接层升级直接断掉。

## Assumptions And Defaults

- 当前实现范围只覆盖 `TestScene_sign UI test` 的新流程迁移，不同步改造正式签约业务场景。
- NPC 历史“全实例保留”只针对当前 NPC 时间段；切 NPC 立即清空。
- 玩家历史不保留；玩家框始终只保留当前显示实例。
- 选项按钮继续复用现有 `Assets/Prefabs/UI/OptionButton.prefab`，只改变承载方式，不重做按钮美术。
- 文本一律写入 prefab 自身内嵌 TMP，不再使用外部统一文字层。
- 旧 `SignDialogueSlotRuntime` 不作为目标实现继续扩展，只保留兼容和参考价值。
