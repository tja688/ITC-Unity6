# **動態的白話文：電子遊戲敘事中的動態文字與打字機效果之深度剖析**

## **1\. 緒論：文字作為表演者**

在互動娛樂的廣袤領域中，文字從未僅是被動的訊息載體。從早期大型主機終端機上閃爍的綠色磷光，到現代獨立遊戲傑作中充滿生命力的頂點動畫對話，電子遊戲中的書寫文字承擔著遠超乎資訊傳遞的功能。它充當了聲音的替身、節奏的導演，以及情感的視覺效應器。本報告題為《動態的白話文》（The Kinetic Vernacular），旨在對電子遊戲歷史、設計與工程領域中的動態文字（Dynamic Text）及「打字機效果」（Typewriter Effect）進行決定性且詳盡的深度研究。

我們將深入探討「動態字體排印學」（Kinetic Typography）——即移動文字的技術藝術——在即時渲染環境的特定限制與可能性下的運作機制。雖然電影界早在索爾·巴斯（Saul Bass）與阿爾弗雷德·希區考克（Alfred Hitchcock）的片頭設計中便已運用動態字體，但電子遊戲將其轉化為一種「活體介面」，能夠對玩家的輸入與模擬狀態做出即時反應 1。所謂的「打字機效果」——即文字隨時間推移逐字顯現的過程——不僅是類比時代的擬物化遺跡，更是一種精密的節奏控制機制，旨在模仿人類語音的時間流動 3。當這種效果被注入「文字果汁」（Text Juice）——如空間操縱、色差干擾及同步音效——這個靜態介面便轉化為一場情感表演，無需任何傳統的角色動畫幀，便能傳遞諷刺、恐懼或溫暖 5。

本分析報告將引用廣泛的案例研究，從《Zork》的基礎文字解析器與《勇者鬥惡龍》（Dragon Quest）的捲動獨白，到《動物森友會》（Animal Crossing）、《Undertale》及《Celeste》中複雜的程序化視聽合成。此外，我們將深入探討Unity與Unreal等引擎中的底層工程挑戰、動態文字對玩家留存率與閱讀理解的心理學影響，以及藝術意圖與無障礙設計（Accessibility）之間的關鍵交集。

## ---

**2\. 歷史演進：從終端機到紋理**

要理解現代動態文字的實作，必須先追溯其在媒介技術地層中的譜系。遊戲文字的演化史，本質上是一部克服頻寬限制的歷史——最初是記憶體的頻寬，隨後是解析度的頻寬，最終是人類注意力的頻寬。

### **2.1 大型主機時代與電傳打字機範式（1970s）**

最早的互動式敘事作品，如《巨洞冒險》（Colossal Cave Adventure, 1976）與Infocom的《Zork》（1977），誕生於PDP-10等大型主機之上 7。在這個時代，「打字機效果」並非一種風格選擇，而是一種硬體現實。透過電傳打字機（TTY）介面連接的終端機，以連線速度（通常為110或300波特率）逐字列印文字。這種物理上的列印動作——或是CRT螢幕上的磷光掃描軌跡——創造了一種自然的懸念。機器正在「思考」，而玩家則在等待判決。

文字解析器（Parser）介面要求玩家輸入自然語言指令，建立了一種雙向的文字關係。遊戲扮演著敘述者的角色，僅透過散文來描述「地下大帝國」（Great Underground Empire）7。現代遊戲中常見的震動文字或動態變色在此時是不可能的；所謂的「效果」純粹是數據傳輸的節奏。然而，這建立了遊戲文字應「隨時間展開」而非「瞬間整塊出現」的基礎預期，這一慣例在五十年後依然根深蒂固 7。這種逐字顯現的過程，無意中模擬了人類說話的線性過程，為後來RPG對話系統奠定了心理學基礎。

### **2.2 8位元與16位元RPG：捲動的限制與美學（1980s-1990s）**

隨著遊戲轉移至NES（紅白機）與SNES（超級任天堂）等家用主機，限制從傳輸速度轉變為螢幕空間與記憶體容量。在《勇者鬥惡龍》（西方發行名為Dragon Warrior）等遊戲中，對話視窗成為了敘事傳遞的主要舞台 8。

從技術角度來看，這些系統使用基於圖塊（Tile-based）的渲染方式。瞬間顯示整頁文字在運算上其實是輕而易舉的，但開發者保留了逐字捲動的顯示方式。其原因可歸納為三點：

1. **節奏與理解（Pacing and Comprehension）：** 捲動文字強迫玩家以特定的節奏閱讀，防止他們快速掃視而錯過關鍵劇情細節 9。這是一種強制的認知同步，確保玩家的思維速度與遊戲的敘事速度一致。  
2. **硬體限制與在地化（Hardware Limits）：** 早期的在地化工作，特別是《勇者鬥惡龍》，面臨嚴峻的記憶體限制。英語劇本所需的空間遠大於日語假名。「捲動」允許使用可變寬度的視窗管理與緩衝區清除技術，從而掩蓋後續數據庫的讀取時間 8。  
3. **介面即敘事（Diegetic Weight）：** 在《EarthBound》（Mother 2）中，文字不僅是UI元素，更是一種風味傳遞系統。「提示男」（Hint Guy）與其他NPC提供了對劇情無關緊要但對「風味」至關重要的對話——這在遊戲敘事學（Ludonarrative）研究中被視為建構世界觀的關鍵 10。

《勇者鬥惡龍》系列還引入了文字音效回饋的概念，儘管形式粗糙。伴隨文字捲動的「嗶-嗶-嗶」聲效，是後來複雜語音合成技術的雛形，其主要功能是確認系統處於活躍狀態，並暗示角色正在「說話」12。

### **2.3 現代獨立復興：文字作為美學武器（2010s-Present）**

在2010年代後的獨立遊戲爆發期，儘管高傳真圖形已觸手可及，許多開發者卻回歸像素藝術與文字框。然而，這種回歸標誌著一個「新歷史主義」（New Historical）階段，復古美學被現代強大的運算能力重新語境化 13。

像《Undertale》與《Celeste》這樣的遊戲將打字機效果武器化。不再受限於硬體，文字在角色憤怒時會*劇烈震動*，在角色頭暈時會*波浪起伏*，在強調語氣時會*減速*。這就是設計師所謂的「果汁」（Juice）——一種讓互動感覺豐滿、令人滿足的觸覺與視覺回饋 14。動態字體排印學成為了一種繞過昂貴配音成本，同時保留甚至增強情感細微差別的方法 16。文字本身，成為了演員。

## ---

**3\. 動態字體排印學的心理機制**

為何我們更傾向於觀看字母逐一出現，而非閱讀靜態的文字塊？答案位於認知心理學與使用者介面設計的交匯處。

### **3.1 蔡加尼克效應與預期心理（The Zeigarnik Effect）**

關於動態字體的研究表明，由於人類視覺系統固有的「運動偏差」（Motion Bias），移動的文字比靜態文字更能捕捉注意力 17。我們在生物學上被硬體編碼為優先偵測運動物體。

此處一個關鍵的心理機制是**蔡加尼克效應**（Zeigarnik Effect），該理論指出人們對未完成或被中斷的任務記憶更為深刻 18。打字機效果創造了一種持續的「未完成」狀態。隨著句子的展開，玩家的大腦參與了一項微觀的預測任務，不斷預期下一個字詞。這將玩家「黏」在對話框上，而靜態文字塊則容易被快速瀏覽並忽視 16。

這種逐字揭示的過程也利用了**資訊缺口理論**（Information Gap Theory）。每一個未出現的字母都是一個微小的懸念，雖然持續時間極短，但足以維持玩家的認知喚醒水平。

### **3.2 情感映射與韻律學（Emotional Mapping and Prosody）**

動態字體充當了**韻律**（Prosody）的視覺地圖——即語音的節奏、重音與語調。在書面語言中，我們依賴標點符號來傳達這些元素，但往往力有未逮。動態文字擴展了這一詞彙表：

* **速度（Speed）：** 急促的文字模擬興奮或焦慮；緩慢的文字模擬猶豫、沉重或思考 16。  
* **運動（Motion）：** 「震動」效果映射到憤怒或恐懼的生理顫抖（高喚醒、負效價）。「波浪」效果映射到吟唱般的語調、頭暈或醉酒 19。  
* **大小（Size）：** 動態縮放（例如文字膨脹以表示喊叫）直接映射到音量 20。

以《Celeste》為例，主角Madeline的內心獨白與外在對話不僅通過上下文區分，更通過文字動畫的*質地*來區分。「老太太」角色的笑聲被具象化為音節「HA HA」在螢幕上「跳舞」，並在過場切換時視覺殘留，象徵著揮之不去的回音 5。這種跨感官模態的映射（Cross-modal Mapping）讓玩家能透過眼睛「聽見」情感。

### **3.3 捲動的多巴胺迴路**

文字捲動的行為也觸及了**變動比率增強時程**（Variable Ratio Reinforcement Schedules）21。雖然這通常與社交媒體的動態消息相關，但同樣適用於RPG對話。下一行文字內容的不確定性，結合有節奏的視覺回饋，創造了一個微型的多巴胺迴路。這在文字量巨大的遊戲（如視覺小說、RPG）中至關重要，能防止玩家產生閱讀疲勞。運動讓閱讀感覺像是在*玩*，而非*工作* 6。

### **3.4 閱讀速度與認知負荷的張力**

然而，這其中存在著微妙的平衡。無障礙設計指南強調，固定的文字速度具有排他性。閱讀障礙者可能難以跟上過快的文字，而速讀者則可能因人為的延遲而感到沮喪 22。這種摩擦——設計師對節奏的控制慾與玩家對效率的渴望之間的衝突——是現代UI設計的核心張力。像《最後生還者 第II章》（The Last of Us Part II）這樣的遊戲引入了極其細緻的設置來控制這些速度或完全禁用動畫，將無障礙性置於美學控制之上 23。

## ---

**4\. 技術實作：波浪文字的工程學**

要創造這些效果，需要對渲染管線有深刻的理解。在Unity與Unreal等現代引擎中，文字不僅僅是一串字符，而是由三角形構成的網格（Mesh）。

### **4.1 打字機協程（The Typewriter Coroutine）**

動態文字最基礎的實作是「打字機效果」。在C\#（Unity）中，這通常透過協程（Coroutine）來處理 25。

**演算法 1：基礎打字機邏輯**

1. **輸入：** 目標字串（例如 "Hello World"）。  
2. **過程：**  
   * 將完整字串存入Text組件，但將 maxVisibleCharacters 屬性設為0。  
   * 進入迴圈（Coroutine）。  
   * 每幀或每隔特定時間（delay），將 maxVisibleCharacters 增加1。  
   * 播放音效（可選）。  
   * **標點符號檢查：** 若當前顯示的字符是逗號或句號，增加等待時間以模擬呼吸停頓 25。  
3. **結束：** 當 maxVisibleCharacters 等於字串長度時，顯示「繼續」箭頭。

這種方法優於字串串接（例如 text \= text \+ "a"），因為它避免了每幀創建新字串實例所導致的記憶體垃圾回收（Garbage Collection）開銷。它修改的是網格的渲染限制，而非數據本身 25。

### **4.2 頂點著色器與網格操縱（Vertex Shaders & Mesh Manipulation）**

對於「波浪」或「震動」等效果，僅修改字串是不夠的。我們必須修改字母的幾何形狀。  
在Unity的 TextMeshPro (TMP) 中，每個字符是一個由4個頂點組成的四邊形（Quad）26。IVertexModifier 介面允許開發者存取 meshInfo 陣列。  
波浪的數學原理：  
為了創造正弦波效果（常用於幽靈或微醺的聲音），開發者會對每個頂點的Y軸位置應用三角函數偏移 26。

$$Y\_{new} \= Y\_{original} \+ A \\cdot \\sin(F \\cdot X\_{original} \+ S \\cdot T)$$  
其中：

* $A$ 為振幅（Amplitude，波的高度）。  
* $F$ 為頻率（Frequency，波的密集度）。  
* $X\_{original}$ 為字符的水平位置（創造波紋穿過句子的效果）。  
* $S$ 為速度（Speed）。  
* $T$ 為時間（Time）。

這種計算可以在CPU上進行（每幀修改網格數據陣列），或更高效地在GPU上透過**頂點著色器**（Vertex Shader）進行 28。

* **CPU方法：** 易於與遊戲邏輯互動（例如當玩家跳躍時停止波浪），但對於大量文字來說開銷昂貴。  
* **GPU方法：** 極快，可處理數千個字符，但需要熟悉Shader Graph或HLSL語言 29。

抖動/震動效果（Jitter/Shake）：  
對於「Undertale式震動」，邏輯將正弦波替換為雜訊函數或每幀計算的隨機偏移。

$$\\vec{P}\_{offset} \= \\vec{P}\_{original} \+ \\text{RandomInsideUnitCircle}() \\cdot \\text{Intensity}$$

關鍵在於，這種隨機性通常需要是連貫的（如Perlin Noise）或鎖定像素網格（Pixel Snapping），以保持復古美學，避免產生過於平滑的「數位感」26。

### **4.3 豐富文字與自定義裝飾器（Rich Text & Custom Decorators）**

Unreal Engine 透過其 **Rich Text Block** 系統處理此類需求。不同於標準文字字串，Rich Text 支援「裝飾器」（Decorators）。開發者可以創建自定義的裝飾器類別（例如 RichTextBlockImageRowDecorator），解析如 \<img id="sword"/\> 的標籤，並將其替換為行內的Slate小部件或紋理 31。

這允許將控制器圖示或物品精靈圖無縫整合到流動的文字中，這是現代遊戲教學（Tutorialization）的主要手段。更複雜的效果（如彩虹漸層在文字上移動）可以通過為字體創建自定義材質並隨時間操縱其UV坐標來實現 33。

### **4.4 「胡言亂語」音訊合成演算法**

視覺組件僅是方程式的一半。文字遊戲中的「聲音」通常是程序化生成的音效。

技術 A：「動物語」方法（Animalese \- Animal Crossing）  
此方法涉及逐字合成。系統為每個字符（或音素）查找一個短促的音訊樣本。

* **輸入：** "Hello"  
* **過程：** 快速連續播放 h.wav, e.wav, l.wav, l.wav, o.wav。  
* **音高偏移（Pitch Shifting）：** 為了區分角色，基礎樣本會被變調。一個「暴躁」村民的音高乘數可能是0.6；一個「元氣」村民則是1.5 34。  
* **速度：** 播放速度被加速，將獨立樣本模糊化，形成連貫的「咕噥聲」35。

技術 B：「Undertale」方法（聲音咕噥 Voice Grunts）  
《Undertale》不使用音素，而是為每個角色使用單一的「聲音咕噥」樣本（例如骷髏的 "neh" 或狗的 "bark"）。

* **演算法：** 在每第 *n* 個字符（例如每2或3個字母）播放聲音，以避免「機關槍」效應 36。  
* **程式碼洞察：** 程式碼片段建議檢查 if (visibleCharacterCount % frequency \== 0\) PlaySound()。這創造了一種有節奏的「打字」聲，而非持續的嗡嗡聲 37。  
* **例外處理：** 演算法必須在空格和標點符號處暫停，讓句子的「呼吸」被聽見 38。

技術 C：「Banjo-Kazooie」式喃喃語  
Rareware的遊戲使用循環隨機化器。

* **輸入：** 一組3-5個咕噥聲（例如 "Guh", "Huh", "Wuh"）。  
* **過程：** 當文字捲動時，隨機選擇其中一個聲音播放。這創造了一種有質感、有機的聲音，感覺像在說話，但不需要語言學解析 39。

下表總結了這些音訊系統的技術差異：

**表 1：文字音效系統比較分析**

| 特徵 | Animal Crossing (Animalese) | Undertale (Voice Grunts) | Banjo-Kazooie (Mumble) | Celeste (Synth Voice) |
| :---- | :---- | :---- | :---- | :---- |
| **合成類型** | 連接合成 (字母/音素) | 觸發樣本 (每字符間隔) | 隨機選擇 (每字符) | 共振峰調變 (Formant Modulation) |
| **音訊來源** | \~26 字母音 (a-z) | 每個角色 1-2 個獨特「咕噥」 | 3-5 個隨機「喃喃」聲 | 合成波形 |
| **音高變化** | 每個NPC固定 | 固定或微小隨機變異 | 隨機變異 | 動態 (語調曲線) |
| **節奏** | 連續流 (急促) | 斷奏 (跳過字母) | 斷奏 / 循環 | 流動 (音節式) |
| **美學目標** | 可愛、忙碌、「喋喋不休」 | 復古、獨特、標誌性 | 喜劇、卡通化 | 情感化、氛圍化 |

## ---

**5\. 案例研究：敘事架構的具象化**

### **5.1 Celeste：焦慮的對話**

《Celeste》是透過字體排印學實現「遊戲敘事共鳴」（Ludonarrative Resonance）的典範。遊戲探討焦慮與憂鬱的主題，而文字引擎是傳遞此主題的主要載體。

* **波浪文字：** 當Madeline感到疲憊或不確定時使用。頂點起伏，創造出不穩定的視覺隱喻 5。  
* **震動文字：** 用於喊叫或恐慌。  
* **「肖像」系統：** 《Celeste》使用高解析度角色肖像，並隨文字改變表情。程式碼將特定的文字標籤（如 {anxious}）與肖像切換同步，確保視覺頭像與文字語氣匹配 41。  
* **胡言亂語的聲音：** 《Celeste》的音訊經過合成以模仿情感共振峰結構。它不僅僅是隨機的嗶嗶聲；胡言亂語的音高曲線在疑問句時上升，在陳述句時下降，模仿人類語調 19。

### **5.2 Undertale：透過字體與聲音塑造角色**

《Undertale》證明了字體選擇即角色設計。

* **Papyrus 與 Sans：** 這兩個角色以他們使用的字體命名（Papyrus與Comic Sans）。這是一種後設敘事裝置，他們對話的形式本身定義了他們的個性（自命不凡/古板 vs. 隨性/滑稽）42。  
* **「恐怖」之聲：** 當角色Sans變得具威脅性時，打字機聲音停止，字體保持靜態，且肖像的眼睛變黑。預期中動態效果的*缺失*（Absence）本身成為了一種驚嚇（Jump Scare）。  
* **程式碼恐懼：** 據傳（並由反編譯部分證實），《Undertale》的對話系統依賴巨大的 switch 語句來處理這些無數的變體，這證明了有效的設計不一定需要優雅的工程 43。

### **5.3 Hades：神聖的文本**

在Supergiant Games的《Hades》中，文字被用來強化眾神的階級制度。

* **介面整合：** 文字經常懸浮在世界中，或伴隨「雷鳴般」的視覺特效出現，震動整個螢幕，強化說話者的力量感 45。  
* **圖示學：** 關鍵術語（如「恩賜」Boons或特定神祇的名字）使用獨特的顏色高亮並嵌入圖示，作為超文本（Hypertext）教導玩家遊戲的詞彙，而不破壞沉浸感 46。  
* **聲音分層：** 遊戲不僅有全程語音，還有伴隨文字顯現的細微音效（如Charon的硬幣叮噹聲與水聲），創造出豐富的觸覺回饋 47。

### **5.4 Paper Mario: The Thousand-Year Door：重製版的文本演變**

《紙片瑪利歐：千納之門》重製版（Remake）展示了文字效果如何隨時代演進。

* **語音氣泡的現代化：** 原版遊戲已有動態文字，但重製版增加了更多基於物理的氣泡動畫。當角色生氣時，不僅文字震動，整個對話氣泡也會劇烈收縮膨脹 48。  
* **在地化與審查的微妙之處：** 重製版在某些對話上進行了調整（例如Vivian的性別認同描述在不同語言版本的恢復），這顯示了文字內容本身如何在不同文化語境下被「動態」調整。儘管這更多是內容層面，但文字顯示的*方式*（例如Vivian被欺負時文字縮小、變淡）強化了敘事的情感衝擊 49。

## ---

**6\. 設計果汁：「脆口」文字的理論**

在遊戲設計中，「果汁」（Juice）一詞指的是增強體驗的非功能性回饋——螢幕震動、粒子效果和音效 15。動態字體排印學即是「文字果汁」（Text Juice）。

將文字與粒子效果進行比較：

* **粒子：** 為物理互動（碰撞、爆炸）提供回饋。  
* **文字果汁：** 為*情感*與*資訊*互動提供回饋。

當玩家造成爆擊時，傷害數字不僅僅是出現；它會*彈出*（Pop）、放大、變紅並抖動。這就是**彈出效應**（Pop-up Effect），通過使用彈性緩動函數（Elastic Easing Function）從中心點向外線性插值頂點來實現 51。這直觀地傳達了傷害的*嚴重性*。

UI中的「蔡加尼克」應用：  
遊戲UI設計師經常利用「蔡加尼克效應」來動畫化目標文字。當新增任務時，文字可能會滑入、發光，然後定格。這種運動吸引眼球（中斷當前的焦點），確保玩家註冊新任務。如果文字只是簡單地出現，變化盲視（Change Blindness）可能會導致玩家錯過它 2。  
**表 2：常見動態文字標籤及其心理映射**

| 效果 | 視覺描述 | 心理映射 | 使用範例 |
| :---- | :---- | :---- | :---- |
| **Shake (震動)** | 隨機頂點抖動 | 恐懼、憤怒、不穩定、高喚醒 | "滾出去！"、"我好怕。" |
| **Wave (波浪)** | Y軸正弦波 | 頭暈、諷刺、吟唱、醉酒 | "嗚嗚嗚我是鬼\~"、"我覺得好想吐..." |
| **Scale Pulse (脈衝)** | 膨脹/收縮尺寸 | 心跳、急迫、大聲 | "砰-砰"、大喊。 |
| **Fade In (淡入)** | Alpha透明度 0-\>1 | 溫柔、耳語、虛弱 | 臨終遺言、秘密。 |
| **Rainbow (彩虹)** | UV捲動漸層 | 魔法、力量、無敵、特殊物品 | "無敵星狀態！"、"獲得傳說之劍" |
| **Slow Type (慢速)** | 增加字元間延遲 | 猶豫、沉重、懸念 | "我... 是... 你的父親。" |

## ---

**7\. 無障礙性與作者論的衝突**

動態字體排印學的興起帶來了重大的無障礙挑戰。雖然「波浪文字」能向視力正常的玩家傳達「頭暈」，但它可能導致閱讀障礙者無法閱讀，甚至引發光敏性癲癇或暈動症 22。

### **7.1 速度的衝突**

設計師經常鎖定文字速度以強制特定的戲劇節奏（例如小島秀夫的做法）。然而，無障礙指南（WCAG和遊戲無障礙指南）明確指出，玩家應能夠調整文字速度或完全跳過 22。

* **閱讀率：** 平均閱讀速度差異巨大。對於速讀者來說，緩慢的打字機效果可能令人難以忍受，不僅沒有增強沉浸感，反而破壞了它。  
* **解決方案：** 《最後生還者 第II章》設立了黃金標準。它提供「文字轉語音」（TTS）、「高對比顯示」以及對HUD比例和文字行為的精細控制。它允許玩家「跳過解謎選項」，關鍵是，自定義字幕呈現（說話者姓名、顏色、方向箭頭）23。

### **7.2 運動中的易讀性**

不斷移動（抖動）的文字較難閱讀。

* **最佳實踐：** 將運動用於*強調*（短詞），而非用於長段落的說明。  
* **實作：** 在《Celeste》中，震動通常保留給簡短的感嘆詞（"Stop\!", "Help\!"），而非段落。當段落真的震動時，強度通常很低。  
* **WCAG合規：** 遊戲應提供「減少運動」（Reduce Motion）開關，在保留內容的同時禁用文字上的頂點動畫 53。

## ---

**8\. 未來展望：AI與程序化情感**

動態字體排印學的未來在於語意分析（Semantic Analysis）。目前，設計師必須手動標記文字：我\<shake\>好生氣！\</shake\>。  
未來的系統，由大型語言模型（LLMs）和情感分析驅動，可以自動解析台詞的情感內容並應用相應的頂點著色器效果。

* **輸入：** "我真不敢相信你做了那種事。"（偵測情感：震驚/背叛）。  
* **輸出：** 系統自動對文字應用「顫抖」效果，並在最後幾個字減慢打字機速度。  
* **研究：** 關於即時通訊中「情感文字」（Emotive Text）的研究表明，將動態效果映射到情感是恢復數位溝通中非語言線索的可行方法 54。

此外，隨著VR/AR技術的發展，文字將不再侷限於2D平面。空間化文字（Spatialized Text）——即文字在3D空間中佔據物理位置並隨視角變化——將成為新的敘事工具。這在《Hades》的戰鬥浮動文字中已見端倪，但未來可能演變為文字本身成為可互動的物理物件。

## **9\. 結論**

電子遊戲中的「打字機效果」與動態字體排印學並非單純的裝飾；它們是五十年來試圖讓機器「說話」的演化結果。從《Zork》沉默的捲動緩衝區，到《Undertale》極具表現力、彩虹震動的字體，目標始終如一：賦予書寫文字以生命的氣息。

對於遊戲設計師而言，教訓在於整合。文字不應是漂浮在遊戲之上的圖層；它應是世界物理模擬的一部分。它應該像活體一樣反應、移動和發聲。  
對於工程師而言，教訓在於優化與邏輯。利用頂點著色器與協程，可以在不犧牲幀率的情況下實現豐富的表達。  
而對於整個行業而言，教訓是包容性。隨著文字變得更加動態，穩定和清晰化文字以供所有讀者使用的工具必須同步進步。  
遊戲文字的未來，不僅在於寫了什麼，更在於它如何移動。

---

引用文獻代碼對照：

1

#### **引用的著作**

1. Typography, the Video Game \- Hyperallergic, 存取日期：1月 18, 2026， [https://hyperallergic.com/typography-the-video-game/](https://hyperallergic.com/typography-the-video-game/)  
2. Kinetic typography: the what, why, and how \- Linearity, 存取日期：1月 18, 2026， [https://www.linearity.io/blog/kinetic-typography/](https://www.linearity.io/blog/kinetic-typography/)  
3. "Typewriter" Text Effect \- RPG Maker Forums, 存取日期：1月 18, 2026， [https://forums.rpgmakerweb.com/index.php?threads/typewriter-text-effect.102482/](https://forums.rpgmakerweb.com/index.php?threads/typewriter-text-effect.102482/)  
4. Typewriter Effect in Games \- Yae or nay? : r/gamedev \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/gamedev/comments/1ami11y/typewriter\_effect\_in\_games\_yae\_or\_nay/](https://www.reddit.com/r/gamedev/comments/1ami11y/typewriter_effect_in_games_yae_or_nay/)  
5. Celeste : The philosophy of game design, a study of the prologue, 存取日期：1月 18, 2026， [https://www.pointnthink.fr/en/celeste-philosophy/](https://www.pointnthink.fr/en/celeste-philosophy/)  
6. Juicy Game Design: Understanding the Impact of Visual Embellishments on Player Experience | Request PDF \- ResearchGate, 存取日期：1月 18, 2026， [https://www.researchgate.net/publication/336711817\_Juicy\_Game\_Design\_Understanding\_the\_Impact\_of\_Visual\_Embellishments\_on\_Player\_Experience](https://www.researchgate.net/publication/336711817_Juicy_Game_Design_Understanding_the_Impact_of_Visual_Embellishments_on_Player_Experience)  
7. Zork \- Wikipedia, 存取日期：1月 18, 2026， [https://en.wikipedia.org/wiki/Zork](https://en.wikipedia.org/wiki/Zork)  
8. The History of Dragon Quest \- Game Developer, 存取日期：1月 18, 2026， [https://www.gamedeveloper.com/game-platforms/the-history-of-dragon-quest](https://www.gamedeveloper.com/game-platforms/the-history-of-dragon-quest)  
9. The Evolution of Dragon Quest: A Retrospective \- Why "Familiar" Doesn't Equal Boring and Overly Iterative. : r/JRPG \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/JRPG/comments/enov7e/the\_evolution\_of\_dragon\_quest\_a\_retrospective\_why/](https://www.reddit.com/r/JRPG/comments/enov7e/the_evolution_of_dragon_quest_a_retrospective_why/)  
10. Hidden Dialogue in Earthbound \- Set Side B, 存取日期：1月 18, 2026， [https://setsideb.com/hidden-dialogue-in-earthbound/](https://setsideb.com/hidden-dialogue-in-earthbound/)  
11. Why EarthBound's Flavor Text Tastes So Good \- With A Terrible Fate, 存取日期：1月 18, 2026， [https://withaterriblefate.com/2021/07/07/why-earthbounds-flavor-text-tastes-so-good/](https://withaterriblefate.com/2021/07/07/why-earthbounds-flavor-text-tastes-so-good/)  
12. History Lessons: Dragon Quest \- Waltorious Writes About Games, 存取日期：1月 18, 2026， [https://waltoriouswritesaboutgames.com/2018/01/08/history-lessons-dragon-quest/](https://waltoriouswritesaboutgames.com/2018/01/08/history-lessons-dragon-quest/)  
13. STS13: Videogame Typography and its Antecedents \- Zach Whalen, 存取日期：1月 18, 2026， [https://www.zachwhalen.net/posts/sts13-videogame-typography-and-its-antecedents](https://www.zachwhalen.net/posts/sts13-videogame-typography-and-its-antecedents)  
14. Squeezing more juice out of your game design\! \- GameAnalytics, 存取日期：1月 18, 2026， [https://www.gameanalytics.com/blog/squeezing-more-juice-out-of-your-game-design](https://www.gameanalytics.com/blog/squeezing-more-juice-out-of-your-game-design)  
15. Secrets of Game Feel and Juice \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=216\_5nu4aVQ](https://www.youtube.com/watch?v=216_5nu4aVQ)  
16. What is Kinetic Typography? | Captivating Audiences with Motion \- Weavers Web Solutions, 存取日期：1月 18, 2026， [https://weaversweb.com/what-is-kinetic-typography/](https://weaversweb.com/what-is-kinetic-typography/)  
17. Kinetic typography: when and why it works \- We Design Motion, 存取日期：1月 18, 2026， [https://wedesignmotion.com/blog/design/kinetic-typography-when-and-why-it-works/](https://wedesignmotion.com/blog/design/kinetic-typography-when-and-why-it-works/)  
18. The Key Advantages of Kinetic Typography in Design \- LottieFiles, 存取日期：1月 18, 2026， [https://lottiefiles.com/blog/design-inspiration/the-key-advantages-of-kinetic-typography-in-design](https://lottiefiles.com/blog/design-inspiration/the-key-advantages-of-kinetic-typography-in-design)  
19. Any tips for recreating gibberish text sound (like in Celeste or Undertale)? \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/GameAudio/comments/iet3ud/any\_tips\_for\_recreating\_gibberish\_text\_sound\_like/](https://www.reddit.com/r/GameAudio/comments/iet3ud/any_tips_for_recreating_gibberish_text_sound_like/)  
20. (PDF) Kinetic Typography in Digital Media \- ResearchGate, 存取日期：1月 18, 2026， [https://www.researchgate.net/publication/382470702\_Kinetic\_Typography\_in\_Digital\_Media](https://www.researchgate.net/publication/382470702_Kinetic_Typography_in_Digital_Media)  
21. The Dopamine Loop: Why Your Brain is Hooked on Infinite Scrolling | by Mind & Tech Lab, 存取日期：1月 18, 2026， [https://medium.com/@ys1738033/the-dopamine-loop-why-your-brain-is-hooked-on-infinite-scrolling-06ff21c872c4](https://medium.com/@ys1738033/the-dopamine-loop-why-your-brain-is-hooked-on-infinite-scrolling-06ff21c872c4)  
22. Allow players to progress through text prompts at their own pace, 存取日期：1月 18, 2026， [https://gameaccessibilityguidelines.com/allow-players-to-progress-through-text-prompts-at-their-own-pace/](https://gameaccessibilityguidelines.com/allow-players-to-progress-through-text-prompts-at-their-own-pace/)  
23. Accessibility options for The Last of Us Part II \- PlayStation, 存取日期：1月 18, 2026， [https://www.playstation.com/en-us/games/the-last-of-us-part-ii/accessibility/](https://www.playstation.com/en-us/games/the-last-of-us-part-ii/accessibility/)  
24. The Last of Us Part II Accessibility Options Appreciation (No Spoilers) : r/GirlGamers \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/GirlGamers/comments/hf8o9y/the\_last\_of\_us\_part\_ii\_accessibility\_options/](https://www.reddit.com/r/GirlGamers/comments/hf8o9y/the_last_of_us_part_ii_accessibility_options/)  
25. Create a Typewriter Effect for TextMesh Pro in Unity 6 \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=UR\_Rh0c4gbY](https://www.youtube.com/watch?v=UR_Rh0c4gbY)  
26. Custom Text Effects in Unity using TextMeshPro \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=OszKsS\_Oe98](https://www.youtube.com/watch?v=OszKsS_Oe98)  
27. Wavy Text Unity Shader \- DrMop, 存取日期：1月 18, 2026， [https://www.drmop.com/index.php/2016/09/16/wavy-text-unity-shader/](https://www.drmop.com/index.php/2016/09/16/wavy-text-unity-shader/)  
28. I made a beginner-friendly tutorial about vertex shaders in Shader Graph and made this wave effect (link in comments) : r/Unity3D \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/Unity3D/comments/1apt3up/i\_made\_a\_beginnerfriendly\_tutorial\_about\_vertex/](https://www.reddit.com/r/Unity3D/comments/1apt3up/i_made_a_beginnerfriendly_tutorial_about_vertex/)  
29. Waves in ShaderGraph by Vertex Displacement | Unity Tutorial \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=poi10W6LHJo](https://www.youtube.com/watch?v=poi10W6LHJo)  
30. Typewriter Text \- Unreal Engine 5 Tutorial \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=fw9bFv5a-\_4](https://www.youtube.com/watch?v=fw9bFv5a-_4)  
31. UMG Rich Text Blocks in Unreal Engine \- Epic Games Developers, 存取日期：1月 18, 2026， [https://dev.epicgames.com/documentation/en-us/unreal-engine/umg-rich-text-blocks-in-unreal-engine](https://dev.epicgames.com/documentation/en-us/unreal-engine/umg-rich-text-blocks-in-unreal-engine)  
32. Advanced Text Styling with Rich Text Block \- Unreal Engine, 存取日期：1月 18, 2026， [https://www.unrealengine.com/de/tech-blog/advanced-text-styling-with-rich-text-block](https://www.unrealengine.com/de/tech-blog/advanced-text-styling-with-rich-text-block)  
33. Effects for TextMesh Pro: materials and gradients in Unity 6 \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=GPxDIUcNX-o](https://www.youtube.com/watch?v=GPxDIUcNX-o)  
34. Animalese \- Animal Crossing Wiki \- Nookipedia, 存取日期：1月 18, 2026， [https://nookipedia.com/wiki/Animalese](https://nookipedia.com/wiki/Animalese)  
35. I Built The Animal Crossing Voice Generator In 64-Lines of Code \- (Animalese) \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=RYnI\_ZLj5ys](https://www.youtube.com/watch?v=RYnI_ZLj5ys)  
36. Best way to make animal crossing/undertale "blarble" noises with textbox \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/gamedev/comments/3oeege/best\_way\_to\_make\_animal\_crossingundertale\_blarble/](https://www.reddit.com/r/gamedev/comments/3oeege/best_way_to_make_animal_crossingundertale_blarble/)  
37. How to make an Undertale voice for your games. Construct 3 \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=CAY0HTdmnKQ](https://www.youtube.com/watch?v=CAY0HTdmnKQ)  
38. \[SOLVED\] How to make a sound play per character shown in dialogue? (Undertale/Deltarune style) \- Lemma Soft Forums, 存取日期：1月 18, 2026， [https://lemmasoft.renai.us/forums/viewtopic.php?t=69074](https://lemmasoft.renai.us/forums/viewtopic.php?t=69074)  
39. Creating the Banjo Kazooie Voice Mumbling : r/BanjoKazooie \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/BanjoKazooie/comments/nw9s2y/creating\_the\_banjo\_kazooie\_voice\_mumbling/](https://www.reddit.com/r/BanjoKazooie/comments/nw9s2y/creating_the_banjo_kazooie_voice_mumbling/)  
40. I perfectly recreated the Banjo-Kazooie dialogue system as a bits alert for my channel\! (Description in comments) : r/Twitch \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/Twitch/comments/xum9f3/i\_perfectly\_recreated\_the\_banjokazooie\_dialogue/](https://www.reddit.com/r/Twitch/comments/xum9f3/i_perfectly_recreated_the_banjokazooie_dialogue/)  
41. Recreating Celeste Dialog System? \- Devlog \# 4 \- Making A Top Down RPG w \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=3V-v1OtaIqo](https://www.youtube.com/watch?v=3V-v1OtaIqo)  
42. Would you approve of giberish dialogue audio like in Celeste/Animal Crossing/Undertale as a substitue for voice acting in VNs? : r/visualnovels \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/visualnovels/comments/1b58c5q/would\_you\_approve\_of\_giberish\_dialogue\_audio\_like/](https://www.reddit.com/r/visualnovels/comments/1b58c5q/would_you_approve_of_giberish_dialogue_audio_like/)  
43. Undertale dialog system is one giant switch statement that goes on for 5k+ lines of code, 存取日期：1月 18, 2026， [https://www.reddit.com/r/programminghorror/comments/1exqik2/undertale\_dialog\_system\_is\_one\_giant\_switch/](https://www.reddit.com/r/programminghorror/comments/1exqik2/undertale_dialog_system_is_one_giant_switch/)  
44. Apparently, Undertale has a 1000+ long case switch statement. \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/YandereTechnique/comments/ufya27/apparently\_undertale\_has\_a\_1000\_long\_case\_switch/](https://www.reddit.com/r/YandereTechnique/comments/ufya27/apparently_undertale_has_a_1000_long_case_switch/)  
45. Introduction: The Sound and Music of Hades \- UC Press Journals, 存取日期：1月 18, 2026， [https://online.ucpress.edu/jsmg/article/6/1/1/205396/IntroductionThe-Sound-and-Music-of-Hades](https://online.ucpress.edu/jsmg/article/6/1/1/205396/IntroductionThe-Sound-and-Music-of-Hades)  
46. Hades: speech patterns of all characters analysis : r/HadesTheGame \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/HadesTheGame/comments/pic08d/hades\_speech\_patterns\_of\_all\_characters\_analysis/](https://www.reddit.com/r/HadesTheGame/comments/pic08d/hades_speech_patterns_of_all_characters_analysis/)  
47. How Hades Creates a Responsive Underworld | by JoDoe | Medium, 存取日期：1月 18, 2026， [https://medium.com/@JoDoe/how-hades-creates-a-responsive-underworld-915715a7c2a](https://medium.com/@JoDoe/how-hades-creates-a-responsive-underworld-915715a7c2a)  
48. Nitpicking on the TTYD remake: speech bubbles : r/papermario \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/papermario/comments/1cuw5wm/nitpicking\_on\_the\_ttyd\_remake\_speech\_bubbles/](https://www.reddit.com/r/papermario/comments/1cuw5wm/nitpicking_on_the_ttyd_remake_speech_bubbles/)  
49. TTYD Remake's Many Dialogue Changes (yes, the video is 4.5 HOURS long) \- Reddit, 存取日期：1月 18, 2026， [https://www.reddit.com/r/papermario/comments/1kg2wxf/ttyd\_remakes\_many\_dialogue\_changes\_yes\_the\_video/](https://www.reddit.com/r/papermario/comments/1kg2wxf/ttyd_remakes_many_dialogue_changes_yes_the_video/)  
50. Paper Mario: TTYD changes dialogue in catcalling scene \- Nintendo Switch \- GameFAQs, 存取日期：1月 18, 2026， [https://gamefaqs.gamespot.com/boards/189706-nintendo-switch/80756811?page=8](https://gamefaqs.gamespot.com/boards/189706-nintendo-switch/80756811?page=8)  
51. COOL TEXT EFFECTS IN UNITY 2020 | TEXTMESH PRO (FREE) \- YouTube, 存取日期：1月 18, 2026， [https://www.youtube.com/watch?v=FgWVW2PL1bQ](https://www.youtube.com/watch?v=FgWVW2PL1bQ)  
52. The Last of Us Part II: Accessibility features detailed \- PlayStation.Blog, 存取日期：1月 18, 2026， [https://blog.playstation.com/2020/06/09/the-last-of-us-part-ii-accessibility-features-detailed/](https://blog.playstation.com/2020/06/09/the-last-of-us-part-ii-accessibility-features-detailed/)  
53. Web Content Accessibility Guidelines (WCAG) 2.1 \- W3C, 存取日期：1月 18, 2026， [https://www.w3.org/TR/WCAG21/](https://www.w3.org/TR/WCAG21/)  
54. Using Kinetic Typography to Convey Emotion in Text-Based Interpersonal Communication \- CMU School of Computer Science, 存取日期：1月 18, 2026， [https://www.cs.cmu.edu/\~joonhwan/documents/p41-lee.pdf](https://www.cs.cmu.edu/~joonhwan/documents/p41-lee.pdf)