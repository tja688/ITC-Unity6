using ITC.Dialogue;
using ITC.Sign;
using QFramework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace ITC.Editor
{
    public static class SignScenePublisher
    {
        private const string MenuPath = "Tools/ITC/Sign/Publish Production Sign Scene";
        private const string SourceScenePath = "Assets/Tests/Test scene/TestScene_sign UI test.unity";
        private const string TargetScenePath = "Assets/Scenes/SignScene.unity";
        private const string ProductionYarnProjectPath = "Assets/Doc/ITC Doc/dialogue/ITC_YarnWorkSpace/ITC_YARN_Project.yarnproject";
        private const string ProductionStartNode = "Sign_Day1_Start";

        [MenuItem(MenuPath)]
        public static void Publish()
        {
            var previousScene = EditorSceneManager.GetActiveScene();
            if (previousScene.isDirty)
            {
                LogKit.E("[SignScenePublisher] Active scene has unsaved changes. Save or discard them before publishing.");
                return;
            }

            var yarnProject = AssetDatabase.LoadAssetAtPath<YarnProject>(ProductionYarnProjectPath);
            if (yarnProject == null)
            {
                LogKit.E($"[SignScenePublisher] Missing YarnProject: {ProductionYarnProjectPath}");
                return;
            }

            var sourceScene = EditorSceneManager.OpenScene(SourceScenePath, OpenSceneMode.Single);
            if (!sourceScene.IsValid() || !sourceScene.isLoaded)
            {
                LogKit.E($"[SignScenePublisher] Failed to open source scene: {SourceScenePath}");
                return;
            }

            var signRoot = GameObject.Find("SignUI");
            if (signRoot == null)
            {
                LogKit.E("[SignScenePublisher] Source scene is missing SignUI root.");
                return;
            }

            var dialogueRunner = signRoot.GetComponentInChildren<DialogueRunner>(true);
            var bootstrap = signRoot.GetComponent<SignDialogueSceneBootstrap>();
            if (dialogueRunner == null || bootstrap == null)
            {
                LogKit.E("[SignScenePublisher] Source scene is missing DialogueRunner or SignDialogueSceneBootstrap.");
                return;
            }

            RemoveTestDriver(dialogueRunner.gameObject);
            ConfigureDialogueRunner(dialogueRunner, yarnProject);
            ConfigureBootstrap(bootstrap);
            ConfigureLauncher(signRoot, dialogueRunner, yarnProject);

            EditorSceneManager.MarkSceneDirty(sourceScene);

            if (!EditorSceneManager.SaveScene(sourceScene, TargetScenePath, true))
            {
                LogKit.E($"[SignScenePublisher] Failed to save production scene copy to {TargetScenePath}");
                return;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (previousScene.IsValid() && !string.IsNullOrWhiteSpace(previousScene.path))
            {
                EditorSceneManager.OpenScene(previousScene.path, OpenSceneMode.Single);
            }

            LogKit.I("[SignScenePublisher] Production SignScene published from TestScene_sign UI test.");
        }

        private static void RemoveTestDriver(GameObject runnerObject)
        {
            foreach (var component in runnerObject.GetComponents<MonoBehaviour>())
            {
                if (component != null && component.GetType().Name == "SignDialogueTestLoopDriver")
                {
                    Object.DestroyImmediate(component);
                }
            }
        }

        private static void ConfigureDialogueRunner(DialogueRunner dialogueRunner, YarnProject yarnProject)
        {
            var runnerSerializedObject = new SerializedObject(dialogueRunner);
            runnerSerializedObject.Update();

            SetObject(runnerSerializedObject, "yarnProject", yarnProject);
            SetBool(runnerSerializedObject, "autoStart", false);
            SetString(runnerSerializedObject, "startNode", ProductionStartNode);

            runnerSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            dialogueRunner.SetProject(yarnProject);
            dialogueRunner.autoStart = false;
            dialogueRunner.startNode = ProductionStartNode;

            EditorUtility.SetDirty(dialogueRunner);
        }

        private static void ConfigureBootstrap(SignDialogueSceneBootstrap bootstrap)
        {
            var bootstrapSerializedObject = new SerializedObject(bootstrap);
            bootstrapSerializedObject.Update();

            SetBool(bootstrapSerializedObject, "ensureDialogueStarts", false);
            SetString(bootstrapSerializedObject, "fallbackStartNode", ProductionStartNode);

            bootstrapSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(bootstrap);
        }

        private static void ConfigureLauncher(GameObject signRoot, DialogueRunner dialogueRunner, YarnProject yarnProject)
        {
            var launcher = signRoot.GetComponent<SignSceneDialogueLauncher>();
            if (launcher == null)
            {
                launcher = signRoot.AddComponent<SignSceneDialogueLauncher>();
            }

            var launcherSerializedObject = new SerializedObject(launcher);
            launcherSerializedObject.Update();

            SetObject(launcherSerializedObject, "dialogueRunner", dialogueRunner);
            SetObject(launcherSerializedObject, "yarnProject", yarnProject);
            SetBool(launcherSerializedObject, "autoStartOnEnable", true);
            SetFloat(launcherSerializedObject, "startupDelaySeconds", 0.2f);
            SetString(launcherSerializedObject, "startNode", ProductionStartNode);

            launcherSerializedObject.ApplyModifiedPropertiesWithoutUndo();
            launcher.Configure(dialogueRunner, yarnProject, true, 0.2f, ProductionStartNode);
            EditorUtility.SetDirty(launcher);
        }

        private static void SetBool(SerializedObject serializedObject, string propertyName, bool value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null && property.propertyType == SerializedPropertyType.Boolean)
            {
                property.boolValue = value;
            }
        }

        private static void SetFloat(SerializedObject serializedObject, string propertyName, float value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null && property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = value;
            }
        }

        private static void SetString(SerializedObject serializedObject, string propertyName, string value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null && property.propertyType == SerializedPropertyType.String)
            {
                property.stringValue = value;
            }
        }

        private static void SetObject(SerializedObject serializedObject, string propertyName, Object value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null && property.propertyType == SerializedPropertyType.ObjectReference)
            {
                property.objectReferenceValue = value;
            }
        }
    }
}
