using UnityEngine;
using UnityEngine.InputSystem;

namespace ITC.Dialogue
{
    public sealed class SignDialogueHistoryBrowser
    {
        private SignDialogueNpcTrack npcTrack;
        private Canvas rootCanvas;
        private bool pointerWasInside;

        public void Configure(SignDialogueNpcTrack track, Canvas canvas)
        {
            npcTrack = track;
            rootCanvas = canvas;
        }

        public void Tick()
        {
            if (npcTrack == null || !npcTrack.HasHistory || npcTrack.HistoryBrowseRect == null)
            {
                pointerWasInside = false;
                return;
            }

            if (Mouse.current == null)
            {
                return;
            }

            var pointerPosition = Mouse.current.position.ReadValue();
            var eventCamera = rootCanvas != null ? rootCanvas.worldCamera : null;
            var isInside = RectTransformUtility.RectangleContainsScreenPoint(
                npcTrack.HistoryBrowseRect,
                pointerPosition,
                eventCamera);

            if (isInside)
            {
                var scrollValue = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scrollValue) > 0.01f)
                {
                    npcTrack.ScrollHistory(scrollValue > 0f ? -1 : 1);
                }
            }
            else if (pointerWasInside)
            {
                npcTrack.ResetVisibleHistoryToDefault(true);
            }

            pointerWasInside = isInside;
        }
    }
}
