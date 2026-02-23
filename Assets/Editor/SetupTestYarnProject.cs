#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Yarn.Unity;

public class SetupTestYarnProject
{
    [MenuItem("Tools/Setup Test Yarn Project")]
    public static void Setup()
    {
        // 查找场景中的 Dialogue Runner
        var runner = Object.FindAnyObjectByType<DialogueRunner>();
        if (runner == null)
        {
            Debug.LogError("场景中没有找到 DialogueRunner，请先添加一个。");
            return;
        }

        var projectPath = "Assets/Tests/Test dialogue/TestMinigamesProject.yarnproject";
        var project = AssetDatabase.LoadAssetAtPath<YarnProject>(projectPath);
        if (project == null)
        {
            Debug.LogError("未找到 YarnProject，请等待Unity编译导入完成。路径: " + projectPath);
            return;
        }

        // 关联 Yarn Project -> 用 SerializedObject 修改避免编译报错
        var so = new SerializedObject(runner);
        so.FindProperty("yarnProject").objectReferenceValue = project;
        so.ApplyModifiedProperties();

        // 选择起点
        runner.startNode = "Start";

        EditorUtility.SetDirty(runner);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(runner.gameObject.scene);

        Debug.Log("✅ Dialogue Runner 已经成功关联了测试 Yarn Project，且起点的Node已设置为 'Start'！");
    }
}
#endif
