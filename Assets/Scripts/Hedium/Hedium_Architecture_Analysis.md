# Hedium (签约场景) 移植架构解析文档

## 1. 整体架构概述

Hedium 模块是旧版游戏中的“签约、小游戏、结算/退出”核心玩法流程控制中心。它采用**状态机（State Machine）**模式管理签约的各个阶段，并通过**中心化事件总线（SlotCenter）**进行模块间的解耦与通信，同时通过 **Yarn Spinner** 命令实现了与叙事（对话）系统的深度绑定。

### 核心设计模式
*   **状态机模式（State Machine）**：通过 `IContractStage` 接口设计，将整个签约流程拆分为独立的小阶段（文书核验、符文输入、盖章、灵魂收取、突发事件等）。
*   **事件驱动（Event-Driven）**：使用单例模式的 `SlotCenter` 统一管理所有字符串常量的事件派发和监听。
*   **配置驱动（Data-Driven）**：使用 `ScriptableObject` (`HeContractGameConfig`) 统一管理小游戏时间、满意度、失败次数、难度等所有配置项。

---

## 2. 核心控制流程与入口

### 2.1 主流程控制器：`SigningFlowManager`
`SigningFlowManager.cs` 是整个签约玩法的核心入口和控制中枢。
*   **上下文数据**：通过 `HeContractContext ctx` 维护完整的签约流程上下文数据（包含顾客信息、文书真实性、满意度状态、错误统计等）。
*   **流程流转**：实例化了多个继承自 `IContractStage` 的系统（如 `DocumentVerifier`、`RuneInputManager` 等）。
*   **Yarn系统集成入口**：
    在 `Start()` 方法中，它把各个阶段注册到了 Yarn Spinner 的运行时命令（Commands）中，保证对话能够无缝呼出特定的签约小游戏环节：
    - `<<he_doc_review>>` -> 触发**文书核验**小游戏。
    - `<<he_rune_typing>>` -> 触发**符文敲击/节奏输入**小游戏。
    - `<<he_stamp_select>>` -> 触发**印章选择及盖章**小游戏。
    - `<<he_soul_collect>>` -> 触发**灵魂收取**（切割）小游戏。
    - `<<he_special_event>>` -> 触发**突发特殊事件**（QTE等）。

### 2.2 UI与动画调度：`HeContractUIManager`
负责处理各阶段的视觉反馈：
*   **面板状态切换**：通过 `UIState` 区分当前小游戏，调度不同 UI 组的显示与隐藏。
*   **Spine骨骼动画**：处理 `SkeletonGraphic` 等 Spine 组件播放进出场动画（比如气动通道 `pneumaticChannelSkeleton`）。
*   **Feel反馈**：依赖 `MoreMountains.Feedbacks (Feel)` 插件，调用 `MMF_Player` 来执行统一的 UI 进入（Enter）和退出（Exit）动效（如打字机 `typewriterGameObject` 入场）。
*   **拖拽交互绑定**：借助自定义类（`DraggableUI`, `EntryAnimation`, `SkeletonHoverHighLight`）实现拖拽和悬浮处理，并将结果提交至 `SlotCenter` 触发对应判定逻辑。

### 2.3 测试与调试入口：`HeGameEnterMenu`
这是为开发者进行快速 Debug 提供的入口脚本：
*   **挂载Context Menu**：利用 `[ContextMenu]` 特性，可直接在 Inspector 面板强制触发某一阶段进行测试（如 `DocumentVerifierStageStart`, `StampStageStart` 等）。
*   **动态难度调试**：提供 `UpDataConfig(int day)` 接口用于根据游戏天数（Day）来初始化测试配置。

---

## 3. 具体小游戏模块 (IContractStage 子系统)

小游戏的设计遵循统一接口规范：`Enter()`, `Update()`, `Exit()`。不同的小游戏只关心各自的内部判定与视图通信。

### 3.1 文书核验 (`DocumentVerifier`)
*   **逻辑**：根据 `HeContractContext` 中的属性判断契约是否有破损、伪造、错误日期、甚至顾客是否为危险分子等 (`DocumentError` 枚举)。
*   **交互**：UI端拖动放大镜/印章悬浮检查，最终由玩家抉择并向 `SlotCenter` 派出 `DocumentErrorChosen` 事件，阶段完成后回调 `SigningFlowManager.OnMinigameDone` 继续 Yarn 对话。

### 3.2 节奏/符文输入 (`RuneInputManager` & `Rhythmgame.cs`)
*   **逻辑**：控制原本的“打字”式操作改版为了基于**节奏输入**的游戏模式。
*   **实现**：生成一定数量（比如 6 个）的随机箭头符文组，玩家通过 `HeKeyInput` （监听 WASD 或上下左右键）依据生成的方向输入。输入对则完成一轮（Success），输出错则执行失败逻辑。
*   **关键架构**：拥有 `RhythmgameHandle` 句柄进行多轮游戏（TuneCount）的管理和失败重试管理。

### 3.3 盖章 & 充能小游戏 (`StampSystem` & `HeChargingGame.cs`)
*   **逻辑**：确定文书正确后进行印章选择及盖章。
*   **机制**：`HeChargingGame.cs` 控制印章的蓄力判定（Charging）。需要在精准的蓄力点松开拖拽（通常绑定在 `EndDragEvent` ），分为完美与失败，并通过 `SlotCenter` 派发 `OnChargingGameEnd` 事件传回结果给 `StampSystem`。

### 3.4 灵魂收取 (`SoulHarvestSystem`)
*   **逻辑**：根据客户的文书上的预设价值（百分比），在一定容差范围内（例如 0.05 半成误差），使用切割刀（灵魂切割线）按比例截取。
*   **实现**：通过横向位移游标以及添加手部抖动偏移（Shake）实现操作难度控制，切割完成后更新满意度属性。

### 3.5 突发事件 (`SpecialEventSystem`)
*   **逻辑**：基于一定触发几率 (`eventTriggerChance`) 调用的纯 QTE 阶段。
*   **内容**：包括顾客接电话(Space)、拔枪(F)、癫痫发作(H)、变身(C)、突然对话(R) 等。规定时间内正确输入即可，否则扣除顾客满意度/引发恶性后果。

---

## 4. 事件总线机制：`SlotCenter`

为了避免复杂的组件互相获取，Hedium 使用了一个自研事件中心。
*   **注册与注销**：`add_listener(name, Action)`, `remove_listener`。
*   **事件触发**：`trigger_event(name, params)`。
*   **约定定义**：所有事件名强制写在 `HeEventNames` 或枚举 `HeEventNamesOption` 中，如打字机完成事件 (`OnTypeWriterEndType`)、节奏游戏结果 (`OnRythmGameEnd`)。

**开发提示：**如果发现不同阶段流程卡住，首要检查是否是 `SlotCenter` 中的某个回调抛出了异常，或者事件标识名不一致导致的。

---

## 5. 迁移对接注意事项 (For 新项目对接)

1.  **修复外部库引用**：
    该系统重度依赖 `MoreMountains.Feedbacks` 和 `Spine.Unity`，需要确保新项目已装载相关插件，或者将项目中所有的 `MMF_Player` 和 `SkeletonGraphic` 根据新架构进行平替。
2.  **Yarn系统整合**：
    目前的接口 `runner.AddCommandHandler` 会因静态和实例绑定等问题报 "needs a target" 或者无响应（之前报的错）。建议在新版项目中采取单例中转调用，或为 Yarn Unity 6.x 修改相应的 Handler 注册方式。
3.  **UI 预制体替换**：
    `c:\Users\jinji\Documents\GitHub\ITC-Unity6\ITC-Unity6\Assets\Scripts\Hedium\Prefab\签约场景打包_移植用.prefab` 中保存着 UI 参考，注意修复预制体中脚本丢失(`Missing Script`)的问题（通常是由 `HeGameEnterMenu`、`HeContractUIManager` 以及 `SlotCenter` 引起）。
4.  **按键输入系统更新**：
    当前的 `HeKeyInput` 主要使用了旧式 `Input.GetKeyDown` 或 `Input.GetAxis`。若 Unity 6 新项目使用了 `Input System Package (New)`，需要优先重构 `HeKeyInput.cs` 中的底层调用，避免按键无交互。

---

希望这份文档能为您及团队提供该老项目模块的清晰蓝图。所有子模块的核心类名均保持原状，可以直接使用全局搜索以跟进代码细节。
