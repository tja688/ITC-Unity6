# ITC 签约小游戏落地操作手册（统合版）

> **文档性质**：本文档为签约小游戏系统的最终统合指南，聚合了各子环节的设计案、代码说明及白模规范。
> **目标**：保证小游戏原子化、WebGL 可运行、Yarn 流程无缝衔接。

---

## 1. 共通架构与落地规范 (Shared Standards)

### 1.1 项目现状
- **架构根**：`MainMenuApp` (`Assets/Scripts/MainMenu/MainMenuApp.cs`)
- **UIKit 配置**：`MainMenuUIKitConfig`，使用 `ResKitPanelLoaderPool`。
- **对话入口**：`ITCDialoguePanel` 注册 Yarn 命令触发小游戏。

### 1.2 QFramework 责任边界
- **Model**
  - `ContractFlowStateModel`：记录本轮客户、满意度、失误标记、中间判定缓存。
  - `ContractClientConfigModel`：客户配置读取与缓存。
- **Command**
  - `StartMiniGame` 系命令：统一开局，暂停 Yarn。
  - `SubmitMiniGameResult` 系命令：统一收口，写 Model + 写 Yarn 变量，关闭面板并恢复对话。
- **Controller (UIPanel)**
  - 各小游戏面板负责输入、表现和局部状态，不直接写业务总状态。

### 1.3 Yarn 桥接协议
- **常用命令**：
  - `<<itc_doc_review clientId>>`
  - `<<itc_rune_typing clientId gridSize>>`
  - `<<itc_rune_verify clientId>>`
  - `<<itc_stamp_select clientId>>`
  - `<<itc_soul_collect clientId targetPercent>>`
  - `<<itc_bean_sell clientId>>`
- **变量规范**：
  - 全流程：`$Sign_mistake`, `$satisfaction`
  - 玩法结果：`$Route_<MiniGame>_*`

### 1.4 共通约束
- **字体强制优先**：所有可见文本必须使用 `Assets/Arts/Fronts/WenQuanYi Bitmap Song 16px SDF.asset`。
- **WebGL 兼容**：禁用多线程，计时与流程全部走协程；避免大纹理同时加载。
- **原子化**：每个 Panel 必须可在空白场景独立运行，附带“一键测试启动器”。
- **说明系统**：内嵌中文说明，支持 `GuideToggleButton` 一键开关；标签跟随对应对象。

---

## 2. 小游戏详细设计与实现 (Mini-game Details)

### 2.1 文书审核 (DocumentReview)
- **定位**：客户入场后首环节。检查封蜡、墨水、日期、身份。
- **关键脚本**：
  - `Assets/Scripts/Contracting/DocumentReview/DocumentReviewPanel.cs`
  - `Assets/Scripts/Contracting/DocumentReview/DocumentReviewGuideTag.cs`
  - `Assets/Scripts/Contracting/DocumentReview/DocumentReviewQuickTestLauncher.cs`
- **核心判定**：通过 (Pass) 或 退回 (Reject)。
- **变量回写**：`$Route_DocReviewResult` (passed/rejected_correct/rejected_wrong)。

### 2.2 符文控制板 QTE (RuneTyping)
- **定位**：文书审核后的 WASD QTE。按序输入符文序列。
- **关键脚本**：
  - `Assets/Scripts/Contracting/RuneTyping/RuneTypingPanel.cs`
  - `Assets/Scripts/Contracting/RuneTyping/RuneTypingGuideTag.cs`
  - `Assets/Scripts/Contracting/RuneTyping/RuneTypingQuickTestLauncher.cs`
- **输入通道**：WASD + Confirm (Space/Enter)；保留屏幕按钮作为 WebGL 兜底。
- **变量回写**：`$Route_QTEErrorCount`。

### 2.3 符文核验找茬 (RuneVerify)
- **定位**：RuneTyping 后的 30% 概率插入环节。限时 5s 找茬。
- **关键脚本**：
  - `Assets/Scripts/Contracting/RuneVerify/RuneVerifyPanel.cs`
  - `Assets/Scripts/Contracting/RuneVerify/RuneVerifyGuideTag.cs`
  - `Assets/Scripts/Contracting/RuneVerify/RuneVerifyQuickTestLauncher.cs`
- **失败惩罚**：若失败，后续盖印环节 (Stamp) 会出现屏幕晃动干扰。
- **变量回写**：`$Route_RuneVerifyResult` (skipped/success/failed)。

### 2.4 印章选择与盖印 (Stamp)
- **定位**：知识判断（四选一） + 手感判定（蓄力蓄能）。
- **关键脚本**：
  - `Assets/Scripts/Contracting/Stamp/StampPanel.cs`
  - `Assets/Scripts/Contracting/Stamp/StampGuideTag.cs`
  - `Assets/Scripts/Contracting/Stamp/StampQuickTestLauncher.cs`
- **判定区间**：完美窗、普通窗、失败窗；错选类型直接计失误。
- **变量回写**：`$Route_StampType`, `$Route_StampTimingResult` (perfect/normal/failed)。

### 2.5 灵魂收取分割 (SoulCollect)
- **定位**：分灵刀分割灵魂光球。
- **关键脚本**：
  - `Assets/Scripts/Contracting/SoulCollect/SoulCollectPanel.cs`
  - `Assets/Scripts/Contracting/SoulCollect/SoulCollectGuideTag.cs`
  - `Assets/Scripts/Contracting/SoulCollect/SoulCollectQuickTestLauncher.cs`
- **规则**：收取过多 (-满意度)，收取过少 (+失误)。
- **变量回写**：`$Route_SoulCollectPercent`。

### 2.6 豆罐头推销 (BeanSell)
- **定位**：Day2+ 触发的选择题环节。
- **关键脚本**：
  - `Assets/Scripts/Contracting/BeanSell/BeanSellPanel.cs`
  - `Assets/Scripts/Contracting/BeanSell/BeanSellGuideTag.cs`
  - `Assets/Scripts/Contracting/BeanSell/BeanSellQuickTestLauncher.cs`
- **变量回写**：`$Route_BeanSellResult`, `$Route_BeanSoldCount`。

### 2.7 满意度结算 (Settlement)
- **定位**：单客户签约链路尾部。聚合结果，计算小费。
- **关键脚本**：
  - `Assets/Scripts/Contracting/Settlement/SettlementPanel.cs`
  - `Assets/Scripts/Contracting/Settlement/SettlementGuideTag.cs`
  - `Assets/Scripts/Contracting/Settlement/SettlementQuickTestLauncher.cs`
- **核心逻辑**：满意度等级判定（Henet 反馈依赖项），小费按阶层发放。

---

## 3. 白模测试设计语言 (Whitebox Language)

- **颜色语义**：青色 (交互)、绿色 (正确)、红色 (错误)、黄色 (计时压力)。
- **操作标注**：所有交互对象必须带跟随标签 `WB_<Flow>_<Type>_<Index>`。
- **槽位隔离**：使用 `SLOT_*` 命名可替换节点，更换资源时不改逻辑层。
- **验收标准**：一眼识别焦点、操作可理解、变量回写准确。

---

## 4. 关键文件路径一览 (Script Paths)

### 4.1 公共底层 (`Common/`)
- `Assets/Scripts/Contracting/Common/ContractingTypes.cs`
- `Assets/Scripts/Contracting/Common/ContractFlowStateModel.cs`
- `Assets/Scripts/Contracting/Common/ContractClientConfigModel.cs`
- `Assets/Scripts/Contracting/Common/ContractCommands.cs`

### 4.2 编辑器工具 (`Assets/Editor/Contracting/`)
- `DocumentReviewPanelPrefabBuilder.cs`
- `DocumentReviewQuickTestTools.cs`
- `RuneTypingPanelPrefabBuilder.cs`
- `RuneTypingQuickTestTools.cs`
- （其余以此类推...）

---

## 5. 实施优先级
1. **文书审核** -> **符文QTE** -> **印章** (主线必经)
2. **灵魂收取** -> **符文核验** (核心闭环)
3. **结算** -> **推销** (流程完整性)
