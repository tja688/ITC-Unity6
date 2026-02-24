using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TargetCursorEffectHostBridge))]
public class TargetCursorShowcaseHelper : MonoBehaviour
{
    private void Start()
    {
        // Add a few test buttons automatically to test the hover effect
        var host = GetComponent<TargetCursorEffectHostBridge>();

        var panel = ReplicaUIFactoryV2.CreateRect("TestButtonsPanel", host.transform);
        panel.SetSiblingIndex(0); // Move behind the cursor
        ReplicaUIFactoryV2.Stretch(panel);

        Vector2[] positions = { new Vector2(-200, 100), new Vector2(250, 0), new Vector2(-150, -250), new Vector2(100, -200) };
        Vector2[] sizes = { new Vector2(200, 60), new Vector2(150, 150), new Vector2(300, 80), new Vector2(120, 80) };
        string[] labels = { "Hover Me 1", "Square Button", "Wide Button", "Small" };

        for (int i = 0; i < 4; i++)
        {
            var btn = ReplicaUIFactoryV2.CreateButton(labels[i], panel, new Color(0.1f, 0.1f, 0.1f, 0.6f));
            btn.GetComponent<RectTransform>().anchoredPosition = positions[i];
            btn.GetComponent<RectTransform>().sizeDelta = sizes[i];
            btn.gameObject.AddComponent<TargetCursorInteractable>();

            var txt = ReplicaUIFactoryV2.CreateText("Text", btn.transform, labels[i], 24, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            ReplicaUIFactoryV2.Stretch(txt.GetComponent<RectTransform>());
        }
    }
}
