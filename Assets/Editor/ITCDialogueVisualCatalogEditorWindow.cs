using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITC.Dialogue;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ITC.Editor
{
    public sealed class ITCDialogueVisualCatalogEditorWindow : EditorWindow
    {
        private const string DefaultCatalogPath = "Assets/Resources/Dialogue/DialogueVisualCatalog.asset";
        private const string PlannerGuidePath = "Docs/ITCDialogueVisualCatalogPlannerGuide.md";

        private static readonly string[] SlotFilterOptions =
            new[] { "All", "Background", "NpcMain", "NpcAvatar", "PcAvatar" };

        private DialogueVisualCatalog catalog;
        private SerializedObject serializedCatalog;
        private SerializedProperty mappingsProperty;
        private Vector2 mappingScrollPosition;

        private string searchKeyword = string.Empty;
        private int slotFilterIndex;

        private DialogueVisualSlot batchImportSlot = DialogueVisualSlot.NpcAvatar;
        private string batchKeyPrefix = string.Empty;
        private bool batchUseSlotDefaultFallback = true;
        private bool batchApplyDefaultBundle = true;
        private bool batchReplaceExistingByKey = true;

        private bool showHeaderGuide = true;
        private bool showConfigHelp = true;

        [MenuItem("Tools/ITC/Dialogue/Open Visual Catalog Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<ITCDialogueVisualCatalogEditorWindow>("Dialogue Visual Catalog");
            window.minSize = new Vector2(980f, 660f);
            window.Show();
        }

        private void OnEnable()
        {
            ResolveCatalogReference();
            RebindSerializedCatalog();
        }

        private void OnFocus()
        {
            if (catalog == null)
            {
                ResolveCatalogReference();
            }

            if (catalog != null && (serializedCatalog == null || serializedCatalog.targetObject != catalog))
            {
                RebindSerializedCatalog();
            }
        }

        private void OnGUI()
        {
            DrawHeaderGuide();
            DrawCatalogPicker();

            if (catalog == null)
            {
                EditorGUILayout.HelpBox(
                    "DialogueVisualCatalog is not assigned. Use Find/Create buttons above before editing mappings.",
                    MessageType.Warning);
                return;
            }

            if (serializedCatalog == null || mappingsProperty == null)
            {
                RebindSerializedCatalog();
                if (serializedCatalog == null)
                {
                    EditorGUILayout.HelpBox("Failed to bind serialized object.", MessageType.Error);
                    return;
                }
            }

            serializedCatalog.Update();

            DrawGlobalConfiguration();
            EditorGUILayout.Space(6f);
            DrawWorkflowButtons();
            EditorGUILayout.Space(6f);
            DrawBatchImportPanel();
            EditorGUILayout.Space(6f);
            DrawFilterToolbar();
            DrawMappingsList();

            if (serializedCatalog.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(catalog);
            }
        }

        private void DrawHeaderGuide()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showHeaderGuide = EditorGUILayout.Foldout(showHeaderGuide, "Quick Guide", true);
            if (showHeaderGuide)
            {
                EditorGUILayout.HelpBox(
                    "1) Set global defaults first. " +
                    "2) Use drag-and-drop to fill each mapping with Sprite metadata. " +
                    "3) Use Validate/Sync/Apply Labels buttons before build. " +
                    "All fields stay visible: key, slot, bundle, asset, sub-sprite, fallback, editor path.",
                    MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawCatalogPicker()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Catalog Asset", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var picked = (DialogueVisualCatalog)EditorGUILayout.ObjectField(
                "DialogueVisualCatalog",
                catalog,
                typeof(DialogueVisualCatalog),
                false);
            if (EditorGUI.EndChangeCheck())
            {
                catalog = picked;
                RebindSerializedCatalog();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Find Catalog In Project", GUILayout.Height(24f)))
            {
                ResolveCatalogReference(forceSearch: true);
                RebindSerializedCatalog();
            }

            if (GUILayout.Button("Create Default Catalog", GUILayout.Height(24f)))
            {
                CreateDefaultCatalogAsset();
                RebindSerializedCatalog();
            }

            if (GUILayout.Button("Ping Asset", GUILayout.Height(24f)) && catalog != null)
            {
                EditorGUIUtility.PingObject(catalog);
                Selection.activeObject = catalog;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawGlobalConfiguration()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showConfigHelp = EditorGUILayout.Foldout(showConfigHelp, "Global Configuration", true);

            if (showConfigHelp)
            {
                EditorGUILayout.HelpBox(
                    "Default bundles apply when mapping.assetBundleName is empty. " +
                    "Default keys are per-slot fallback roots. Missing sprite fallback is used only when all key fallback attempts fail.",
                    MessageType.None);
            }

            DrawPropertyWithHint("defaultBackgroundBundle", "Default Bundle - Background", "Fallback bundle for slot Background.");
            DrawPropertyWithHint("defaultPortraitBundle", "Default Bundle - Portrait", "Fallback bundle for NpcMain/NpcAvatar/PcAvatar.");
            DrawPropertyWithHint("npcMainPortraitDefaultKey", "Default Key - NpcMain", "Used when itc_npc_main key load fails.");
            DrawPropertyWithHint("npcAvatarDefaultKey", "Default Key - NpcAvatar", "Used when itc_npc_avatar key load fails.");
            DrawPropertyWithHint("pcAvatarDefaultKey", "Default Key - PcAvatar", "Used when itc_pc_avatar key load fails.");
            DrawPropertyWithHint("missingSpriteBundle", "Missing Bundle", "Global missing image bundle.");
            DrawPropertyWithHint("missingSpriteAssetName", "Missing Asset Name", "Main asset name inside bundle.");
            DrawPropertyWithHint("missingSpriteSubSpriteName", "Missing Sub Sprite", "Required when missing image is Multiple Sprite.");
#if UNITY_EDITOR
            DrawPropertyWithHint("missingSpriteEditorAssetPath", "Missing Editor Asset Path", "Project path used in editor simulation.");
#endif
            EditorGUILayout.EndVertical();
        }

        private void DrawWorkflowButtons()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Workflow Shortcuts", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Recommended sequence: Sync Keys From Yarn -> Fill mappings by drag-and-drop -> Validate Visual Catalog -> Apply AssetBundle Labels -> Build WebGL Bundles.",
                MessageType.None);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Validate Visual Catalog", GUILayout.Height(24f)))
            {
                ITCDialogueVisualCatalogTools.ValidateVisualCatalogMenu();
            }

            if (GUILayout.Button("Sync Keys From Yarn", GUILayout.Height(24f)))
            {
                ITCDialogueVisualCatalogTools.SyncKeysFromYarnMenu();
                ResolveCatalogReference(forceSearch: true);
                RebindSerializedCatalog();
            }

            if (GUILayout.Button("Apply AssetBundle Labels", GUILayout.Height(24f)))
            {
                ITCDialogueResKitBuildTools.ApplyDialogueAssetBundleLabels();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build WebGL ResKit Bundles", GUILayout.Height(24f)))
            {
                ITCDialogueResKitBuildTools.BuildWebGLResKitBundles();
            }

            if (GUILayout.Button("Open Planner Guide", GUILayout.Height(24f)))
            {
                OpenPlannerGuide();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawBatchImportPanel()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Batch Import (Drag Sprites / Textures / Folders)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Batch import creates or replaces mappings in the selected slot. " +
                "Each dropped Sprite keeps its import metadata by writing editorAssetPath + assetName + subSpriteName.",
                MessageType.None);

            batchImportSlot = (DialogueVisualSlot)EditorGUILayout.EnumPopup("Target Slot", batchImportSlot);
            batchKeyPrefix = EditorGUILayout.TextField("Key Prefix (optional)", batchKeyPrefix);
            batchUseSlotDefaultFallback = EditorGUILayout.Toggle("Set Fallback To Slot Default", batchUseSlotDefaultFallback);
            batchApplyDefaultBundle = EditorGUILayout.Toggle("Fill Bundle With Slot Default", batchApplyDefaultBundle);
            batchReplaceExistingByKey = EditorGUILayout.Toggle("Replace Existing Same Slot+Key", batchReplaceExistingByKey);

            var dropRect = GUILayoutUtility.GetRect(0f, 70f, GUILayout.ExpandWidth(true));
            GUI.Box(dropRect, "Drop Sprite / Texture2D / Folder Here");
            HandleBatchDropArea(dropRect);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import From Current Selection", GUILayout.Height(24f)))
            {
                ImportFromObjects(Selection.objects);
            }

            if (GUILayout.Button("Add Empty Mapping", GUILayout.Height(24f)))
            {
                AddEmptyMapping(batchImportSlot);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawFilterToolbar()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Mapping List", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            slotFilterIndex = EditorGUILayout.Popup("Slot Filter", slotFilterIndex, SlotFilterOptions);
            searchKeyword = EditorGUILayout.TextField("Search", searchKeyword);

            if (GUILayout.Button("Clear", GUILayout.Width(90f)))
            {
                slotFilterIndex = 0;
                searchKeyword = string.Empty;
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawMappingsList()
        {
            if (mappingsProperty == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Mappings: {mappingsProperty.arraySize}", EditorStyles.boldLabel);
            mappingScrollPosition = EditorGUILayout.BeginScrollView(mappingScrollPosition, GUILayout.ExpandHeight(true));

            var hasVisibleRows = false;
            for (var i = 0; i < mappingsProperty.arraySize; i++)
            {
                var mapping = mappingsProperty.GetArrayElementAtIndex(i);
                if (!PassesFilter(mapping))
                {
                    continue;
                }

                hasVisibleRows = true;
                if (DrawMappingRow(i, mapping))
                {
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }

            if (!hasVisibleRows)
            {
                EditorGUILayout.HelpBox("No mapping matches current filters.", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private bool DrawMappingRow(int index, SerializedProperty mappingProperty)
        {
            var keyProperty = mappingProperty.FindPropertyRelative("key");
            var slotProperty = mappingProperty.FindPropertyRelative("slot");
            var assetBundleProperty = mappingProperty.FindPropertyRelative("assetBundleName");
            var assetNameProperty = mappingProperty.FindPropertyRelative("assetName");
            var subSpriteProperty = mappingProperty.FindPropertyRelative("subSpriteName");
            var fallbackProperty = mappingProperty.FindPropertyRelative("fallbackKey");
#if UNITY_EDITOR
            var editorPathProperty = mappingProperty.FindPropertyRelative("editorAssetPath");
#endif

            var slot = (DialogueVisualSlot)slotProperty.enumValueIndex;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                $"#{index}   {slot} / {keyProperty.stringValue}",
                EditorStyles.boldLabel);

            if (GUILayout.Button("Up", GUILayout.Width(52f)) && index > 0)
            {
                mappingsProperty.MoveArrayElement(index, index - 1);
                return true;
            }

            if (GUILayout.Button("Down", GUILayout.Width(52f)) && index < mappingsProperty.arraySize - 1)
            {
                mappingsProperty.MoveArrayElement(index, index + 1);
                return true;
            }

            if (GUILayout.Button("Copy", GUILayout.Width(58f)))
            {
                DuplicateMapping(index);
                return true;
            }

            if (GUILayout.Button("Delete", GUILayout.Width(62f)))
            {
                mappingsProperty.DeleteArrayElementAtIndex(index);
                return true;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(keyProperty, new GUIContent("Key"));
            EditorGUILayout.PropertyField(slotProperty, new GUIContent("Slot"));
            EditorGUILayout.PropertyField(assetBundleProperty, new GUIContent("Asset Bundle Name"));
            EditorGUILayout.PropertyField(assetNameProperty, new GUIContent("Asset Name"));
            EditorGUILayout.PropertyField(subSpriteProperty, new GUIContent("Sub Sprite Name"));
            EditorGUILayout.PropertyField(fallbackProperty, new GUIContent("Fallback Key"));
#if UNITY_EDITOR
            EditorGUILayout.PropertyField(editorPathProperty, new GUIContent("Editor Asset Path"));

            var currentSprite = ResolveSpriteFromMapping(editorPathProperty.stringValue, subSpriteProperty.stringValue);
            EditorGUI.BeginChangeCheck();
            var selectedSprite = (Sprite)EditorGUILayout.ObjectField(
                new GUIContent("Source Sprite (Drag To Auto Fill)"),
                currentSprite,
                typeof(Sprite),
                false);
            if (EditorGUI.EndChangeCheck() && selectedSprite != null)
            {
                AutoFillMappingFromSprite(mappingProperty, selectedSprite);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill From Current Selection", GUILayout.Height(22f)))
            {
                if (TryGetSelectionSprite(out var selectionSprite))
                {
                    AutoFillMappingFromSprite(mappingProperty, selectionSprite);
                }
                else
                {
                    Debug.LogWarning("[ITCDialogueVisualCatalogEditor] Current selection does not contain a Sprite.");
                }
            }

            if (GUILayout.Button("Ping Source Asset", GUILayout.Height(22f)))
            {
                PingAssetByPath(editorPathProperty.stringValue);
            }
            EditorGUILayout.EndHorizontal();
#endif

            EditorGUILayout.EndVertical();
            return false;
        }

        private void HandleBatchDropArea(Rect dropRect)
        {
            var currentEvent = Event.current;
            if (!dropRect.Contains(currentEvent.mousePosition))
            {
                return;
            }

            if (currentEvent.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                currentEvent.Use();
                return;
            }

            if (currentEvent.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                ImportFromObjects(DragAndDrop.objectReferences);
                currentEvent.Use();
            }
        }

        private void ImportFromObjects(IEnumerable<Object> inputObjects)
        {
            var sprites = CollectSprites(inputObjects).ToList();
            if (sprites.Count == 0)
            {
                Debug.LogWarning("[ITCDialogueVisualCatalogEditor] No Sprite could be resolved from dropped objects.");
                return;
            }

            serializedCatalog.Update();
            var addedCount = 0;
            var replacedCount = 0;

            foreach (var sprite in sprites)
            {
                var generatedKey = BuildImportKey(sprite.name);
                var key = EnsureKeyUniqueOrReplace(generatedKey, batchImportSlot, out var replacedIndex);

                if (replacedIndex >= 0)
                {
                    var mapping = mappingsProperty.GetArrayElementAtIndex(replacedIndex);
                    FillMappingFromSprite(mapping, key, batchImportSlot, sprite);
                    replacedCount++;
                    continue;
                }

                var index = mappingsProperty.arraySize;
                mappingsProperty.InsertArrayElementAtIndex(index);
                var newMapping = mappingsProperty.GetArrayElementAtIndex(index);
                FillMappingFromSprite(newMapping, key, batchImportSlot, sprite);
                addedCount++;
            }

            serializedCatalog.ApplyModifiedProperties();
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();

            Debug.Log(
                $"[ITCDialogueVisualCatalogEditor] Import done. added={addedCount}, replaced={replacedCount}, slot={batchImportSlot}");
            Repaint();
        }

        private void AddEmptyMapping(DialogueVisualSlot slot)
        {
            if (serializedCatalog == null || mappingsProperty == null)
            {
                return;
            }

            serializedCatalog.Update();
            var index = mappingsProperty.arraySize;
            mappingsProperty.InsertArrayElementAtIndex(index);
            var mapping = mappingsProperty.GetArrayElementAtIndex(index);

            mapping.FindPropertyRelative("key").stringValue = EnsureUniqueKey(slot, "new_key");
            mapping.FindPropertyRelative("slot").enumValueIndex = (int)slot;
            mapping.FindPropertyRelative("assetName").stringValue = string.Empty;
            mapping.FindPropertyRelative("subSpriteName").stringValue = string.Empty;
            mapping.FindPropertyRelative("assetBundleName").stringValue = batchApplyDefaultBundle
                ? GetDefaultBundleForSlot(slot)
                : string.Empty;
            mapping.FindPropertyRelative("fallbackKey").stringValue = batchUseSlotDefaultFallback
                ? GetDefaultKeyForSlot(slot)
                : string.Empty;
#if UNITY_EDITOR
            mapping.FindPropertyRelative("editorAssetPath").stringValue = string.Empty;
#endif
            serializedCatalog.ApplyModifiedProperties();
            EditorUtility.SetDirty(catalog);
            GUI.FocusControl(null);
        }

        private void AutoFillMappingFromSprite(SerializedProperty mappingProperty, Sprite sourceSprite)
        {
            if (mappingProperty == null || sourceSprite == null)
            {
                return;
            }

            var slot = (DialogueVisualSlot)mappingProperty.FindPropertyRelative("slot").enumValueIndex;
            var currentKey = mappingProperty.FindPropertyRelative("key").stringValue;
            if (string.IsNullOrWhiteSpace(currentKey))
            {
                currentKey = EnsureUniqueKey(slot, BuildImportKey(sourceSprite.name));
            }

            FillMappingFromSprite(mappingProperty, currentKey, slot, sourceSprite);
        }

        private void FillMappingFromSprite(
            SerializedProperty mappingProperty,
            string key,
            DialogueVisualSlot slot,
            Sprite sourceSprite)
        {
            var assetPath = AssetDatabase.GetAssetPath(sourceSprite);
            var mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            var spriteCandidates = LoadSpritesAtPath(assetPath);
            var hasMultipleSprites = spriteCandidates.Length > 1;

            mappingProperty.FindPropertyRelative("key").stringValue = key;
            mappingProperty.FindPropertyRelative("slot").enumValueIndex = (int)slot;
            mappingProperty.FindPropertyRelative("assetName").stringValue =
                mainAsset != null ? mainAsset.name : Path.GetFileNameWithoutExtension(assetPath);
            mappingProperty.FindPropertyRelative("subSpriteName").stringValue =
                hasMultipleSprites ? sourceSprite.name : string.Empty;

            var bundleProperty = mappingProperty.FindPropertyRelative("assetBundleName");
            if (batchApplyDefaultBundle || string.IsNullOrWhiteSpace(bundleProperty.stringValue))
            {
                bundleProperty.stringValue = GetDefaultBundleForSlot(slot);
            }

            var fallbackProperty = mappingProperty.FindPropertyRelative("fallbackKey");
            if (batchUseSlotDefaultFallback && string.IsNullOrWhiteSpace(fallbackProperty.stringValue))
            {
                var slotDefault = GetDefaultKeyForSlot(slot);
                fallbackProperty.stringValue = string.Equals(slotDefault, key, StringComparison.OrdinalIgnoreCase)
                    ? string.Empty
                    : slotDefault;
            }

#if UNITY_EDITOR
            mappingProperty.FindPropertyRelative("editorAssetPath").stringValue = assetPath;
#endif
        }

        private void DuplicateMapping(int index)
        {
            mappingsProperty.InsertArrayElementAtIndex(index);
            var original = mappingsProperty.GetArrayElementAtIndex(index + 1);
            var duplicate = mappingsProperty.GetArrayElementAtIndex(index);
            CopyMapping(duplicate, original);

            var slot = (DialogueVisualSlot)duplicate.FindPropertyRelative("slot").enumValueIndex;
            var keyProperty = duplicate.FindPropertyRelative("key");
            keyProperty.stringValue = EnsureUniqueKey(slot, $"{keyProperty.stringValue}_copy");
        }

        private static void CopyMapping(SerializedProperty target, SerializedProperty source)
        {
            target.FindPropertyRelative("key").stringValue = source.FindPropertyRelative("key").stringValue;
            target.FindPropertyRelative("slot").enumValueIndex = source.FindPropertyRelative("slot").enumValueIndex;
            target.FindPropertyRelative("assetName").stringValue = source.FindPropertyRelative("assetName").stringValue;
            target.FindPropertyRelative("subSpriteName").stringValue = source.FindPropertyRelative("subSpriteName").stringValue;
            target.FindPropertyRelative("assetBundleName").stringValue = source.FindPropertyRelative("assetBundleName").stringValue;
            target.FindPropertyRelative("fallbackKey").stringValue = source.FindPropertyRelative("fallbackKey").stringValue;
#if UNITY_EDITOR
            target.FindPropertyRelative("editorAssetPath").stringValue = source.FindPropertyRelative("editorAssetPath").stringValue;
#endif
        }

        private bool PassesFilter(SerializedProperty mappingProperty)
        {
            var key = mappingProperty.FindPropertyRelative("key").stringValue ?? string.Empty;
            var slot = (DialogueVisualSlot)mappingProperty.FindPropertyRelative("slot").enumValueIndex;
            var assetName = mappingProperty.FindPropertyRelative("assetName").stringValue ?? string.Empty;
            var fallback = mappingProperty.FindPropertyRelative("fallbackKey").stringValue ?? string.Empty;

            if (slotFilterIndex > 0 && slot != (DialogueVisualSlot)(slotFilterIndex - 1))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(searchKeyword))
            {
                return true;
            }

            var token = searchKeyword.Trim();
            return key.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0
                   || assetName.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0
                   || fallback.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private string EnsureKeyUniqueOrReplace(string key, DialogueVisualSlot slot, out int replacedIndex)
        {
            replacedIndex = FindMappingIndex(slot, key);
            if (replacedIndex >= 0)
            {
                if (batchReplaceExistingByKey)
                {
                    return key;
                }

                replacedIndex = -1;
                return EnsureUniqueKey(slot, key);
            }

            return key;
        }

        private int FindMappingIndex(DialogueVisualSlot slot, string key)
        {
            if (mappingsProperty == null || string.IsNullOrWhiteSpace(key))
            {
                return -1;
            }

            for (var i = 0; i < mappingsProperty.arraySize; i++)
            {
                var mapping = mappingsProperty.GetArrayElementAtIndex(i);
                var slotValue = (DialogueVisualSlot)mapping.FindPropertyRelative("slot").enumValueIndex;
                var keyValue = mapping.FindPropertyRelative("key").stringValue;
                if (slotValue == slot && string.Equals(keyValue, key, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private string EnsureUniqueKey(DialogueVisualSlot slot, string desiredKey)
        {
            var normalized = NormalizeKey(desiredKey);
            if (FindMappingIndex(slot, normalized) < 0)
            {
                return normalized;
            }

            var suffix = 2;
            while (true)
            {
                var candidate = $"{normalized}_{suffix}";
                if (FindMappingIndex(slot, candidate) < 0)
                {
                    return candidate;
                }

                suffix++;
            }
        }

        private string BuildImportKey(string sourceName)
        {
            var combined = string.IsNullOrWhiteSpace(batchKeyPrefix)
                ? sourceName
                : $"{batchKeyPrefix}_{sourceName}";
            return NormalizeKey(combined);
        }

        private static string NormalizeKey(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "visual_key";
            }

            var chars = input.Trim().ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (char.IsWhiteSpace(chars[i]) || chars[i] == '-')
                {
                    chars[i] = '_';
                }
            }

            var normalized = new string(chars).Trim('_').ToLowerInvariant();
            while (normalized.Contains("__"))
            {
                normalized = normalized.Replace("__", "_");
            }

            return string.IsNullOrWhiteSpace(normalized) ? "visual_key" : normalized;
        }

        private string GetDefaultBundleForSlot(DialogueVisualSlot slot)
        {
            if (serializedCatalog == null)
            {
                return string.Empty;
            }

            var background = serializedCatalog.FindProperty("defaultBackgroundBundle")?.stringValue;
            var portrait = serializedCatalog.FindProperty("defaultPortraitBundle")?.stringValue;
            return slot == DialogueVisualSlot.Background ? background : portrait;
        }

        private string GetDefaultKeyForSlot(DialogueVisualSlot slot)
        {
            if (serializedCatalog == null)
            {
                return string.Empty;
            }

            return slot switch
            {
                DialogueVisualSlot.NpcMain => serializedCatalog.FindProperty("npcMainPortraitDefaultKey")?.stringValue,
                DialogueVisualSlot.NpcAvatar => serializedCatalog.FindProperty("npcAvatarDefaultKey")?.stringValue,
                DialogueVisualSlot.PcAvatar => serializedCatalog.FindProperty("pcAvatarDefaultKey")?.stringValue,
                _ => string.Empty
            };
        }

        private void ResolveCatalogReference(bool forceSearch = false)
        {
            if (!forceSearch && catalog != null)
            {
                return;
            }

            catalog = AssetDatabase.LoadAssetAtPath<DialogueVisualCatalog>(DefaultCatalogPath);
            if (catalog != null)
            {
                return;
            }

            var guids = AssetDatabase.FindAssets("t:DialogueVisualCatalog");
            if (guids == null || guids.Length == 0)
            {
                catalog = null;
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            catalog = AssetDatabase.LoadAssetAtPath<DialogueVisualCatalog>(path);
        }

        private void RebindSerializedCatalog()
        {
            if (catalog == null)
            {
                serializedCatalog = null;
                mappingsProperty = null;
                return;
            }

            serializedCatalog = new SerializedObject(catalog);
            mappingsProperty = serializedCatalog.FindProperty("mappings");
        }

        private void CreateDefaultCatalogAsset()
        {
            if (AssetDatabase.LoadAssetAtPath<DialogueVisualCatalog>(DefaultCatalogPath) != null)
            {
                catalog = AssetDatabase.LoadAssetAtPath<DialogueVisualCatalog>(DefaultCatalogPath);
                Selection.activeObject = catalog;
                return;
            }

            EnsureFolderExists("Assets/Resources");
            EnsureFolderExists("Assets/Resources/Dialogue");

            var created = CreateInstance<DialogueVisualCatalog>();
            AssetDatabase.CreateAsset(created, DefaultCatalogPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            catalog = created;
            Selection.activeObject = created;
            EditorGUIUtility.PingObject(created);
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            var parent = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            var folderName = Path.GetFileName(folderPath);
            if (string.IsNullOrWhiteSpace(parent) || string.IsNullOrWhiteSpace(folderName))
            {
                return;
            }

            EnsureFolderExists(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }

        private static IEnumerable<Sprite> CollectSprites(IEnumerable<Object> inputObjects)
        {
            var spriteSet = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
            if (inputObjects == null)
            {
                return spriteSet.Values;
            }

            foreach (var obj in inputObjects)
            {
                if (obj == null)
                {
                    continue;
                }

                if (obj is Sprite sprite)
                {
                    AddSprite(spriteSet, sprite);
                    continue;
                }

                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                if (AssetDatabase.IsValidFolder(path))
                {
                    CollectSpritesFromFolder(path, spriteSet);
                    continue;
                }

                CollectSpritesFromAssetPath(path, spriteSet);
            }

            return spriteSet.Values;
        }

        private static void CollectSpritesFromFolder(string folderPath, IDictionary<string, Sprite> spriteSet)
        {
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                CollectSpritesFromAssetPath(assetPath, spriteSet);
            }
        }

        private static void CollectSpritesFromAssetPath(string assetPath, IDictionary<string, Sprite> spriteSet)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
            if (sprites.Length == 0)
            {
                var single = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (single != null)
                {
                    AddSprite(spriteSet, single);
                }

                return;
            }

            foreach (var sprite in sprites)
            {
                AddSprite(spriteSet, sprite);
            }
        }

        private static void AddSprite(IDictionary<string, Sprite> spriteSet, Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(sprite);
            var hashKey = $"{assetPath}|{sprite.name}";
            if (!spriteSet.ContainsKey(hashKey))
            {
                spriteSet.Add(hashKey, sprite);
            }
        }

        private static bool TryGetSelectionSprite(out Sprite sprite)
        {
            sprite = Selection.activeObject as Sprite;
            if (sprite != null)
            {
                return true;
            }

            if (Selection.activeObject is Texture2D)
            {
                var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
                return sprite != null;
            }

            return false;
        }

        private Sprite ResolveSpriteFromMapping(string editorAssetPath, string subSpriteName)
        {
            if (string.IsNullOrWhiteSpace(editorAssetPath))
            {
                return null;
            }

            var sprites = LoadSpritesAtPath(editorAssetPath);
            if (sprites.Length == 0)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(subSpriteName))
            {
                var matched = sprites.FirstOrDefault(s =>
                    string.Equals(s.name, subSpriteName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (matched != null)
                {
                    return matched;
                }
            }

            return sprites[0];
        }

        private Sprite[] LoadSpritesAtPath(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return Array.Empty<Sprite>();
            }

            if (AssetDatabase.LoadMainAssetAtPath(assetPath) == null)
            {
                return Array.Empty<Sprite>();
            }

            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
            if (sprites.Length > 0)
            {
                return sprites;
            }

            var single = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            return single != null ? new[] { single } : Array.Empty<Sprite>();
        }

        private static void PingAssetByPath(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return;
            }

            var obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (obj == null)
            {
                return;
            }

            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        private static void OpenPlannerGuide()
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), PlannerGuidePath);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"[ITCDialogueVisualCatalogEditor] Planner guide is missing: {PlannerGuidePath}");
                return;
            }

            var normalized = fullPath.Replace("\\", "/");
            Application.OpenURL($"file:///{normalized}");
        }

        private void DrawPropertyWithHint(string propertyName, string label, string hint)
        {
            var property = serializedCatalog.FindProperty(propertyName);
            if (property == null)
            {
                return;
            }

            EditorGUILayout.PropertyField(property, new GUIContent(label));
            EditorGUILayout.HelpBox(hint, MessageType.None);
        }
    }
}
