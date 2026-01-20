# **数字演出的灵魂：基于戏剧理论与计算韵律学的剧本对白“表达投放”系统化研究**

## **1\. 绪论：数字文本的静默与表达的鸿沟**

在当今的数字互动叙事与电子游戏领域，剧本创作已经达到了前所未有的复杂度。然而，当这些精心雕琢的文本最终呈现给玩家时，往往面临着“表达投放”（Delivery）的巨大断层。在传统戏剧、电影或广播剧中，剧本仅仅是蓝图，演员的“投放”——即对白说出的节奏、停顿、重音、音调起伏（Intonation）、音色变化——才是赋予文本情感与生命的关键工序。而在大多数非全语音（Full Voice Acting）的游戏中，这一工序被简化为单调的字幕显示或机械的打字机音效，导致了严重的“情感降维”。

本报告旨在响应用户的深度研究需求，构建一套通用的、跨学科的“数字导演引擎”理论框架。该框架不依赖于昂贵的人类配音演员，而是通过解构传统戏剧导演与表演专家的行业最佳实践（Best Practices），结合语言韵律学（Linguistic Prosody）的量化参数，以及大语言模型（LLM）的语义分析能力，将单调的文本信息转化为具有高度设计感、表达力和情感动态的演出对白。

本研究将深入探讨如何将“潜台词”（Subtext）转化为可执行的代码指令，如何利用“动作化”（Actioning）理论指导音频合成与动态排版（Kinetic Typography），并最终形成一套可供AI执行的标准化工作流。这一研究虽然根植于斯坦尼斯拉夫斯基（Stanislavski）等传统戏剧理论，但其应用场景完全针对数字化环境，甚至在“以字幕呈现”的背景下，利用数字媒介的特性突破人类生理限制，创造出超现实的艺术表达。

## ---

**2\. 戏剧本体论：对白投放的深层逻辑解析**

要让计算机或AI像导演一样控制对白，首先必须理解人类导演是如何工作的。在戏剧排练场，导演并不是简单地告诉演员“要悲伤”或“要快乐”，因为情绪是结果而非过程。导演通过控制“动机”、“动作”和“节拍”来塑造表达。

### **2.1 表演的原子单位：节拍（The Beat）与动态调整**

在戏剧理论中，“节拍”（Beat）并非音乐节奏上的拍子，而是剧本结构中的最小行动单元。一个节拍代表了角色在追求某个具体目标（Objective）时所采取的一种特定策略或状态。当角色的策略发生改变，或者有新信息进入场景时，节拍就发生了转换（Beat Change）1。

对于游戏演出系统而言，识别“节拍”至关重要。一个长句或一段对话往往包含多个节拍，每个节拍的“投放”参数应当截然不同。

* **单一文本的多重节拍分析：**  
  * *文本：* “我求求你……把枪放下！否则我就开枪了！”  
  * *节拍A（“我求求你……”）：* 策略是“乞求”（To Beg）。投放特征：音调上扬，语速缓慢，伴随颤抖，音量低。  
  * *节拍转换点（“……”）：* 这是一个决策时刻，角色意识到乞求无效，决定改变策略。  
  * *节拍B（“把枪放下！否则我就开枪了！”）：* 策略是“威胁”（To Threaten）。投放特征：音调下压，语速急促，重音加强，音量骤增。

如果系统不能识别这两个节拍的转换，仅仅用统一的“焦虑”情绪来渲染整句话，演出就会显得平庸且缺乏层次。因此，**数字投放的第一原则是：表达参数必须随节拍动态变化，而非随句子静态设定**。

### **2.2 动作化（Actioning）：将情感量化为动词**

英国戏剧界广泛采用的“动作化”（Actioning）技术，由导演Max Stafford-Clark等人推广，是连接剧本与表演最精准的桥梁 3。该技术要求为每一句台词指派一个**及物动词（Transitive Verb）**，即“我要对听者做什么”。

相比于形容词（如“生气的”），及物动词（如“惩罚”、“羞辱”、“审问”）具有明确的方向性和物理属性，这使得它们极易被映射为数字信号参数。

| 情感范畴 (Adjective) | 动作动词 (Transitive Verb) | 投放物理特征 (Physical Delivery) | 游戏化映射 (Game Mapping) |
| :---- | :---- | :---- | :---- |
| **愤怒 (Angry)** | **粉碎 (To Crush)** | 重低音，缓慢，每个字都重读，无停顿。 | 屏幕震动(低频高幅)，字体加粗，低音轰鸣。 |
| **愤怒 (Angry)** | **刺痛 (To Sting)** | 高音，极快，尖锐，句尾上扬。 | 字体边缘锐化，高频音效，极快打字速度。 |
| **喜爱 (Loving)** | **抚慰 (To Soothe)** | 柔和，连贯（Legato），音调平稳，气声。 | 波浪文字效果，淡入淡出，柔和正弦波音效。 |
| **喜爱 (Loving)** | **引诱 (To Seduce)** | 压低嗓音，慢速，句尾拖长，抑扬顿挫。 | 慢速波浪，粉色/紫色色调，低音共振。 |

通过建立一个包含数百个“心理动词”的数据库 4，我们可以让AI分析剧本并指派动词，从而获得精确的投放指令，而非模糊的情感标签。

### **2.3 沉默的句法：品特式停顿（Pinteresque Pause）与马梅特式重叠（Mamet Speak）**

在哈罗德·品特（Harold Pinter）和大卫·马梅特（David Mamet）等剧作家的作品中，沉默和节奏本身就是语言的一部分。在游戏文本演出中，单纯的 WaitForSeconds() 函数远远不足以表达这种复杂的戏剧张力。

#### **2.3.1 品特的三种沉默**

6

品特在剧本中严格区分了三种非语言状态，这应成为游戏文本系统的基础标点：

1. **省略号 (...)：** 这是语言的犹豫或寻找。角色在思考措辞，但连接未断。  
   * *数字实现：* 打字速度减慢，字符间出现微小延迟，可能伴随光标闪烁。  
2. **停顿 (Pause)：** 这是一个桥梁。角色在这个间隙中进行了一次激烈的心理活动，通常标志着节拍的转换或潜台词的浮现。  
   * *数字实现：* 文本完全停止输出，持续时间较长（例如1.5-2秒），在此期间可能插入角色立绘的表情变化或环境音效的突出。  
3. **静默 (Silence)：** 这是一个墙壁。沟通彻底破裂，或者发生了某种毁灭性的认知冲击。  
   * *数字实现：* 停止所有文本和音效，清空文本框，甚至停止背景音乐（BGM Cut），造成“真空”感，迫使玩家感受到压抑。

#### **2.3.2 马梅特的重叠与打断**

9

大卫·马梅特的对白特征是碎片化和高频打断（Overlapping）。在传统RPG中，文本框通常是“你方唱罢我登场”，这种轮流机制极其缺乏真实感。

* **最佳实践：** 引入**多通道文本并发系统**。当角色A说到一半被角色B打断时，角色A的文本框不应消失，而应被角色B的新文本框在视觉上覆盖或挤压，同时角色A的最后几个字可能呈现出“被切断”的视觉效果（如乱码或突然终止）。

### **2.4 潜台词与反讽：文本与投放的错位**

朱迪思·韦斯顿（Judith Weston）在《指导演员》（Directing Actors）中指出，潜台词必须比文本“更响亮” 11。在计算语境下，这意味着我们需要支持**语义与韵律的解耦（Decoupling）**。

最典型的高级表达是“反讽”（Sarcasm）。

* *文本：* “这真是太棒了。”（语义情感分析：正面）  
* *潜台词：* “这简直是灾难。”（真实意图：负面）  
* *投放指令：* 必须使用与文本字面意义相反的韵律特征。  
  * *正常“太棒了”：* 高音，快语速，升调。  
  * *反讽“太棒了”：* 极低音，极慢语速，夸张的拖长音（Duration Stretching），以及特殊的“抑-扬-抑”音调轮廓 12。

如果AI仅仅根据文本字面意思进行TTS生成，它会读成高兴的语气，从而破坏剧本。因此，系统必须具备**潜台词侦测层**，允许“投放指令”覆盖“文本语义”。

## ---

**3\. 听觉维度：计算韵律学与参数映射**

为了将上述戏剧理论转化为计算机可执行的数据，我们需要进入语言学与声学的交叉领域——计算韵律学（Computational Prosody）。韵律是语言的音乐属性，主要由音高（Pitch/F0）、时长（Duration）、音强（Intensity）和音色（Timbre）构成 14。

### **3.1 情感声学特征的量化矩阵**

基于多项关于情感语音合成的研究 16，我们可以构建一个高精度的参数矩阵，将戏剧意图映射为具体的声学参数。这个矩阵是“数字导演”的核心数据库。

**表3.1：戏剧动作与声学参数映射表**

| 戏剧动作 (Action Verb) | 音高均值 (Pitch Mean) | 音高范围 (Pitch Range) | 语速 (Speech Rate) | 音强/音量 (Intensity) | 音高轮廓 (Contour) | 抖动/微扰 (Jitter/Shimmer) |
| :---- | :---- | :---- | :---- | :---- | :---- | :---- |
| **攻击/斥责 (To Attack)** | 低 (冷怒) / 高 (热怒) | 宽 (爆发性) | 快 (咄咄逼人) | 高 (强攻击性) | 句尾急降 (Falling) | 高 (粗糙感/嘶吼) |
| **乞求/畏惧 (To Plead)** | 高 (紧绷) | 窄 (压抑) 或 宽 (失控) | 快且不规则 | 起伏大 | 句尾上扬 (Rising) | 中 (颤抖/泣声) |
| **悲伤/放弃 (To Grieve)** | 低 | 极窄 (单调) | 极慢 | 低 | 平坦或微降 | 低 (无力感) |
| **安抚/诱惑 (To Soothe)** | 中低 | 适中 | 慢 | 低 (气声) | 波浪状/旋律感 | 低 (柔和) |
| **反讽/嘲弄 (To Mock)** | 变化剧烈 | 宽 (夸张) | 慢 (拖长音) | 中 | 复杂的抑扬顿挫 | 可以在特定词上加重 |
| **宣教/汇报 (To Inform)** | 中 | 适中 | 稳定/中等 | 中 | 标准陈述 | 极低 (清晰) |
| **压迫/威胁 (To Intimidate)** | 极低 | 窄 (控制感) | 慢且稳定 | 中高 | 持续平稳 | 极低 (冷酷) |

### **3.2 抽象语音合成的最佳实践（Bleeps & Bloops）**

既然用户提到“忽略人类生理限制”且适用于游戏背景，我们需要特别关注**抽象语音合成**（Abstract Voice Synthesis）。这种技术不录制真实语音，而是像《动物森友会》（Animal Crossing）、《蔚蓝》（Celeste）或《传说之下》（Undertale）那样，用短促的音效（Blips）模拟说话 20。

#### **3.2.1 频率调制与共振峰合成（Formant Synthesis）**

《蔚蓝》的开发者透露，他们并非简单播放音效，而是使用正弦波并通过均衡器（EQ）模拟人类口腔的\*\*共振峰（Formants）\*\*变化 20。

* **最佳实践：** 不要只改变音调（Pitch），要改变音色（Timbre）。  
  * *Madeline（主角）：* 波形圆润（正弦波），共振峰清晰，高频丰富，听起来年轻、充满希望。  
  * *Badeline（反派）：* 波形失真（锯齿波或方波），低频加重，听起来有攻击性和压迫感。  
  * *鬼魂/灵体：* 增加混响（Reverb）和延迟（Delay），使声音听起来空灵、遥远。

#### **3.2.2 文本驱动的音高算法**

为了让单调的“哔哔”声具有情感，必须让音高随文本内容波动。

* **算法逻辑：**  
  1. **基准音高（Base Pitch）：** 由角色身份决定（如大灰狼低音，小白兔高音）。  
  2. **情感偏移（Emotion Offset）：** 由当前的“动作动词”决定。  
     * *To Excite:* Base Pitch \* 1.5  
     * *To Depress:* Base Pitch \* 0.8  
  3. **随机抖动（Jitter）：** 这是赋予生命感的关键。  
     * *快乐/激动：* 高抖动范围（例如 ±5个半音）。这会让声音听起来跳跃、活泼。  
     * *严肃/机器人：* 零抖动。声音听起来机械、冷漠。  
     * *悲伤：* 负向微调（每个音符比前一个略低），模拟叹气。

### **3.3 SSML：语音合成标记语言的高级应用**

如果游戏使用TTS（Text-to-Speech）技术，**SSML (Speech Synthesis Markup Language)** 是控制AI配音的标准协议 24。

为了实现“有设计的对白”，不能直接将文本传给TTS引擎，必须通过“中间层”将剧本的“动作”翻译为SSML标签。

* **速度控制 (Prosody Rate):**  
  * 用于表现犹豫：\<prosody rate="-30%"\>I... I don't know.\</prosody\>  
  * 用于表现惊恐：\<prosody rate="+50%"\>We need to go now\!\</prosody\>  
* **音高控制 (Prosody Pitch):**  
  * 用于表现疑问或不确定：\<prosody pitch="+20%"\>Really?\</prosody\>  
  * 用于表现威慑：\<prosody pitch="-15%"\>Get out.\</prosody\>  
* **停顿 (Break):**  
  * 实现品特式停顿：\<break time="1500ms"/\>  
* **风格迁移 (Style Models):** Azure等现代TTS支持 \<mstts:express-as style="shouting"\> 或 \<mstts:express-as style="whispering"\>，这比单纯调整参数更自然 25。

## ---

**4\. 视觉维度：动态排版（Kinetic Typography）与语义动画**

由于用户提到“以字幕方式呈现”，视觉层面的“表达投放”与听觉同等重要。动态排版（Kinetic Typography）是指让文字本身产生运动、形变和颜色变化，以此来传达语气、音量和情绪 27。

在游戏中，文字不仅是信息的载体，更是**演员的肢体语言**。

### **4.1 视觉效果的语义分类学**

通过分析《蔚蓝》、《Undertale》等游戏的最佳实践，我们可以建立一套标准的视觉语义库 30。

**表4.1：动态排版效果与戏剧语义对照表**

| 视觉效果 (Effect) | 运动描述 (Motion) | 对应戏剧动作/状态 (Acting Equivalent) | 适用场景 |
| :---- | :---- | :---- | :---- |
| **颤抖 (Shake)** | 字符在原位进行随机的高频小幅位移。 | **高唤醒度/失控**：愤怒（大喊）、恐惧（颤抖）、寒冷。 | 角色咆哮、遭遇怪物、极度恐慌。 |
| **波浪 (Wave)** | 字符按正弦波进行上下浮动。 | **不稳定性/非现实**：反讽（阴阳怪气）、醉酒、眩晕、甚至“歌唱”感。 | 魅魔的诱惑、幽灵的低语、嘲讽的语气。 |
| **脉冲/缩放 (Pulse)** | 字符周期性变大变小，或特定词突然放大。 | **重音/强调 (Stress)**：强调某个词，如同演员加重语气。 | 关键词提示、命令句、心脏跳动声。 |
| **故障 (Glitch)** | 字符随机替换为乱码，或发生位移撕裂。 | **认知崩溃/系统错误**：精神错乱、被黑客攻击、极度压抑后的爆发。 | 赛博朋克背景、SAN值归零。 |
| **淡入/幽灵 (Ghost)** | 字符透明度低，带有模糊或拖尾。 | **气声/弱势**：耳语、内心独白、濒死状态。 | 潜行场景、回忆片段。 |
| **瀑布/下坠 (Cascade)** | 字符出现后缓慢向下位移或“滴落”。 | **疲惫/绝望**：放弃抵抗、流泪、流血。 | 战败后、悲伤的告别。 |

### **4.2 超越生理限制的视觉表达**

由于不需要遵循人类发声器官的限制，游戏文本可以创造“超自然表达”：

* **多重语调叠加：** 文本可以同时应用“火焰特效”和“冰冻颜色”，表达一种“冰冷的怒火”——这是人类声音很难同时做到的。  
* **爆发式文本（Explosive Text）：** 一个单词可以瞬间占满整个屏幕，持续0.1秒后消失。这模拟了极高分贝的冲击波。  
* **书写速度的戏剧性：**  
  * *犹豫：* 字......字......字。（极慢，伴随停顿）  
  * *崩溃：* 字字字字字字！（瞬间喷涌而出，速度超过阅读极限，营造混乱感）。

### **4.3 具体的引擎实现逻辑（Unity/Unreal）**

在技术实现上，这通常依赖于\*\*富文本标签（Rich Text Tags）\*\*解析系统 33。

* **数据结构：** 文本不仅仅是 string，而是包含元数据的 struct。  
* **顶点动画（Vertex Animation）：** 在渲染层（如Unity的TextMeshPro），通过修改字符网格的顶点位置（Vertices）来实现Shake或Wave，而不是移动整个GameObject。这保证了性能，且允许每个字母独立运动。  
* **标签嵌套：** 支持 \<shake\>\<color=red\>GET OUT\</color\>\</shake\> 这样的嵌套，意味着“愤怒且危险地颤抖”。

## ---

**5\. 导演引擎：基于LLM的自动化管线架构**

这是本研究的核心产出：如何利用AI将上述所有理论自动化。传统的做法需要策划手动在文本中插入 \<shake\> 或 \<break\> 标签，工作量巨大且难以维护。我们利用LLM（如GPT-4）作为“虚拟导演”，对原始文本进行语义标注。

### **5.1 提示词工程（Prompt Engineering）：思维链（Chain of Thought）**

要让AI像导演一样思考，不能只给它文本，必须给它**上下文**和**分析方法**。我们需要使用“思维链”技术，引导AI先分析潜台词，再生成投放指令 35。

**最佳实践：构建“导演Agent”的Prompt模板**

Role: 你是一位世界顶级的戏剧导演和计算语言学家。  
Task: 分析以下剧本对白。你的目标是将单调的文本转化为包含丰富演出指令的JSON数据。  
Input Context:  
Input Text: "我不需要你的道歉。我只需要你离开。"  
**Reasoning Steps (CoT):**

1. **Analyze Subtext (潜台词分析):** 角色B表面冷静，但内心可能压抑着痛苦。这句话不是简单的陈述，而是拒绝和防御。  
2. **Identify Beat (节拍划分):** 这句话有两个节拍。  
   * 节拍1：“我不需要你的道歉” \-\> 动作：To Reject (拒绝/冷漠)。  
   * 节拍2：“我只需要你离开” \-\> 动作：To Command (命令/驱逐)。  
3. **Assign Verb (指派动词):** 节拍1用“冷漠/疲惫”；节拍2用“决绝”。  
4. **Map to Parameters (参数映射):**  
   * 节拍1：语速慢，音调平，视觉效果为“冷色调”。  
   * 节拍2：语速正常，重音在“离开”，视觉效果“轻微震动”。

**Output Requirement:** 输出严格的JSON格式，包含 SSML标签（用于音频）和 Kinetic Tags（用于视觉）。

### **5.2 数据结构设计：标准化JSON输出**

AI的输出必须是机器可读的结构化数据 37。以下是为游戏引擎设计的推荐JSON Schema：

**表5.1：对白投放元数据结构 (Dialogue Delivery Schema)**

| 字段 (Key) | 类型 (Type) | 说明 (Description) | 示例值 (Example) |
| :---- | :---- | :---- | :---- |
| line\_id | String | 唯一标识符 | "sc01\_dial\_05" |
| speaker | String | 说话者 | "Hero" |
| segments | Array | 分段数组，用于处理节拍转换 | \[{...}, {...}\] |
| segments\[i\].text | String | 该片段的文本内容 | "我不需要你的道歉。" |
| segments\[i\].action | String | 戏剧动作动词 | "To Reject" |
| segments\[i\].emotion | String | 情感基调 | "Cold/Tired" |
| segments\[i\].audio | Object | 音频参数 (F0, Rate, Vol) | {"pitch": 0.8, "rate": 0.7, "vol": 0.5, "jitter": 0.0} |
| segments\[i\].visual | Object | 视觉参数 (Effect, Color) | {"effect": "fade", "color": "\#AABCCF", "speed": 0.05} |
| segments\[i\].pause | Float | 片段后的停顿时间(秒) | 0.8 |

### **5.3 幻觉控制与一致性管理**

AI可能会产生“幻觉”，例如创造引擎不支持的标签（如 \<explode\_text\>）。

* **约束策略：** 在System Prompt中明确列出“允许的标签白名单”（Allowed Tags Whitelist）。  
* **少样本学习（Few-Shot Learning）：** 在Prompt中提供3-5个完美的JSON转换示例，AI会极高精度地模仿这种格式 38。

## ---

**6\. 综合实施：从文本到演出的完整管线**

结合上述所有研究，我们为您的游戏项目提出以下具体的实施路径：

### **步骤一：建立“动作-参数”数据库**

不要为每一句话单独调参。建立一个名为 DeliveryStyles 的配置表（ScriptableObject/DataAsset）。

* 定义 10-20 个核心动作（如：*Threaten, Beg, Mock, Whisper, Announce*）。  
* 为每个动作预设好：音频曲线（Pitch/Speed）、视觉效果（Shake/Wave/Color）。

### **步骤二：AI 预处理 (Pre-production)**

使用 Python 脚本调用 OpenAI API 或本地 LLaMA 模型。

* 输入：您的原始剧本文件（Excel/CSV）。  
* 处理：AI 逐行分析，根据上下文填入 action\_verb 和 beat\_break 信息。  
* 输出：带有标记的增强剧本（Enhanced Script）。

### **步骤三：运行时渲染 (Runtime Engine)**

在游戏引擎（Unity/Unreal）中编写“导演控制器”（Director Controller）。

* **文本解析器：** 读取增强剧本，识别 \<beat\> 标签。  
* **状态机：** 当解析器遇到新节拍时，从 DeliveryStyles 数据库中读取对应参数，实时修改 Typewriter 的速度和 AudioSource 的音高。  
* **表现层：** 文字逐字出现，伴随着基于动作动词的特定动态效果和音效。

## **7\. 结语**

本报告通过整合戏剧导演理论、心理声学和现代AI技术，构建了一套完整的“对白表达投放”方法论。其核心在于：**拒绝平庸的字面意思，拥抱潜台词的物理化表达。**

通过让AI识别“动作动词”，让音频系统模拟“情感韵律”，让文字排版拥有“肢体语言”，您可以将枯燥的文本信息转化为具有深层情感冲击力的游戏演出。这不仅解决了成本问题（无需全语音），更利用了数字媒介的特性，创造出一种超越真实人类表演的、高度风格化的艺术形式。这是“数字戏剧”对传统表演艺术的继承与超越。

---

参考文献引用说明：  
本报告中引用的理论、数据及案例均基于以下研究片段：  
1 戏剧节拍与目标分析；  
3 动作化（Actioning）理论与动词列表；  
6 品特与马梅特的沉默及节奏理论；  
14 情感语音合成与声学参数映射；  
20 《蔚蓝》、《动物森友会》的抽象语音实现；  
30 动态排版与视觉语义；  
35 LLM思维链与JSON结构化输出。

#### **引用的著作**

1. Line delivery exercises | Open Forum \- CALLBOARD, 存取日期：1月 18, 2026， [https://community.schooltheatre.org/discussion/line-delivery-exercises](https://community.schooltheatre.org/discussion/line-delivery-exercises)  
2. Understanding Beats Analysis for Scripts \- Dramatics Magazine, 存取日期：1月 18, 2026， [https://dramatics.org/beat-analysis/](https://dramatics.org/beat-analysis/)  
3. Actioning in Acting: A Full Guide to the Technique \- Backstage, 存取日期：1月 18, 2026， [https://www.backstage.com/magazine/article/actioning-acting-explained-75443/](https://www.backstage.com/magazine/article/actioning-acting-explained-75443/)  
4. ACTION LISTS | Acting Studio Chicago, 存取日期：1月 18, 2026， [https://www.actingstudiochicago.com/action-lists/](https://www.actingstudiochicago.com/action-lists/)  
5. The Ultimate List of Acting Verbs (Tactics) \- BYU Theatre Education, 存取日期：1月 18, 2026， [https://tedb.byu.edu/0000018b-0566-df82-a5cf-3d67c2360000/pdf-lesson-3-tactic-list-pdf](https://tedb.byu.edu/0000018b-0566-df82-a5cf-3d67c2360000/pdf-lesson-3-tactic-list-pdf)  
6. The Theatre of Harold Pinter 9781408175316, 9781408175309, 9781408175293, 9781408175330 \- DOKUMEN.PUB, 存取日期：1月 18, 2026， [https://dokumen.pub/the-theatre-of-harold-pinter-9781408175316-9781408175309-9781408175293-9781408175330.html](https://dokumen.pub/the-theatre-of-harold-pinter-9781408175316-9781408175309-9781408175293-9781408175330.html)  
7. Pinteresque Dialogue \- Biblioteka Nauki, 存取日期：1月 18, 2026， [https://bibliotekanauki.pl/articles/641693.pdf](https://bibliotekanauki.pl/articles/641693.pdf)  
8. Learning from Great Writers: Harold Pinter x the Dramatic Pause | by Will Ellington \- Medium, 存取日期：1月 18, 2026， [https://willellington.medium.com/learning-from-great-writers-harold-pinter-and-the-famous-pinteresque-pause-75f1450afec0](https://willellington.medium.com/learning-from-great-writers-harold-pinter-and-the-famous-pinteresque-pause-75f1450afec0)  
9. LANGUAGE AS DRAMATIC ACTION \- A STUDY OF FIVE PLAYS BY DAVID MAMET ANNE MARGARET DEAN A Thesis presented to the University of Lo \- CORE, 存取日期：1月 18, 2026， [https://core.ac.uk/download/pdf/78865739.pdf](https://core.ac.uk/download/pdf/78865739.pdf)  
10. "Wag the Dog", production draft, by David Mamet \- Daily Script, 存取日期：1月 18, 2026， [https://www.dailyscript.com/scripts/wag-the-dog\_production.html](https://www.dailyscript.com/scripts/wag-the-dog_production.html)  
11. Top 10 Ideas from Directing Actors \- Judith Weston, 存取日期：1月 18, 2026， [https://judithweston.com/web/archive/top-10-ideas-directing-actors](https://judithweston.com/web/archive/top-10-ideas-directing-actors)  
12. brycehowitson/SSML-prosody-library: A collection of pre ... \- GitHub, 存取日期：1月 18, 2026， [https://github.com/brycehowitson/SSML-prosody-library](https://github.com/brycehowitson/SSML-prosody-library)  
13. Irony Man: Augmenting a Social Robot with the Ability to Use Irony in Multimodal Communication with Humans \- IFAAMAS, 存取日期：1月 18, 2026， [https://www.ifaamas.org/Proceedings/aamas2019/pdfs/p86.pdf](https://www.ifaamas.org/Proceedings/aamas2019/pdfs/p86.pdf)  
14. Perception of affective and linguistic prosody: an ALE meta-analysis of neuroimaging studies \- PMC \- NIH, 存取日期：1月 18, 2026， [https://pmc.ncbi.nlm.nih.gov/articles/PMC4158374/](https://pmc.ncbi.nlm.nih.gov/articles/PMC4158374/)  
15. Prosody (linguistics) \- Wikipedia, 存取日期：1月 18, 2026， [https://en.wikipedia.org/wiki/Prosody\_(linguistics)](https://en.wikipedia.org/wiki/Prosody_\(linguistics\))  
16. The Sound of Emotional Prosody: Nearly 3 Decades of Research and Future Directions, 存取日期：1月 18, 2026， [https://pmc.ncbi.nlm.nih.gov/articles/PMC12231869/](https://pmc.ncbi.nlm.nih.gov/articles/PMC12231869/)  
17. Exploring Prosodic Features Modelling for Secondary Emotions Needed for Empathetic Speech Synthesis \- Preprints.org, 存取日期：1月 18, 2026， [https://www.preprints.org/manuscript/202301.0008/v1/download](https://www.preprints.org/manuscript/202301.0008/v1/download)  
18. Average of source parameters and duration of different emotions estimated from speech of the whole text \- ResearchGate, 存取日期：1月 18, 2026， [https://www.researchgate.net/figure/Average-of-source-parameters-and-duration-of-different-emotions-estimated-from-speech-of\_tbl1\_221491825](https://www.researchgate.net/figure/Average-of-source-parameters-and-duration-of-different-emotions-estimated-from-speech-of_tbl1_221491825)  
19. Prosodic Parameters in Emotional Speech \- ISCA Archive, 存取日期：1月 18, 2026， [https://www.isca-archive.org/icslp\_1998/koike98\_icslp.pdf](https://www.isca-archive.org/icslp_1998/koike98_icslp.pdf)  
20. Devs share insight into Celeste's dialogue design process \- Shacknews, 存取日期：1月 18, 2026， [https://www.shacknews.com/article/125697/devs-share-insight-into-celestes-dialogue-design-process](https://www.shacknews.com/article/125697/devs-share-insight-into-celestes-dialogue-design-process)  
21. I Tried Recreating the Animal Crossing Talking Sound Effects in New Horizons \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=4W57Wy6veUM](https://www.youtube.com/watch?v=4W57Wy6veUM)  
22. How to Create Dialogue Audio like in Celeste and Animal Crossing using Unity \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=P3FcXHEai\_E](https://www.youtube.com/watch?v=P3FcXHEai_E)  
23. Any tips for recreating gibberish text sound (like in Celeste or Undertale)? \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/GameAudio/comments/iet3ud/any\_tips\_for\_recreating\_gibberish\_text\_sound\_like/](https://www.reddit.com/r/GameAudio/comments/iet3ud/any_tips_for_recreating_gibberish_text_sound_like/)  
24. Supported SSML tags \- Amazon Polly \- AWS Documentation, 存取日期：1月 18, 2026， [https://docs.aws.amazon.com/polly/latest/dg/supportedtags.html](https://docs.aws.amazon.com/polly/latest/dg/supportedtags.html)  
25. Voice and sound with Speech Synthesis Markup Language (SSML) \- Microsoft Learn, 存取日期：1月 18, 2026， [https://learn.microsoft.com/en-us/azure/ai-services/speech-service/speech-synthesis-markup-voice](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/speech-synthesis-markup-voice)  
26. Speech Synthesis Markup Language (SSML) Version 1.1 \- W3C, 存取日期：1月 18, 2026， [https://www.w3.org/TR/speech-synthesis11/](https://www.w3.org/TR/speech-synthesis11/)  
27. 存取日期：1月 18, 2026， [https://localo.com/marketing-dictionary/what-is-kinetic-typography\#:\~:text=Kinetic%20Typography%20%2D%20is%20an%20animation,and%20emotion%20to%20written%20words.](https://localo.com/marketing-dictionary/what-is-kinetic-typography#:~:text=Kinetic%20Typography%20%2D%20is%20an%20animation,and%20emotion%20to%20written%20words.)  
28. Kinetic typography \- Wikipedia, 存取日期：1月 18, 2026， [https://en.wikipedia.org/wiki/Kinetic\_typography](https://en.wikipedia.org/wiki/Kinetic_typography)  
29. Deep Dive: Animating Text \- Toyful Games, 存取日期：1月 18, 2026， [https://www.toyfulgames.com/blog/animating-text](https://www.toyfulgames.com/blog/animating-text)  
30. Celeste : The philosophy of game design, a study of the prologue, 存取日期：1月 18, 2026， [https://www.pointnthink.fr/en/celeste-philosophy/](https://www.pointnthink.fr/en/celeste-philosophy/)  
31. Vibrating/Trembling Text | RPG Maker Forums, 存取日期：1月 18, 2026， [https://forums.rpgmakerweb.com/index.php?threads/vibrating-trembling-text.91984/](https://forums.rpgmakerweb.com/index.php?threads/vibrating-trembling-text.91984/)  
32. Semantics | Jetpack Compose \- Android Developers, 存取日期：1月 18, 2026， [https://developer.android.com/develop/ui/compose/accessibility/semantics](https://developer.android.com/develop/ui/compose/accessibility/semantics)  
33. Text — Ren'Py Documentation, 存取日期：1月 18, 2026， [https://www.renpy.org/doc/html/text.html](https://www.renpy.org/doc/html/text.html)  
34. ink/Documentation/WritingWithInk.md at master · inkle/ink \- GitHub, 存取日期：1月 18, 2026， [https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md](https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md)  
35. How Chain of Thought (CoT) Prompting Helps LLMs Reason More Like Humans | Splunk, 存取日期：1月 18, 2026， [https://www.splunk.com/en\_us/blog/learn/chain-of-thought-cot-prompting.html](https://www.splunk.com/en_us/blog/learn/chain-of-thought-cot-prompting.html)  
36. Chain of Thought Prompting: A Deep Dive into the AI Architecture Pattern \- Rahul Krishnan, 存取日期：1月 18, 2026， [https://solutionsarchitecture.medium.com/chain-of-thought-prompting-a-deep-dive-into-the-ai-architecture-pattern-d35cd8b52c53](https://solutionsarchitecture.medium.com/chain-of-thought-prompting-a-deep-dive-into-the-ai-architecture-pattern-d35cd8b52c53)  
37. Crafting Structured {JSON} Responses: Ensuring Consistent Output from any LLM \- DEV Community, 存取日期：1月 18, 2026， [https://dev.to/rishabdugar/crafting-structured-json-responses-ensuring-consistent-output-from-any-llm-l9h](https://dev.to/rishabdugar/crafting-structured-json-responses-ensuring-consistent-output-from-any-llm-l9h)  
38. Prompt and settings for Story generation using LLMs : r/LocalLLaMA \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/LocalLLaMA/comments/1fbggqv/prompt\_and\_settings\_for\_story\_generation\_using/](https://www.reddit.com/r/LocalLLaMA/comments/1fbggqv/prompt_and_settings_for_story_generation_using/)  
39. Tips for generative AI large language model (LLM) prompt patterns \- Red Hat, 存取日期：1月 18, 2026， [https://www.redhat.com/en/blog/tips-for-gen-ai-prompts](https://www.redhat.com/en/blog/tips-for-gen-ai-prompts)