#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITC.Tests.SignPanel.Editor
{
    public static class SignPanelCardFeelPrototypeSetup
    {
        private const string TargetSceneName = "TestScene_sign UI test";
        private const string PrototypeRootName = "纯背景";
        private const string CardName = "对话框1号";
        private const string HiddenAnchorName = "对话框0号";
        private const string MaskName = "对话框遮罩";

        [MenuItem("ITC/Tests/Setup Sign Card Feel Prototype")]
        public static void SetupPrototype()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogError("[SignPanelCardFeelPrototypeSetup] 当前没有已加载的场景。");
                return;
            }

            if (scene.name != TargetSceneName)
            {
                Debug.LogWarning($"[SignPanelCardFeelPrototypeSetup] 当前场景是 {scene.name}，不是 {TargetSceneName}，仍会继续挂载。");
            }

            var prototypeRoot = FindRectTransform(PrototypeRootName);
            var cardRoot = FindRectTransform(CardName);
            var maskRoot = FindRectTransform(MaskName);

            if (prototypeRoot == null || cardRoot == null)
            {
                Debug.LogError("[SignPanelCardFeelPrototypeSetup] 缺少纯背景或对话框1号，无法挂载。");
                return;
            }

            var component = prototypeRoot.GetComponent<ITC.Tests.SignPanel.SignPanelCardFeelPrototype>();
            if (component == null)
            {
                component = Undo.AddComponent<ITC.Tests.SignPanel.SignPanelCardFeelPrototype>(prototypeRoot.gameObject);
            }

            AssignReference(component, "cardRoot", cardRoot);
            AssignReference(component, "hiddenAnchor", EnsureHiddenAnchor(maskRoot, cardRoot));

            EditorUtility.SetDirty(component);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[SignPanelCardFeelPrototypeSetup] 独立原型已挂载到当前场景。");
        }

        private static RectTransform EnsureHiddenAnchor(RectTransform maskRoot, RectTransform cardRoot)
        {
            var existing = FindRectTransform(HiddenAnchorName);
            var parent = cardRoot.parent as RectTransform;
            var anchorParent = parent != null ? parent : maskRoot;

            if (existing != null)
            {
                existing.SetParent(anchorParent, false);
                existing.anchorMin = cardRoot.anchorMin;
                existing.anchorMax = cardRoot.anchorMax;
                existing.pivot = cardRoot.pivot;
                existing.sizeDelta = cardRoot.sizeDelta;
                existing.localRotation = cardRoot.localRotation;
                existing.localScale = cardRoot.localScale;
                existing.anchoredPosition = cardRoot.anchoredPosition + new Vector2(0f, -240f);
                return existing;
            }

            var anchorObject = new GameObject(HiddenAnchorName, typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(anchorObject, "Create 对话框0号");
            anchorObject.transform.SetParent(anchorParent, false);
            anchorObject.name = HiddenAnchorName;

            var anchor = anchorObject.GetComponent<RectTransform>();
            anchor.anchorMin = cardRoot.anchorMin;
            anchor.anchorMax = cardRoot.anchorMax;
            anchor.pivot = cardRoot.pivot;
            anchor.sizeDelta = cardRoot.sizeDelta;
            anchor.localRotation = cardRoot.localRotation;
            anchor.localScale = cardRoot.localScale;
            anchor.anchoredPosition = cardRoot.anchoredPosition + new Vector2(0f, -240f);
            return anchor;
        }

        private static void AssignReference(Object target, string propertyName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogWarning($"[SignPanelCardFeelPrototypeSetup] 未找到属性 {propertyName}。");
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static RectTransform FindRectTransform(string objectName)
        {
            var rectTransforms = Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < rectTransforms.Length; i++)
            {
                var rectTransform = rectTransforms[i];
                if (rectTransform != null && rectTransform.name == objectName)
                {
                    return rectTransform;
                }
            }

            return null;
        }
    }
}
#endif
