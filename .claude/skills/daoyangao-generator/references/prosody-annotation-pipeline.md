# **基于计算副语言学的叙事性语音合成全流程控制框架：从文本分析到声学实现的客观准则**

## **摘要**

在现代语音合成（TTS）与数字娱乐产业中，如何将静态的剧本转换为具有高度表现力、符合叙事逻辑且角色性格鲜明的语音表演，是计算语言学与声学信号处理领域的核心挑战。传统的端到端（End-to-End）TTS模型往往受限于“平均化”的训练数据，导致生成的语音缺乏对特定戏剧语境的精细控制。本文档构建了一套详尽的、可复用的、工业级的处理流水线框架，旨在通过深度解析“对话文本”、“故事设定书”与“人物设定集”三维输入，实现对\*\*停顿（Pause）、语速（Speech Rate）、情绪档位（Emotional Intensity）、句尾收束（Sentence Closure）及强调词（Emphasis）\*\*这五个关键副语言维度的确定性控制。

本报告综合了学术界关于情感计算、声学语音学、心理语言学的最新研究成果1，以及工业界在SSML（语音合成标记语言）与DSP（数字信号处理）方面的最佳实践4，提出了一套基于“导演代理（Director Agent）”与“声学渲染引擎（Acoustic Renderer）”解耦的自动化作业标准。报告全文约15,000字，旨在为音频工程师、计算语言学家及游戏/动画制作流程设计者提供理论支撑与实操指南。

## ---

**第一章 引言：从文本到表演的参数化重构**

### **1.1 问题背景：语义与语用的鸿沟**

在自然语言处理（NLP）与语音合成的交叉领域，核心痛点始终在于“语义（Semantic）”与“语用（Pragmatic）”的断裂。文本只能承载语义信息（说什么），而语用信息（怎么说、为什么说、在什么情境下说）则隐含在文本之外的“潜台词”与“设定”中。对于一个拥有“台词文本+故事设定书+人物设定集”的输入组合，工程系统的首要任务是将这些非结构化的背景信息“显式化”，转化为声学模型可以理解的控制向量。

研究表明，人类听觉系统对语音“自然度”的感知，并非仅仅依赖于音色的还原度，更依赖于韵律（Prosody）特征的层级结构7。当合成语音缺乏对停顿、语速、语调的动态调节时，即便是最先进的神经声码器（如HiFi-GAN, WaveNet）生成的语音也会产生“恐怖谷”效应——听起来像人，但缺乏灵魂。

### **1.2 五维控制向量的声学定义**

为了实现“流水线化”的工业生产，必须将原本属于表演艺术范畴的主观描述转化为客观的物理参量。本文将任务书中的五个控制点定义为以下声学-语言学向量：

1. **停顿（Pause / Break Index）：** 对应ToBI（Tones and Break Indices）系统中的边界指数，物理量为静音时长（Duration of Silence, ms），反映认知负荷与句法结构9。  
2. **语速快慢（Speech Rate / Tempo）：** 对应音节速率（Syllables Per Second, SPS）及局部弹性（Elasticity），物理量为时域伸缩比（Time-stretch Ratio），反映生理唤醒度与紧迫感11。  
3. **情绪档位（Emotion Gear / Arousal）：** 对应Russell环形情感模型中的“唤醒度（Arousal）”轴，物理量为基频均值（F0 Mean）、频谱倾斜（Spectral Tilt）及微扰（Jitter/Shimmer），反映能量层级13。  
4. **句尾收束（Sentence Closure / Boundary Tone）：** 对应句末F0轨迹（Final F0 Contour），物理量为音高斜率（Pitch Slope）及声门化特征（Glottalization），反映人际权势关系与言语行为类型15。  
5. **强调词（Emphasis / Prominence）：** 对应信息焦点（Information Focus），物理量为局部F0峰值偏移（Pitch Excursion）、时长延长及能量提升，反映信息结构中的新旧信息分布16。

### **1.3 提出的流水线架构**

本报告主张建立一套“基于规则与模型混合驱动”的生产管线（Hybrid Rule-Model Pipeline）。该管线包含三个核心模组：

* **解析层（Analysis Layer）：** 利用大语言模型（LLM）深度理解故事与人物设定，生成包含五维参数的中间描述语言（Intermediate Representation）。  
* **映射层（Mapping Layer）：** 将中间描述转换为标准的SSML标签或TTS控制参数。  
* **渲染层（Rendering & Post-processing Layer）：** 结合神经TTS生成基础波形，并利用DSP技术进行微观声学修饰（如增加嘶哑感、调整呼吸声）。

## ---

**第二章 预处理：基于设定集的“导演代理”构建**

在任何音频生成之前，必须首先建立一个能够理解“上下文”的决策中心。在工业流水线中，这一角色由经过Prompt Engineering（提示工程）优化的LLM（如GPT-4o, Claude 3.5 Sonnet）担任，我们称之为“AI导演”18。

### **2.1 人物设定集的参数化映射（Character Profiling）**

“人物设定集”是语音表演的基石。不同的性格特征直接决定了声学参数的**基线值（Baseline）**。在流水线中，我们需要将文学性的性格描述转化为初始的声学配置。

#### **2.1.1 性格与声学基线的相关性研究**

社会语言学与心理声学研究指出了性格特质与语音特征的强相关性：

* **多血质/外向型（Extroverted/Sanguine）：** 设定集中描述为“活泼、自信、冲动”的角色。  
  * *声学映射：* 基础语速+10%，基础音高（F0 Mean）偏高，音高动态范围（F0 Range）宽20。  
  * *流水线配置：* 默认SSML标签 \<prosody rate="1.1" pitch="+5%"\>。  
* **抑郁质/内向型（Introverted/Melancholic）：** 设定集中描述为“沉稳、阴郁、思考者”的角色。  
  * *声学映射：* 基础语速-10%，基础音高偏低，音高动态范围窄，句尾多用降调21。  
  * *流水线配置：* 默认SSML标签 \<prosody rate="0.9" pitch="-5%"\>。  
* **权威型/高地位（Authoritative/High Status）：** 设定集中描述为“领袖、霸道总裁、神祇”。  
  * *声学映射：* 语速极其稳定（低抖动），音量较大，句尾坚决使用降调（Falling Tone），拒绝使用上扬语调（Upspeak）23。

#### **2.1.2 生理特征的声学约束**

人物设定中的生理参数（年龄、体型）对声道长度（Vocal Tract Length）有物理约束，这直接影响共振峰（Formants）频率。

* *体型巨大：* 声道长，共振峰频率低。DSP处理中应调整共振峰位移（Formant Shift \-10%）25。  
* *体型微小：* 声道短，共振峰频率高。DSP处理中应调整共振峰位移（Formant Shift \+10%）。

**表 2.1：人物设定到声学基线的映射矩阵**

| 人物特征标签 | 基础语速 (Rate) | 基础音高 (Pitch) | 停顿频率 (Pause Freq) | 句尾特征 (Ending) | 推荐音色纹理 |
| :---- | :---- | :---- | :---- | :---- | :---- |
| **热血/少年** | \+15% (Fast) | \+2st (High) | 低 (连贯) | Abrupt (短促) | Bright, Clear |
| **冷酷/杀手** | \-5% (Steady) | \-2st (Low) | 中 (控制) | Fry (气泡音) | Breathy, Tense |
| **焦虑/受害者** | \+20% (Erratic) | \+4st (High) | 极高 (破碎) | Rising (疑问) | Rough (颤抖) |
| **年迈/智者** | \-20% (Slow) | \-5st (Low) | 高 (沉思) | Falling (肯定) | Hoarse (嘶哑) |

### **2.2 故事设定书的环境场效应（Contextual Field Effect）**

“故事设定书”定义了对话发生的物理环境与心理氛围，这对全剧的“底噪”与“发声模式”产生全局影响。这被称为**环境场效应**。

#### **2.2.1 朗伯德效应（Lombard Effect）的模拟**

当故事设定发生在“战场”、“夜店”或“暴风雨”中时，人类会不自觉地提高音量、提高音高并改变频谱倾斜度，以克服环境噪音，这就是朗伯德效应26。

* *流水线规则：* 若场景环境标签为 \`\`，则强制提升全局 Volume \+3dB，提升 Pitch \+1st，并增强高频（Spectral Tilt Flattening），以模拟在嘈杂环境中的喊话状态。

#### **2.2.2 心理氛围对“情绪档位”的基准偏移**

故事的整体基调（Genre）决定了情绪的“零点”。

* *喜剧（Comedy）：* 情绪档位基准上调。即便是“中性”台词，也带有轻微的愉悦度（Gear 2.5）。  
* *正剧/悲剧（Drama/Tragedy）：* 情绪档位基准下调。强调压抑感，语速普遍放缓28。

### **2.3 LLM结构化提示工程（Structured Prompting）**

为了将上述理论转化为工程实践，我们需要设计一套结构化的提示词（System Prompt），强迫LLM输出JSON格式的控制脚本。

**最佳实践Prompt结构：**

1. **角色定义：** "你是一位精通斯坦尼斯拉夫斯基表演体系的配音导演..."29。  
2. **任务指令：** "分析输入的台词，结合人物小传与当前场景，输出五维控制参数。"28。  
3. **输出约束：** "必须输出为JSON格式，包含 keys: pause\_locations, speech\_rate\_modifier, emotion\_gear\_1\_to\_5, sentence\_ending\_type, emphasis\_keywords。"31。

## ---

**第三章 深度解析：控制点一——停顿（Pause/Stop）**

停顿是语言的呼吸，是信息处理的窗口。在语音合成中，停顿切分（Phrasing）的准确性直接决定了语音的可懂度与智能感。我们必须区分三种不同性质的停顿：**句法停顿**、**生理停顿**与**心理停顿**。

### **3.1 句法停顿：标点与语法边界的硬约束**

这是最基础的停顿类型，由文本的标点符号和句法结构决定。学术研究对标点符号对应的最佳静音时长有明确的数据支持。

#### **3.1.1 标点符号的时长标准**

根据对有声读物和新闻播报的大数据分析10：

* **逗号（,）：** 最佳时长为 **200ms \- 400ms**。这个时长足以切分意群，但不足以让听众认为话语结束（Turn-yielding）。  
* **句号（.）：** 最佳时长为 **600ms \- 1000ms**。这个时长标志着一个完整思想的结束，允许听众进行认知归档。  
* **段落间（Paragraph Break）：** 最佳时长为 **1200ms \- 1500ms**。标志着话题的转换。

#### **3.1.2 无标点处的隐式停顿（Prosodic Phrases）**

人类语言并非只有在标点处才停顿。在长难句中，为了保证语流的节奏感，需要在短语边界（Phrase Boundary）插入微停顿（Micro-pause, \<150ms）。

* *算法支持：* 现代NLP管线使用基于BERT的预测模型（Prosody Prediction Model）来预测文本中的“韵律短语边界（Prosodic Word/Phrase Boundary）”8。  
* *规则：* 任何超过10个单词（或15个汉字）的从句，必须在主谓之间或长定语后插入 \<break time="150ms"/\>。

### **3.2 生理停顿：呼吸重置（Breath Groups）**

真实的说话人受限于肺活量。为了增加“真实感”，必须模拟呼吸机制。

* **呼吸组理论（Breath Group Theory）：** 一个呼吸组通常包含4-8秒的语音内容。如果台词文本极长且无标点（如激动的独白），流水线必须强制插入呼吸停顿4。  
* **实现技巧：** 在插入长停顿（\>600ms）的同时，在音频中混入一个轻微的\*\*吸气声（Inhalation Breath）\*\*采样。这能极大地提升临场感。

### **3.3 心理停顿：犹豫与认知负荷（Hesitation & Cognitive Load）**

这是体现“演技”的关键。当人物处于“撒谎”、“回忆”或“寻找措辞”的状态时，会出现非句法的停顿34。

#### **3.3.1 认知负荷与停顿位置**

研究表明，认知负荷较高的词汇（低频词、新信息）之前，往往会出现停顿。

* *规则：* 如果LLM分析出该台词涉及“回忆”或“不确定”，则在关键词（Emphasis Word）**之前**插入一个 200ms 的停顿，甚至配合填充词（Filled Pause，如“呃...”、“这个...”）。  
  * *例子：* “凶手是…… 你的哥哥。”  
* *效果：* 这种停顿制造了悬念（Suspense），并模拟了思维过程。

**表 3.1：停顿控制策略总表**

| 停顿类型 | 触发条件 | 推荐时长 (ms) | SSML标签示例 | 心理/叙事功能 |
| :---- | :---- | :---- | :---- | :---- |
| **微停顿** | 短语边界/长句切分 | 50 \- 150 | \<break strength="weak"/\> | 维持节奏，防止气促 |
| **逗号停顿** | 逗号/分句 | 200 \- 400 | \<break time="300ms"/\> | 意群切分，逻辑连接 |
| **句号停顿** | 句号/感叹号 | 600 \- 1000 | \<break time="800ms"/\> | 思想闭合，话轮转换 |
| **思考停顿** | 关键词前/犹豫 | 300 \- 600 | \<break time="500ms"/\> | 模拟思考，制造悬念 |
| **剧变停顿** | 震惊/被打断后 | 1500+ | \<break time="1.5s"/\> | 戏剧留白，情绪发酵 |

## ---

**第四章 深度解析：控制点二——语速快慢（Speech Rate）**

语速不仅是信息传递的速率，更是情绪唤醒度（Arousal）的直接体现。在流水线中，我们必须通过全局语速（Global Rate）与局部弹性（Local Elasticity）两个层面来控制。

### **4.1 全局语速标准（Global Rate Standards）**

基于不同媒体形式的统计数据，我们可以设立明确的语速标准（Words Per Minute, WPM 或 Characters Per Minute, CPM）12。

* **正常叙述（Narrative/Normal）：** **140 \- 160 WPM**（英文）或 **220 \- 260 CPM**（中文）。这是有声读物和新闻的标准语速，清晰且舒适。  
* **慢速/悲伤/沉思（Slow）：** **100 \- 120 WPM**。当角色处于悲伤、疲惫或进行郑重宣告时使用。研究显示，慢语速与低唤醒度情绪（悲伤、温柔）强相关22。  
* **快速/愤怒/恐惧（Fast）：** **180 \- 200 WPM**。当角色处于愤怒、恐惧或极度兴奋时使用。高唤醒度情绪必然伴随肌肉紧张度的增加和语速的加快11。

### **4.2 局部语速弹性：等时性与Rubato（Isochrony & Rubato）**

机械音之所以机械，是因为它过于匀速。人类语言具有\*\*Rubato（自由节奏）\*\*特性。

#### **4.2.1 功能词压缩与实词延展**

语言学中的“重音节拍（Stress-timed）”理论（针对英语）或“音节节拍（Syllable-timed）”理论（针对中文）都指出，并非所有字的时长都相等。

* **功能词（Function Words）：** 介词、助词（的、了、着）、冠词。这些词在自然语流中会被**压缩**。  
  * *操作：* 设置 \<prosody rate="+20%"\>。  
* **实词（Content Words）：** 名词、动词、形容词。尤其是被强调的实词，会被**拉长**。  
  * *操作：* 设置 \<prosody rate="-10%"\>。

#### **4.2.2 句末减速（Phrase-final Lengthening）**

人类在说一个句子的最后几个音节时，往往会下意识地减速，这被称为“句末延长现象”。这是提示句子即将结束的重要韵律线索8。

* *最佳实践：* 在句子的最后2-3个字，应用线性减速处理，语速逐渐降低至原速的80%。

### **4.3 语速与情绪的动态耦合**

语速不能独立调整，必须与情绪档位联动。

* **公式：** $Target\\\_Rate \= Base\\\_Rate \\times (1 \+ \\alpha \\times Arousal\\\_Level)$  
  * 其中 $\\alpha$ 为系数。愤怒时 $\\alpha$ 为正，悲伤时 $\\alpha$ 为负。  
* **例外：** “冷怒（Cold Anger）”。这是一种特殊的情绪状态，虽然是愤怒（高唤醒），但为了压抑爆发，语速反而会变慢，每个字都咬得很重（Staccato，断奏）。这需要特殊的规则处理：Emotion=Anger AND Type=Cold \-\> Rate=Slow AND Articulation=Crisp.38

## ---

**第五章 深度解析：控制点三——情绪档位（Emotion Gear）**

“情绪”是一个模糊的概念，在工程上难以量化。为了实现客观复用，我们引入\*\*情绪档位（Emotion Gears 1-5）\*\*系统，该系统基于Russell的情感环形模型（Circumplex Model），主要沿“唤醒度（Arousal）”轴进行分级，同时辅以“效价（Valence）”轴的修饰13。

### **5.1 五档情绪系统的声学定义**

我们定义5个标准档位，每个档位对应一组明确的声学参数配置（基频F0、音量Intensity、音色Timbre）。

| 档位 (Gear) | 定义与场景 | 基频 (Pitch) | 音量 (Volume) | 语速 (Rate) | 音色特征 (Voice Quality) |
| :---- | :---- | :---- | :---- | :---- | :---- |
| **Gear 1** | **压抑/极低唤醒** 耳语、虚弱、绝望 | \-20% (-4st) | \-6dB | \-20% | **气声 (Breathy)** 高频噪声增加，低HNR值 |
| **Gear 2** | **中性/叙述** 日常对话、旁白 | Baseline | 0dB | Baseline | **模态声 (Modal)** 清晰，周期性好 |
| **Gear 3** | **投入/关注** 辩论、命令、愉悦 | \+10% (+2st) | \+1.5dB | \+5% | **明亮 (Bright)** 高频共振峰增强 |
| **Gear 4** | **高唤醒/激动** 争吵、惊吓、狂喜 | \+25% (+4st) | \+3dB | \+15% | **紧张 (Tense/Pressed)** 声带闭合紧，频谱平坦 |
| **Gear 5** | **极端/失控** 尖叫、痛哭、狂怒 | \+40% (+6st) | \+6dB | 极快或极慢 | **糙声 (Rough)** 加入Jitter/Shimmer，饱和失真 |

14

### **5.2 关键技术：音色纹理的合成（Roughness & Breathiness）**

传统的TTS往往只能改变音高和音量，难以模拟Gear 1的气声和Gear 5的嘶吼。为了实现这五个点，必须引入\*\*Estill Voice Training（EVT）\*\*理论中的音质控制，并在后期通过DSP实现43。

#### **5.2.1 糙声（Roughness）与愤怒模拟**

研究表明，人类在极度愤怒或恐惧时，发声系统会出现非线性振动，导致次谐波（Subharmonics）和混沌振动。在声学上表现为\*\*Jitter（频率微扰）**和**Shimmer（振幅微扰）\*\*的增加45。

* *实现方法：* 对于Gear 5的台词，在合成后，使用信号处理算法（如Python的Parselmouth库调用Praat算法）向音频中注入随机的Jitter（周期性抖动）。  
  * *参数参考：* Jitter \> 1%, Shimmer \> 5% 41。  
  * *代码逻辑：* apply\_jitter(audio\_signal, intensity=0.02)。

#### **5.2.2 气声（Breathiness）与悲伤/耳语模拟**

悲伤或私密对话时，声门闭合不全，气流湍流噪音增加。

* *实现方法：* 降低**谐噪比（Harmonics-to-Noise Ratio, HNR）**。  
* *DSP技巧：* 将原始音频与通过高通滤波器（High-pass Filter, \>2kHz）的白噪声进行混合。

### **5.3 情绪的动态包络**

情绪不是静态的，它有\*\*ADSR（Attack, Decay, Sustain, Release）\*\*包络。

* *突发情绪（Shock）：* Attack极快。台词开头的第一个字就需要极高的能量。  
* *酝酿情绪（Grief）：* Attack慢。情绪强度随句子推进逐渐累积。  
* *流水线控制：* LLM需要标记情绪的包络类型，指导TTS引擎在句首或句尾施加不同的Intensity Curve。

## ---

**第六章 深度解析：控制点四——句尾收束（Sentence Closure）**

句尾的处理（Boundary Tones）承载了极高密度的语用信息。它决定了这句话是陈述、疑问、命令还是未尽之言。同时，它也是角色社会地位（Status）的重要标识。

### **6.1 三种核心语调轮廓（Intonation Contours）**

#### **6.1.1 降调收束（Falling Tone, L-L%）**

* *声学特征：* 基频在最后一个音节显著下降，直至说话人的音域底限。  
* *语义功能：* 确定性、终结、命令、陈述事实。  
* *角色关联：* **高地位角色**（权威者）倾向于使用全降调（Full Fall）。

#### **6.1.2 升调收束（Rising Tone, H-H% / L-H%）**

* *声学特征：* 基频在句末上扬。  
* *语义功能：* 一般疑问（Yes/No Question）、不确定性、寻求确认。  
* *特殊现象——高升语调（High Rising Terminal / Upspeak）：* 在陈述句末尾使用升调。这通常被视为**低地位**、缺乏自信或寻求认同的表现（常见于年轻角色或服务人员）47。在人物设定为“缺乏自信”时，流水线应强制应用此模式。

#### **6.1.3 平调/悬置收束（Sustained Tone, H- / L-）**

* *声学特征：* 基频保持平直，不降不升。  
* *语义功能：* 话没说完（被打断）、列表列举中、暗示言外之意。

### **6.2 高级技巧：句末的声质变化**

除了音高变化，句尾的音质处理是提升真实感的秘诀。

#### **6.2.1 气泡音/嘎裂声（Vocal Fry / Creaky Voice）**

现代英语（及部分汉语方言）中，自然放松的陈述句末尾，常伴随着Vocal Fry。这是声带松弛、声门下压降低的自然结果。

* *应用场景：* 放松的、慵懒的、酷的、甚至厌倦的角色。Gear 1 和 Gear 2 的句尾推荐使用。  
* *实现：* 通过风格迁移（Style Transfer）模型，将句尾音素替换为带有Creaky特征的音素；或在DSP阶段进行降频调制49。  
* *效果：* 消除合成语音的“过度完美感”，增加生活气息。

#### **6.2.2 喉塞音截断（Glottal Stop / Abrupt Cut）**

当台词被设定为“被打断”或“急促”时，不能让TTS自然衰减（Fade out）。

* *声学操作：* 在波形过零点（Zero-crossing）处进行硬切（Hard Cut），并在切断前插入一个短暂的静音闭锁（Glottal Stop, ʔ）。  
* *标签：* \<break strength="none"/\> 配合音频波形截断51。

## ---

**第七章 深度解析：控制点五——强调词（Emphasis）**

强调词是句子的逻辑重音所在，它指示了“新信息（New Information）”。没有正确的强调，机器语音就会像念经一样平铺直叙。

### **7.1 强调词的自动化识别：TF-IDF与信息结构**

在流水线中，不能依靠人工标注强调词。我们利用NLP算法自动识别。

* **TF-IDF（词频-逆文档频率）算法：** 在当前对话中，出现频率低、且在语料库中也具有高信息量的词，往往是强调词16。  
* **WH-问答匹配：** 分析上一句的潜在问题。如果上一句是“谁偷了车？”，当前句“**我**偷了车”，则“我”是强调词。如果上一句是“你偷了什么？”，则“**车**”是强调词。LLM非常擅长这种依存关系分析。

### **7.2 强调的声学实现：凸显凸峰（Prominence Hump）**

强调不仅仅是大声。声学实验证明，强调是音高、时长、音量的综合作用，其中**音高变化最重要**17。

* **三位一体控制法：**  
  1. **音高突变（Pitch Excursion）：** 强调词的重读音节F0瞬间抬高（或在低沉情绪中压低）。通常偏移量需达到 **\+3到+5个半音（Semitones）**。  
  2. **时长拉伸（Duration Stretch）：** 强调词的时长延长 **1.2倍到1.5倍**。这给了听众更多的时间去处理这个核心信息。  
  3. **音量提升（Intensity Boost）：** 局部音量提升 **\+2dB到+4dB**。  
* **频谱变化：** 强调词发音更清晰，高频能量更足（Spectral Tilt变平）。  
* **SSML实现最佳实践：**  
  XML  
  I did not \<emphasis level\="strong"\> \<prosody pitch\="+15%" rate\="-10%" volume\="+3dB"\> steal \</prosody\> \</emphasis\> the car.

  注意：在强调词*之后*，往往伴随一个极短的微停顿，且后续词汇的音高和音量会迅速衰减（Post-focus Compression），以反衬强调词的突出地位55。

## ---

**第八章 工程实现：可复用的流水线架构**

基于上述五个深度的控制点，我们构建如下的自动化生产管线。

### **8.1 架构图示**

Raw Text \+ Character Bible \+ Scene Context  
⬇️  
Step 1: NLP Analysis Node (LLM Director)  
(利用Prompt提取五维参数，输出JSON)  
⬇️  
Step 2: Parameter Mapping Node (Rule Engine)  
(将JSON映射为SSML及DSP指令)  
⬇️  
Step 3: Synthesis Node (Neural TTS)  
(生成基础音频 Wav)  
⬇️  
Step 4: DSP Post-Processing Node (Acoustic Refiner)  
(应用Jitter, Breathiness, Glottal Stop等特效)  
⬇️  
Final Expressive Audio

### **8.2 第一步：LLM导演的结构化输出**

我们要求LLM输出如下严的JSON结构32：

JSON

{  
  "dialogue\_id": "L1024",  
  "text": "住手！这根本不是你要找的东西。",  
  "context\_analysis": {  
    "emotion\_gear": 4, // 情绪档位：激动  
    "role\_status": "High",  
    "intent": "Stop action"  
  },  
  "prosody\_controls": {  
    "pause\_points": \[ // 控制点1：停顿  
      {"after\_word": "住手", "duration\_ms": 600, "type": "dramatic"}  
    \],  
    "global\_rate": 1.15, // 控制点2：语速（+15%）  
    "sentence\_closure": { // 控制点4：收束  
      "type": "Falling\_Abrupt", // 降调且短促  
      "glottal\_stop": true  
    },  
    "emphasis\_tokens": \[ // 控制点5：强调词  
      {"word": "不是", "pitch\_mod": "+20%", "duration\_mod": "1.3x"}  
    \]  
  },  
  "timbre\_modifiers": { // 控制点3：情绪音色细节  
    "roughness\_jitter": 0.015, // 增加糙声  
    "breathiness\_mix": 0.0 // 无气声  
  }  
}

### **8.3 第二步：SSML生成策略**

脚本将JSON转换为TTS引擎可读的SSML。

* 利用 \<break\> 标签控制停顿。  
* 利用 \<prosody\> 嵌套标签控制全局语速与局部强调。  
* 利用 \<say-as\> 或自定义标签标记句尾风格，供后续模型识别。

### **8.4 第三步：DSP后处理（Python/Parselmouth）**

由于SSML无法完全控制音质（如嘶哑），必须引入Python脚本对生成的WAV进行后处理56。

* 添加糙声（Anger Simulation）：  
  使用 parselmouth.Sound 加载音频，提取Pitch Tier，叠加高斯噪声到F0轨迹上，制造Jitter效果。  
* 句尾Vocal Fry模拟：  
  检测句尾最后的元音段，使用LPC（线性预测编码）分解，将激励源（Source）的脉冲频率降低至70Hz以下（Fry区间），再合成。

## ---

**第九章 结论与展望**

本文档提出了一套基于\*\*计算副语言学（Computational Paralinguistics）\*\*的客观准则，解决了“如何处理台词文本以区分五大韵律特征”的工程难题。

核心结论如下：

1. **客观性：** 通过建立“人物性格-\>声学基线”和“情绪-\>声学参数（Gear系统）”的映射表，消除了人工指导的主观随意性。  
2. **可复用性：** LLM导演+规则引擎的架构，使得同一套逻辑可以复用于成千上万句台词，实现了工业级量产。  
3. **深度控制：** 突破了传统TTS仅控制音高音量的局限，引入了\*\*音质纹理（Roughness/Breathiness）**和**微观节奏（Rubato/Glottal Stop）\*\*的控制，极大提升了表现力。

随着多模态大模型的发展，未来的流水线将从“Text-to-Audio”进化为“Context-to-Performance”，但本文提出的这五个核心控制向量（停顿、语速、情绪、收束、强调）将始终是连接语义世界与声学世界的桥梁。

---

(本文档内引用的数据与理论均基于提供的学术研究片段 1 至 18 整理得出。)

#### **引用的著作**

1. ProsodyLM: Uncovering the Emerging Prosody Processing Capabilities in Speech Language Models | OpenReview, 存取日期：1月 19, 2026， [https://openreview.net/forum?id=uBg8PClMUu](https://openreview.net/forum?id=uBg8PClMUu)  
2. The evaluation of prosody in speech synthesis: a systematic review \- Journals, 存取日期：1月 19, 2026， [https://journals-sol.sbc.org.br/index.php/jbcs/article/view/5468](https://journals-sol.sbc.org.br/index.php/jbcs/article/view/5468)  
3. The evaluation of prosody in speech synthesis: a systematic review \- ResearchGate, 存取日期：1月 19, 2026， [https://www.researchgate.net/publication/393757577\_The\_evaluation\_of\_prosody\_in\_speech\_synthesis\_a\_systematic\_review](https://www.researchgate.net/publication/393757577_The_evaluation_of_prosody_in_speech_synthesis_a_systematic_review)  
4. Speech Synthesis Markup Language (SSML) \- Google Cloud Documentation, 存取日期：1月 19, 2026， [https://docs.cloud.google.com/text-to-speech/docs/ssml](https://docs.cloud.google.com/text-to-speech/docs/ssml)  
5. Controlling volume, speaking rate, and pitch \- Amazon Polly \- AWS Documentation, 存取日期：1月 19, 2026， [https://docs.aws.amazon.com/polly/latest/dg/prosody-tag.html](https://docs.aws.amazon.com/polly/latest/dg/prosody-tag.html)  
6. tts-basics-customize-ssml.ipynb \- nvidia-riva/tutorials \- GitHub, 存取日期：1月 19, 2026， [https://github.com/nvidia-riva/tutorials/blob/main/tts-basics-customize-ssml.ipynb](https://github.com/nvidia-riva/tutorials/blob/main/tts-basics-customize-ssml.ipynb)  
7. Voice Synthesis Improvement by Machine Learning of Natural Prosody \- MDPI, 存取日期：1月 19, 2026， [https://www.mdpi.com/1424-8220/24/5/1624](https://www.mdpi.com/1424-8220/24/5/1624)  
8. Exploiting Acoustic and Syntactic Features for Automatic Prosody Labeling in a Maximum Entropy Framework \- PMC \- NIH, 存取日期：1月 19, 2026， [https://pmc.ncbi.nlm.nih.gov/articles/PMC2709295/](https://pmc.ncbi.nlm.nih.gov/articles/PMC2709295/)  
9. Automatic detection of prosodic boundaries in spontaneous speech | PLOS One, 存取日期：1月 19, 2026， [https://journals.plos.org/plosone/article?id=10.1371/journal.pone.0250969](https://journals.plos.org/plosone/article?id=10.1371/journal.pone.0250969)  
10. Occurrence and Duration of Pauses in Relation to Speech Tempo and Structural Organization in Two Speech Genres \- MDPI, 存取日期：1月 19, 2026， [https://www.mdpi.com/2226-471X/8/1/23](https://www.mdpi.com/2226-471X/8/1/23)  
11. Measuring negative emotions and stress through acoustic correlates in speech: A systematic review \- PMC \- PubMed Central, 存取日期：1月 19, 2026， [https://pmc.ncbi.nlm.nih.gov/articles/PMC12289014/](https://pmc.ncbi.nlm.nih.gov/articles/PMC12289014/)  
12. Average Speaking Rate and Words per Minute \- VirtualSpeech, 存取日期：1月 19, 2026， [https://virtualspeech.com/blog/average-speaking-rate-words-per-minute](https://virtualspeech.com/blog/average-speaking-rate-words-per-minute)  
13. Improving Emotional TTS with an Emotion Intensity Input from Unsupervised Extraction \- Idiap Publications, 存取日期：1月 19, 2026， [https://publications.idiap.ch/downloads/papers/2021/Schnell\_SSW11\_2021.pdf](https://publications.idiap.ch/downloads/papers/2021/Schnell_SSW11_2021.pdf)  
14. Pitch Perfect: Vocal Pitch and the Emotional Intensity of Congressional Speech | American Political Science Review \- Cambridge University Press & Assessment, 存取日期：1月 19, 2026， [https://www.cambridge.org/core/journals/american-political-science-review/article/pitch-perfect-vocal-pitch-and-the-emotional-intensity-of-congressional-speech/868A227A2C57A7053742CF9CF3B6C419](https://www.cambridge.org/core/journals/american-political-science-review/article/pitch-perfect-vocal-pitch-and-the-emotional-intensity-of-congressional-speech/868A227A2C57A7053742CF9CF3B6C419)  
15. 2.4: Intonation- Statements and Questions \- Humanities LibreTexts, 存取日期：1月 19, 2026， [https://human.libretexts.org/Bookshelves/Languages/English\_as\_a\_Second\_Language/Book%3A\_Speaking\_Listening\_and\_Pronunciation\_Projects\_for\_ELLs\_-\_Intermediate\_Level\_(Zemlick)/02%3A\_Getting\_to\_Know\_You/2.04%3A\_Intonation-\_Statements\_and\_Questions](https://human.libretexts.org/Bookshelves/Languages/English_as_a_Second_Language/Book%3A_Speaking_Listening_and_Pronunciation_Projects_for_ELLs_-_Intermediate_Level_\(Zemlick\)/02%3A_Getting_to_Know_You/2.04%3A_Intonation-_Statements_and_Questions)  
16. Natural Language Processing (NLP) \[A Complete Guide\] \- DeepLearning.AI, 存取日期：1月 19, 2026， [https://www.deeplearning.ai/resources/natural-language-processing/](https://www.deeplearning.ai/resources/natural-language-processing/)  
17. Realization of stress in theatrical speech: The example of Composed Theater | The Journal of the Acoustical Society of America | AIP Publishing, 存取日期：1月 19, 2026， [https://pubs.aip.org/asa/jasa/article/135/4\_Supplement/2292/680817/Realization-of-stress-in-theatrical-speech-The](https://pubs.aip.org/asa/jasa/article/135/4_Supplement/2292/680817/Realization-of-stress-in-theatrical-speech-The)  
18. SpeechLMs: LLM-Powered Text-to-Speech and Neural Audio Codecs Explored \- Hugging Face, 存取日期：1月 19, 2026， [https://huggingface.co/spaces/Steveeeeeeen/SpeechLLM-Playbook](https://huggingface.co/spaces/Steveeeeeeen/SpeechLLM-Playbook)  
19. Agentic Workflows \- Cobus Greyling \- Medium, 存取日期：1月 19, 2026， [https://cobusgreyling.medium.com/agentic-workflows-034d2df458d3](https://cobusgreyling.medium.com/agentic-workflows-034d2df458d3)  
20. Realistic Dialogue: Creating Characters' Speech Patterns \- NowNovel, 存取日期：1月 19, 2026， [https://nownovel.com/talking-character-speech/](https://nownovel.com/talking-character-speech/)  
21. Creating Unique Voices, Mannerisms, and Appearances for Your Characters, 存取日期：1月 19, 2026， [https://www.fantasywritingschool.com/blog/creating-unique-voices-mannerisms-and-appearances-for-your-characters/](https://www.fantasywritingschool.com/blog/creating-unique-voices-mannerisms-and-appearances-for-your-characters/)  
22. Full article: The Acoustic Profiles of Emotion: Analyzing the Spoken Voice in Theater Performance \- Taylor & Francis Online, 存取日期：1月 19, 2026， [https://www.tandfonline.com/doi/full/10.1080/23268263.2025.2482495](https://www.tandfonline.com/doi/full/10.1080/23268263.2025.2482495)  
23. American English | Intonation Patterns | Blog Post \- by Jill Diamond, 存取日期：1月 19, 2026， [https://www.byjilldiamond.com/blog/american-english-intonation-patterns](https://www.byjilldiamond.com/blog/american-english-intonation-patterns)  
24. LLMs as Method Actors: A Model for Prompt Engineering and Architecture \- arXiv, 存取日期：1月 19, 2026， [https://arxiv.org/html/2411.05778v2](https://arxiv.org/html/2411.05778v2)  
25. New Parameterizations for Emotional Speech Synthesis, 存取日期：1月 19, 2026， [https://www.cs.cmu.edu/\~awb/papers/jhuw11\_npess\_final\_report.pdf](https://www.cs.cmu.edu/~awb/papers/jhuw11_npess_final_report.pdf)  
26. (PDF) GOAT-TTS: LLM-based Text-To-Speech Generation Optimized via A Dual-Branch Architecture \- ResearchGate, 存取日期：1月 19, 2026， [https://www.researchgate.net/publication/390892795\_GOAT-TTS\_LLM-based\_Text-To-Speech\_Generation\_Optimized\_via\_A\_Dual-Branch\_Architecture](https://www.researchgate.net/publication/390892795_GOAT-TTS_LLM-based_Text-To-Speech_Generation_Optimized_via_A_Dual-Branch_Architecture)  
27. Comparison of Speech Rate and Long-Term Average Speech Spectrum between Korean Clear Speech and Conversational Speech \- PMC \- NIH, 存取日期：1月 19, 2026， [https://pmc.ncbi.nlm.nih.gov/articles/PMC6773961/](https://pmc.ncbi.nlm.nih.gov/articles/PMC6773961/)  
28. How to Write an Oscar-Worthy LLM Prompt: Your Guide to the Prompt-Chaining Framework, 存取日期：1月 19, 2026， [https://engineering.gusto.com/how-to-write-an-oscar-worthy-llm-prompt-your-guide-to-the-prompt-chaining-framework-777d9d7084c6](https://engineering.gusto.com/how-to-write-an-oscar-worthy-llm-prompt-your-guide-to-the-prompt-chaining-framework-777d9d7084c6)  
29. INSANE Framework for Creating Voice AI Prompts (Prompt Engineering Guide) \- YouTube, 存取日期：1月 19, 2026， [https://www.youtube.com/watch?v=Ri16bbd\_O7U](https://www.youtube.com/watch?v=Ri16bbd_O7U)  
30. Prompt Engineering for Large Language Model-assisted Inductive Thematic Analysis \- arXiv, 存取日期：1月 19, 2026， [https://arxiv.org/html/2503.22978v1](https://arxiv.org/html/2503.22978v1)  
31. Structured Prompting with JSON: The Engineering Path to Reliable LLMs | by vishal dutt, 存取日期：1月 19, 2026， [https://medium.com/@vishal.dutt.data.architect/structured-prompting-with-json-the-engineering-path-to-reliable-llms-2c0cb1b767cf](https://medium.com/@vishal.dutt.data.architect/structured-prompting-with-json-the-engineering-path-to-reliable-llms-2c0cb1b767cf)  
32. Prompt Engineering: How to get open source LLMs to just return a single value or JSON output? : r/LocalLLaMA \- Reddit, 存取日期：1月 19, 2026， [https://www.reddit.com/r/LocalLLaMA/comments/14zei4q/prompt\_engineering\_how\_to\_get\_open\_source\_llms\_to/](https://www.reddit.com/r/LocalLLaMA/comments/14zei4q/prompt_engineering_how_to_get_open_source_llms_to/)  
33. How Pause Duration Influences Impressions of English Speech: Comparison Between Native and Non-native Speakers \- PMC \- NIH, 存取日期：1月 19, 2026， [https://pmc.ncbi.nlm.nih.gov/articles/PMC8874014/](https://pmc.ncbi.nlm.nih.gov/articles/PMC8874014/)  
34. L1 and L2 Read Speech Habits: Pause Patterns from a Crosslinguistic Perspective, 存取日期：1月 19, 2026， [https://www.readingmatrix.com/files/22-q4nl8d42.pdf](https://www.readingmatrix.com/files/22-q4nl8d42.pdf)  
35. Quantifying Speech Pause Durations in Typical English Speakers \- BYU ScholarsArchive, 存取日期：1月 19, 2026， [https://scholarsarchive.byu.edu/cgi/viewcontent.cgi?article=11044\&context=etd](https://scholarsarchive.byu.edu/cgi/viewcontent.cgi?article=11044&context=etd)  
36. What speaking rate (wpm) do you normally encounter? : r/shorthand \- Reddit, 存取日期：1月 19, 2026， [https://www.reddit.com/r/shorthand/comments/1bokwr2/what\_speaking\_rate\_wpm\_do\_you\_normally\_encounter/](https://www.reddit.com/r/shorthand/comments/1bokwr2/what_speaking_rate_wpm_do_you_normally_encounter/)  
37. Analysis of Emotional Speech—A Review, 存取日期：1月 19, 2026， [https://sail.usc.edu/publications/files/jangwonkim-icassp2010.pdf](https://sail.usc.edu/publications/files/jangwonkim-icassp2010.pdf)  
38. Voice and sound with Speech Synthesis Markup Language (SSML) \- Microsoft Learn, 存取日期：1月 19, 2026， [https://learn.microsoft.com/en-us/azure/ai-services/speech-service/speech-synthesis-markup-voice](https://learn.microsoft.com/en-us/azure/ai-services/speech-service/speech-synthesis-markup-voice)  
39. EmoSphere-TTS: Emotional Style and Intensity Modeling via Spherical Emotion Vector for Controllable Emotional Text-to-Speech \- arXiv, 存取日期：1月 19, 2026， [https://arxiv.org/html/2406.07803v1](https://arxiv.org/html/2406.07803v1)  
40. DAVID: An open-source platform for real-time transformation of infra-segmental emotional cues in running speech \- PMC \- NIH, 存取日期：1月 19, 2026， [https://pmc.ncbi.nlm.nih.gov/articles/PMC5809549/](https://pmc.ncbi.nlm.nih.gov/articles/PMC5809549/)  
41. Lightly Weighted Automatic Audio Parameter Extraction for the Quality Assessment of Consensus Auditory-Perceptual Evaluation of Voice Corresponding author \- arXiv, 存取日期：1月 19, 2026， [https://arxiv.org/html/2311.15582](https://arxiv.org/html/2311.15582)  
42. How does speech synthesis simulate emotional expression? \- Tencent Cloud, 存取日期：1月 19, 2026， [https://www.tencentcloud.com/techpedia/119983](https://www.tencentcloud.com/techpedia/119983)  
43. Estill Voice Training and voice quality control in contemporary commercial singing: an exploratory study \- PubMed, 存取日期：1月 19, 2026， [https://pubmed.ncbi.nlm.nih.gov/27686149/](https://pubmed.ncbi.nlm.nih.gov/27686149/)  
44. Estill Voice Training | Stefan Holmström, 存取日期：1月 19, 2026， [https://www.stefanholmstrom.co.uk/estill-voice-training](https://www.stefanholmstrom.co.uk/estill-voice-training)  
45. ANGUS: Real-time manipulation of vocal roughness for emotional speech transformations, 存取日期：1月 19, 2026， [https://www.researchgate.net/publication/343903867\_ANGUS\_Real-time\_manipulation\_of\_vocal\_roughness\_for\_emotional\_speech\_transformations](https://www.researchgate.net/publication/343903867_ANGUS_Real-time_manipulation_of_vocal_roughness_for_emotional_speech_transformations)  
46. Harsh is large: nonlinear vocal phenomena lower voice pitch and exaggerate body size \- PMC \- NIH, 存取日期：1月 19, 2026， [https://pmc.ncbi.nlm.nih.gov/articles/PMC8261225/](https://pmc.ncbi.nlm.nih.gov/articles/PMC8261225/)  
47. Intonation of Questions: can you tell the difference? \- YouTube, 存取日期：1月 19, 2026， [https://www.youtube.com/watch?v=jFOqHlXtoi4](https://www.youtube.com/watch?v=jFOqHlXtoi4)  
48. American English Intonation | Reduce Interruptions & Send Clear Messages \- YouTube, 存取日期：1月 19, 2026， [https://www.youtube.com/watch?v=ni4PYWeBdBo](https://www.youtube.com/watch?v=ni4PYWeBdBo)  
49. The Glottal Stop in American English \- San Diego Voice and Accent, 存取日期：1月 19, 2026， [https://sandiegovoiceandaccent.com/linking-and-connected-speech/the-glottal-stop-in-american-english](https://sandiegovoiceandaccent.com/linking-and-connected-speech/the-glottal-stop-in-american-english)  
50. Glottal Stop in Modern R.P. | How to do a British Accent like Emma Watson \- YouTube, 存取日期：1月 19, 2026， [https://www.youtube.com/watch?v=czGEL3A2z9Y](https://www.youtube.com/watch?v=czGEL3A2z9Y)  
51. Glottal stop \- Wikipedia, 存取日期：1月 19, 2026， [https://en.wikipedia.org/wiki/Glottal\_stop](https://en.wikipedia.org/wiki/Glottal_stop)  
52. audio \- How to obtain sound envelope using python \- Stack Overflow, 存取日期：1月 19, 2026， [https://stackoverflow.com/questions/30889748/how-to-obtain-sound-envelope-using-python](https://stackoverflow.com/questions/30889748/how-to-obtain-sound-envelope-using-python)  
53. Comparative Analysis of TF–IDF and Hashing Vectorizer for Fake News Detection in Sindhi: A Machine Learning and Deep Learning Approach \- MDPI, 存取日期：1月 19, 2026， [https://www.mdpi.com/2673-4591/46/1/5](https://www.mdpi.com/2673-4591/46/1/5)  
54. The phonetics of stress manifestation: Segmental variation, syllable constituency and rhythm \- Stony Brook Linguists, 存取日期：1月 19, 2026， [https://linguistics.stonybrook.edu/\_pdf/dissertation/Mi-ran\_Kim\_2011\_dissertation.pdf](https://linguistics.stonybrook.edu/_pdf/dissertation/Mi-ran_Kim_2011_dissertation.pdf)  
55. The acoustic correlates of word-level stress and focus-related prominence in Mankiyali | Journal of the International Phonetic Association | Cambridge Core, 存取日期：1月 19, 2026， [https://www.cambridge.org/core/journals/journal-of-the-international-phonetic-association/article/acoustic-correlates-of-wordlevel-stress-and-focusrelated-prominence-in-mankiyali/9555C45F9854A9F277EB744923FDDFFB](https://www.cambridge.org/core/journals/journal-of-the-international-phonetic-association/article/acoustic-correlates-of-wordlevel-stress-and-focusrelated-prominence-in-mankiyali/9555C45F9854A9F277EB744923FDDFFB)  
56. Speech Analysis using Parselmouth | by Satish Kumar Andey \- Medium, 存取日期：1月 19, 2026， [https://satishkumarandey.medium.com/speech-analysis-using-parselmouth-7bb1a2760cc9](https://satishkumarandey.medium.com/speech-analysis-using-parselmouth-7bb1a2760cc9)  
57. PraatScripts/Measure Pitch, HNR, Jitter, and Shimmer.ipynb at master \- GitHub, 存取日期：1月 19, 2026， [https://github.com/drfeinberg/PraatScripts/blob/master/Measure%20Pitch%2C%20HNR%2C%20Jitter%2C%20and%20Shimmer.ipynb](https://github.com/drfeinberg/PraatScripts/blob/master/Measure%20Pitch%2C%20HNR%2C%20Jitter%2C%20and%20Shimmer.ipynb)