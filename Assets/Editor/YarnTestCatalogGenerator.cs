using System.Collections.Generic;
using ITC.Dialogue.Testing;
using UnityEditor;
using UnityEngine;

public static class YarnTestCatalogGenerator
{
    [MenuItem("Tools/ITC/Generate Yarn Test Catalog")]
    public static void GenerateCatalog()
    {
        string path = "Assets/Doc/ITC Doc/dialogue/ITC_YarnWorkSpace/YarnTestCatalog.asset";

        YarnTestCatalog catalog = AssetDatabase.LoadAssetAtPath<YarnTestCatalog>(path);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<YarnTestCatalog>();
            AssetDatabase.CreateAsset(catalog, path);
        }

        catalog.files = new List<YarnTestFileInfo>
        {
            new YarnTestFileInfo
            {
                yarnFileName = "ITC_序章.yarn",
                startNode = "ITC_Start",
                endNode = "ITC_Prologue_Scene7_GuardHenet",
                description = "游戏序章：主角在地狱旅馆醒来，认识门童，穿越火焰门进入圣纽约市，来到ITC门口遇见老板。",
                commandsUsed = "itc_bg, itc_npc_main, itc_npc_avatar",
                variableNodes = new List<string>()
            },
            new YarnTestFileInfo
            {
                yarnFileName = "ITC_第一天上班前.yarn",
                startNode = "ITC_BeforeWork_Scene1_Day1Transition",
                endNode = "ITC_BeforeWork_Scene22_AtDesk",
                description = "第一天报到：进入大楼大厅，到6楼面见Henet分配到13号窗口，接受骷髅教学，认识Old Tom和Veer。",
                commandsUsed = "itc_bg, itc_npc_main, itc_npc_avatar, itc_pc_avatar",
                variableNodes = new List<string>()
            },
            new YarnTestFileInfo
            {
                yarnFileName = "ITC_第一天上班签约.yarn",
                startNode = "Sign_Day1_Start",
                endNode = "Ending_Placeholder",
                description = "第一天签约小游戏环节：接待4个客户（醉汉、生病矿工、假酒保、武器专家），审核契约并通过小游戏验证盖章和取魂，期间有上司Henet的电话监测考核。",
                commandsUsed = "itc_bg, itc_npc_main, itc_npc_avatar, itc_pc_avatar, jump",
                variableNodes = new List<string>()
            }
        };

        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ITC] Successfully generated/updated YarnTestCatalog at: " + path);
    }
}
