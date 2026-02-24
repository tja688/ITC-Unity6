using System.IO;
using UnityEditor;
using UnityEngine;

public static class ReplicaBakeTool
{
    [MenuItem("ReplicaV2/Bake All Requested Effects")]
    public static void BakeAllRequested()
    {
        BakeMagnet();
        BakeAnimatedContent(); // Container
        BakeDecay();
        BakeTiltedCard();
    }

    [MenuItem("ReplicaV2/Bake Magnet Effect")]
    public static void BakeMagnet()
    {
        BakeEffect("Magnet", (root) =>
        {
            var view = MagnetEffectViewBuilder.Build(root, null);
            MagnetEffectViewBuilder.Link(view.Root.gameObject, view);
            return view.Root.gameObject;
        }, typeof(MagnetEffectHostBridge));
    }

    [MenuItem("ReplicaV2/Bake AnimatedContent (Container)")]
    public static void BakeAnimatedContent()
    {
        BakeEffect("AnimatedContent", (root) =>
        {
            var view = AnimatedContentEffectViewBuilder.Build(root, null);
            AnimatedContentEffectViewBuilder.Link(view.Root.gameObject, view);
            return view.Root.gameObject;
        }, typeof(AnimatedContentEffectHostBridge));
    }

    [MenuItem("ReplicaV2/Bake Decay Effect")]
    public static void BakeDecay()
    {
        BakeEffect("Decay", (root) =>
        {
            var view = DecayEffectViewBuilder.Build(root, null);
            DecayEffectViewBuilder.Link(view.Root.gameObject, view);
            return view.Root.gameObject;
        }, typeof(DecayEffectHostBridge));
    }

    [MenuItem("ReplicaV2/Bake TiltedCard Effect")]
    public static void BakeTiltedCard()
    {
        BakeEffect("TiltedCard", (root) =>
        {
            var view = TiltedCardEffectViewBuilder.Build(root, null);
            TiltedCardEffectViewBuilder.Link(view.Root.gameObject, view);
            return view.Root.gameObject;
        }, typeof(TiltedCardEffectHostBridge));
    }

    private static void BakeEffect(string name, System.Func<RectTransform, GameObject> buildAction, System.Type bridgeType)
    {
        var rootGO = new GameObject("BakeRoot");
        var rootRect = rootGO.AddComponent<RectTransform>();

        try
        {
            GameObject effectRoot = buildAction(rootRect);

            SetLayerRecursive(effectRoot, 5); // UI Layer

            var rt = effectRoot.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            /*
            if (bridgeType != null && !effectRoot.GetComponent(bridgeType))
            {
                effectRoot.AddComponent(bridgeType);
            }
            */

            string folder = "Assets/ReplicaV2/Prefabs/Effects";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
            }

            string path = $"{folder}/{name}_Baked.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(effectRoot, path);

            if (prefab != null)
            {
                Debug.Log($"<color=green>Successfully baked {name} to {path}</color>");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            Object.DestroyImmediate(rootGO);
        }
    }

    private static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
}
