#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ITC.Dialogue.Testing.Editor
{
    public static class SignDialogueCardTransitionPrototypeSetup
    {
        private const string TargetSceneName = "TestScene_sign UI test";
        private const float DefaultSpawnLocalY = -5.6f;

        [MenuItem("ITC/Tests/Setup Sign Dialogue Card Prototype")]
        public static void SetupPrototype()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.isLoaded)
            {
                Debug.LogError("[SignDialogueCardTransitionPrototypeSetup] 当前没有已加载的场景。");
                return;
            }

            if (scene.name != TargetSceneName)
            {
                Debug.LogWarning($"[SignDialogueCardTransitionPrototypeSetup] 当前场景是 {scene.name}，不是 {TargetSceneName}。仍会继续尝试挂载。");
            }

            var pureBackground = FindInScene<RectTransform>("纯背景");
            var mask = FindInScene<RectTransform>("对话框遮罩");
            var slot1 = FindInScene<RectTransform>("对话框1号");
            var slot2 = FindInScene<RectTransform>("对话框2号");
            var legacyRoot = FindInScene<Transform>("签约特制对话系统");
            var textTemplate = FindLineTextTemplate();

            if (pureBackground == null || mask == null || slot1 == null || slot2 == null || textTemplate == null)
            {
                Debug.LogError("[SignDialogueCardTransitionPrototypeSetup] 缺少关键节点，挂载中止。");
                return;
            }

            var slot0 = EnsureSlot0Marker(mask, slot1);
            var component = pureBackground.GetComponent<SignDialogueCardTransitionPrototype>();
            if (component == null)
            {
                component = Undo.AddComponent<SignDialogueCardTransitionPrototype>(pureBackground.gameObject);
            }

            AssignReference(component, "dialogueSlot0Marker", slot0);
            AssignReference(component, "dialogueSlot1Template", slot1);
            AssignReference(component, "dialogueSlot2Template", slot2);
            AssignReference(component, "latestRuntimeParent", slot1.parent as RectTransform);
            AssignReference(component, "historyRuntimeParent", pureBackground);
            AssignReference(component, "textStyleTemplate", textTemplate);
            AssignReference(component, "legacyDialogueSystemRoot", legacyRoot != null ? legacyRoot.gameObject : null);

            EditorUtility.SetDirty(component);
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[SignDialogueCardTransitionPrototypeSetup] 对话卡片原型已挂载到当前场景。");
        }

        private static RectTransform EnsureSlot0Marker(RectTransform mask, RectTransform slot1)
        {
            var existing = FindInScene<RectTransform>("对话框0号");
            if (existing != null)
            {
                existing.SetParent(mask, false);
                existing.anchoredPosition = new Vector2(slot1.anchoredPosition.x, DefaultSpawnLocalY);
                existing.localScale = slot1.localScale;
                existing.localRotation = slot1.localRotation;
                EnsureMarkerRaycastDisabled(existing);
                return existing;
            }

            var markerObject = Object.Instantiate(slot1.gameObject, mask, false);
            Undo.RegisterCreatedObjectUndo(markerObject, "Create 对话框0号");
            markerObject.name = "对话框0号";

            var marker = markerObject.GetComponent<RectTransform>();
            marker.anchoredPosition = new Vector2(slot1.anchoredPosition.x, DefaultSpawnLocalY);
            marker.localScale = slot1.localScale;
            marker.localRotation = slot1.localRotation;

            EnsureMarkerRaycastDisabled(marker);
            return marker;
        }

        private static void EnsureMarkerRaycastDisabled(Component target)
        {
            if (target.TryGetComponent<Image>(out var image))
            {
                image.raycastTarget = false;
            }
        }

        private static void AssignReference(Object target, string propertyName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogWarning($"[SignDialogueCardTransitionPrototypeSetup] 未找到属性 {propertyName}。");
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static T FindInScene<T>(string objectName) where T : Component
        {
            var components = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] != null && components[i].name == objectName)
                {
                    return components[i];
                }
            }

            return null;
        }

        private static TextMeshProUGUI FindLineTextTemplate()
        {
            var texts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                if (text == null || text.transform.parent == null)
                {
                    continue;
                }

                if (text.transform.parent.name == "LineText")
                {
                    return text;
                }
            }

            return null;
        }
    }
}
#endif
