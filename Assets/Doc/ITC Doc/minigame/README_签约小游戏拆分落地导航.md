# 签约小游戏拆分落地导航（v1）

> 位置：`Assets/Doc/ITC Doc/minigame`
> 目标：把总设计案拆成可直接交给“落地AI”的独立小游戏设计包，保证 UIKit/ResKit/WebGL 约束下可一发做出可玩基础版。

## 目录结构

- `00_shared/00_签约小游戏共通落地规范.md`
- `01_document_review/01_文书审核-落地设计.md`
- `01_document_review/01_文书审核-代码文档与操作说明.md`
- `02_rune_typing/02_符文控制板QTE-落地设计.md`
- `02_rune_typing/02_符文控制板QTE-代码文档与操作说明.md`
- `03_rune_verify/03_符文核验找茬-落地设计.md`
- `04_stamp/04_印章选择盖印-落地设计.md`
- `05_soul_collect/05_灵魂收取分割-落地设计.md`
- `06_bean_sell/06_豆罐头推销-落地设计.md`
- `07_settlement/07_满意度结算离场-落地设计.md`
- `99_whitebox_language/99_白模测试设计语言-v1.md`
- `MINIGAME_GUIDE_CONSOLIDATED.md` (最终统合操作手册)

## 使用顺序（给生产AI）

1. 先读 `00_shared`，锁定架构与接口（不要先写玩法脚本）。
2. 再读目标小游戏文档，按“状态机 -> 交互参数 -> 配置项 -> 接口预留 -> 验收”实现。
3. 资源未到位时，必须按 `99_whitebox_language` 先出白模可玩版。
4. 通过白模验收后再替换正式资源，禁止在替换资源时改动交互判定逻辑。

## 当前项目已对齐事实（用于防止偏题）

- 现有架构根：`MainMenuApp`，并已接入 `MainMenuUIKitConfig` + `ResKitPanelLoaderPool`。
- 对话面板：`ITCDialoguePanel`，当前仅注册视觉命令（`itc_bg` 等），小游戏命令待扩展。
- Yarn 现状：Day1 已有两位客户（Emmett、Thomas）完整占位流程，变量包括 `Sign_mistake`、`satisfaction`、`Route_*`。
- 平台目标：WebGL（线程关闭，主线程协程路径优先）。

## 文档边界说明

- 本批文档为“二次细化设计”，不是最终代码实现。
- 每份文档都包含：节奏控制、手感参数、可配置项、SFX/VFX接口、UIKit生命周期、ResKit加载、WebGL约束、白模验证。
- 若后续策划改规则：优先改各小游戏文档中的“可配置项”，避免改流程代码。
