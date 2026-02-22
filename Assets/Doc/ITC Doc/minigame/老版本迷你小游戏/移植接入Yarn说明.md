# Markdown

接入当前的 Yarn 小游戏触发流程
为确保老项目的这段游戏逻辑能够正常嵌入到你现有的 Yarn 节点当中运行，我直接为 

SigningFlowManager.cs
 的核心触发方法实现了原生的 YarnCommand 封装： 现在，你的 Yarn 脚本中可以直接写入以下指令运行这套老业务，并在小游戏完成前自定挂起当前的对话（等待完成消息后继续）：

<<he_doc_review>> (唤起老版文书核验)
<<he_rune_typing>> (唤起老版符文输入)
<<he_stamp_select>> (唤起老版盖章游戏)
<<he_soul_collect>> (唤起老版收取灵魂)
<<he_special_event>> (唤起老版特殊事件段落) (注：前缀使用 he_ 是为了区别于目前新大地图体系已开发的 itc_doc_review、itc_stamp_select 等通用命令向后兼容)