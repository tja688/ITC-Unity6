using System;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dott.Editor
{
    /// <summary>
    /// Standalone EditorWindow for DOTweenTimeline editing.
    /// Provides a dedicated window interface for timeline editing instead of Inspector-based editing.
    /// </summary>
    public class DOTweenTimelineWindow : EditorWindow
    {
        [MenuItem("Tools/DOTween/Timeline Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<DOTweenTimelineWindow>();
            window.titleContent = new GUIContent("DOTween Timeline",
                EditorGUIUtility.IconContent("d_UnityEditor.Timeline.TimelineWindow").image);
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        // State
        private DOTweenTimeline currentTimeline;
        private DottController controller;
        private DottSelection selection;
        private DottView view;
        private IDOTweenAnimation[] animations = Array.Empty<IDOTweenAnimation>();
        private float? dragTweenTimeShift;
        private Vector2 scrollPosition;
        private bool isLocked;

        // Constants
        private const float TOOLBAR_HEIGHT = 21f;

        private void OnEnable()
        {
            controller = new DottController();
            selection = new DottSelection();
            view = new DottView();

            view.IsSnapping = EditorPrefs.GetBool("Dott.Snap", true);

            BindViewEvents();

            Selection.selectionChanged += OnSelectionChanged;
            Undo.undoRedoPerformed += OnUndoRedo;
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // Initialize with current selection
            OnSelectionChanged();
        }

        private void OnDisable()
        {
            UnbindViewEvents();

            Selection.selectionChanged -= OnSelectionChanged;
            Undo.undoRedoPerformed -= OnUndoRedo;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

            controller?.Dispose();
            controller = null;

            selection?.Dispose();
            selection = null;

            view = null;
            animations = null;
        }

        private void BindViewEvents()
        {
            view.TweenSelected += OnTweenSelected;
            view.TweenDrag += DragSelectedAnimation;

            view.TimeDragEnd += OnTimeDragEnd;
            view.TimeDrag += GoTo;
            view.PreviewDisabled += controller.Stop;

            view.AddClicked += AddAnimation;
            view.AddMore += AddMore;
            view.RemoveClicked += Remove;
            view.DuplicateClicked += Duplicate;

            view.PlayClicked += Play;
            view.StopClicked += controller.Stop;
            view.PlayBackwardsClicked += PlayBackwards;
            view.RewindClicked += Rewind;
            view.FlipClicked += Flip;
            view.LoopToggled += ToggleLoop;
            view.SnapToggled += ToggleSnap;

            view.InspectorUpButtonClicked += MoveSelectedUp;
            view.InspectorDownButtonClicked += MoveSelectedDown;
        }

        private void UnbindViewEvents()
        {
            view.TweenSelected -= OnTweenSelected;
            view.TweenDrag -= DragSelectedAnimation;

            view.TimeDragEnd -= OnTimeDragEnd;
            view.TimeDrag -= GoTo;
            view.PreviewDisabled -= controller.Stop;

            view.AddClicked -= AddAnimation;
            view.AddMore -= AddMore;
            view.RemoveClicked -= Remove;
            view.DuplicateClicked -= Duplicate;

            view.PlayClicked -= Play;
            view.StopClicked -= controller.Stop;
            view.PlayBackwardsClicked -= PlayBackwards;
            view.RewindClicked -= Rewind;
            view.FlipClicked -= Flip;
            view.LoopToggled -= ToggleLoop;
            view.SnapToggled -= ToggleSnap;

            view.InspectorUpButtonClicked -= MoveSelectedUp;
            view.InspectorDownButtonClicked -= MoveSelectedDown;
        }

        private void OnGUI()
        {
            DrawToolbar();
            HandleDragAndDrop();

            if (currentTimeline == null)
            {
                DrawNoTimelineMessage();
                return;
            }

            // Validate timeline
            currentTimeline.OnValidate();

            // 使用GetComponentsInChildren收集所有子对象下的动画组件
            animations = currentTimeline.GetComponentsInChildren<MonoBehaviour>(includeInactive: true)
                .Select(DottAnimation.FromComponent)
                .Where(animation => animation != null)
                .ToArray();
            selection.Validate(animations);

            // Main content area with scroll
            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scroll.scrollPosition;

                // Draw timeline
                view.DrawTimeline(animations, selection.Animation, controller.IsPlaying,
                    controller.ElapsedTime, controller.Loop, controller.Paused);

                // Draw inspector for selected animation
                if (selection.Animation != null)
                {
                    view.DrawInspector(selection.GetAnimationEditor());
                }
            }

            // Preview update
            if (controller.Paused && Event.current.type == EventType.Repaint)
            {
                controller.GoTo(animations, controller.ElapsedTime);
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                // Timeline object field
                EditorGUI.BeginChangeCheck();
                var newTimeline = (DOTweenTimeline)EditorGUILayout.ObjectField(
                    currentTimeline, typeof(DOTweenTimeline), true,
                    GUILayout.Width(200));
                if (EditorGUI.EndChangeCheck() && !isLocked)
                {
                    currentTimeline = newTimeline;
                }

                GUILayout.FlexibleSpace();

                // Ping button - locate in hierarchy
                using (new EditorGUI.DisabledScope(currentTimeline == null))
                {
                    if (GUILayout.Button("Ping", EditorStyles.toolbarButton, GUILayout.Width(40)))
                    {
                        EditorGUIUtility.PingObject(currentTimeline);
                        Selection.activeGameObject = currentTimeline.gameObject;
                    }
                }

                // Lock toggle
                var lockIcon = isLocked
                    ? EditorGUIUtility.IconContent("IN LockButton on")
                    : EditorGUIUtility.IconContent("IN LockButton");
                isLocked = GUILayout.Toggle(isLocked, lockIcon, EditorStyles.toolbarButton,
                    GUILayout.Width(24));
            }
        }

        private void DrawNoTimelineMessage()
        {
            var rect = new Rect(0, TOOLBAR_HEIGHT, position.width, position.height - TOOLBAR_HEIGHT);

            var messageStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 14,
                wordWrap = true
            };

            GUILayout.BeginArea(rect);
            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(300)))
                {
                    GUILayout.Label("No Timeline Selected", messageStyle);
                    GUILayout.Space(10);
                    GUILayout.Label("Select a GameObject with DOTweenTimeline component,\nor drag one into this window.", messageStyle);

                    GUILayout.Space(20);
                    if (GUILayout.Button("Create New Timeline", GUILayout.Height(30)))
                    {
                        CreateNewTimeline();
                    }
                }
                GUILayout.FlexibleSpace();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }

        private void CreateNewTimeline()
        {
            var go = new GameObject("DOTweenTimeline");
            go.AddComponent<DOTweenTimeline>();
            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create DOTween Timeline");
            currentTimeline = go.GetComponent<DOTweenTimeline>();
        }

        private void HandleDragAndDrop()
        {
            if (isLocked) return;

            var evt = Event.current;
            var dropArea = new Rect(0, TOOLBAR_HEIGHT, position.width, position.height - TOOLBAR_HEIGHT);

            if (!dropArea.Contains(evt.mousePosition)) return;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                    var hasTimeline = DragAndDrop.objectReferences
                        .OfType<GameObject>()
                        .Any(go => go.GetComponent<DOTweenTimeline>() != null);

                    if (hasTimeline)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        evt.Use();
                    }
                    break;

                case EventType.DragPerform:
                    var timeline = DragAndDrop.objectReferences
                        .OfType<GameObject>()
                        .Select(go => go.GetComponent<DOTweenTimeline>())
                        .FirstOrDefault(t => t != null);

                    if (timeline != null)
                    {
                        currentTimeline = timeline;
                        DragAndDrop.AcceptDrag();
                        evt.Use();
                        Repaint();
                    }
                    break;
            }
        }

        #region Selection & Updates

        private void OnSelectionChanged()
        {
            if (isLocked) return;

            var go = Selection.activeGameObject;
            if (go != null)
            {
                var timeline = go.GetComponent<DOTweenTimeline>();
                if (timeline != null)
                {
                    currentTimeline = timeline;
                    Repaint();
                }
            }
        }

        private void OnUndoRedo()
        {
            Repaint();
        }

        private void OnEditorUpdate()
        {
            // Continuous repaint when playing or dragging
            if (controller != null && (controller.IsPlaying || view.IsTimeDragging || view.IsTweenDragging))
            {
                Repaint();
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                controller?.Stop();
            }
        }

        #endregion

        #region Timeline Actions (copied from DOTweenTimelineEditor)

        private void Play()
        {
            controller.Play(animations);
        }

        private void GoTo(float time)
        {
            controller.GoTo(animations, time);
        }

        private void PlayBackwards()
        {
            controller.PlayBackwards(animations);
        }

        private void Rewind()
        {
            controller.GoTo(animations, 0f);
            controller.Pause();
        }

        private void Flip()
        {
            // 翻转方向逻辑在运行时有效，编辑器预览时先跳到结尾
            var duration = CalculateTotalDuration(animations);
            var currentTime = controller.ElapsedTime;
            var newTime = duration - currentTime;
            controller.GoTo(animations, Mathf.Max(0f, newTime));
            controller.Pause();
        }

        private float CalculateTotalDuration(IDOTweenAnimation[] anims)
        {
            if (anims == null || anims.Length == 0) return 0f;
            float maxDuration = 0f;
            foreach (var anim in anims)
            {
                var loops = Mathf.Max(1, anim.Loops);
                var fullDuration = anim.Delay + anim.Duration * loops;
                if (fullDuration > maxDuration)
                    maxDuration = fullDuration;
            }
            return maxDuration;
        }

        private void OnTimeDragEnd(Event mouseEvent)
        {
            const int mouseButtonMiddle = 2;
            if (mouseEvent.IsRightMouseButton() || mouseEvent.button == mouseButtonMiddle)
            {
                controller.Stop();
                return;
            }

            controller.Pause();
        }

        private void DragSelectedAnimation(float time)
        {
            if (selection.Animation == null) return;

            Undo.RecordObject(selection.Animation.Component, $"Drag {selection.Animation.Label}");

            dragTweenTimeShift ??= time - selection.Animation.Delay;

            var delay = time - dragTweenTimeShift.Value;
            delay = Mathf.Max(0, delay);
            delay = TrySnapTime(selection.Animation, delay, view.TimeScale);
            delay = (float)Math.Round(delay, 2);
            selection.Animation.Delay = delay;

            Undo.FlushUndoRecordObjects();
        }

        private float TrySnapTime(IDOTweenAnimation target, float newDelay, float timeScale)
        {
            if (!IsSnapActive() || animations.Length < 2)
            {
                return newDelay;
            }

            var snapThreshold = 1f / 40f / timeScale;
            var snapPoints = animations
                .Where(animation => animation.Component != target.Component)
                .SelectMany(animation => new[] { animation.Delay, animation.Delay + animation.Duration * Mathf.Max(1, animation.Loops) })
                .Distinct().ToArray();

            var snapTime = snapPoints.OrderBy(snapPoint => Mathf.Abs(snapPoint - newDelay)).First();
            if (Math.Abs(snapTime - newDelay) < snapThreshold)
            {
                return snapTime;
            }

            if (target.Loops == -1)
            {
                return newDelay;
            }

            var targetFullDuration = target.Duration * Mathf.Max(1, target.Loops);
            var newEndTime = newDelay + targetFullDuration;
            var snapEndTime = snapPoints.OrderBy(snapPoint => Mathf.Abs(snapPoint - newEndTime)).First();
            if (Math.Abs(snapEndTime - newEndTime) < snapThreshold)
            {
                return snapEndTime - targetFullDuration;
            }

            return newDelay;
        }

        private bool IsSnapActive()
        {
            var reverseSnap = Event.current.control;
            var snapEnabled = view.IsSnapping;
            return reverseSnap ? !snapEnabled : snapEnabled;
        }

        private void OnTweenSelected(IDOTweenAnimation animation)
        {
            selection.Set(animation);
            GUIUtility.keyboardControl = 0;
            dragTweenTimeShift = null;
        }

        private void AddAnimation()
        {
            if (currentTimeline == null) return;
            Add(currentTimeline, typeof(DOTweenAnimation));
        }

        private void AddMore(Type type)
        {
            if (currentTimeline == null) return;
            Add(currentTimeline, type);
        }

        private void Add(DOTweenTimeline timeline, Type type)
        {
            var component = ObjectFactory.AddComponent(timeline.gameObject, type);
            var animation = DottAnimation.FromComponent(component);
            if (controller.Paused)
            {
                animation!.Delay = (float)Math.Round(controller.ElapsedTime, 2);
            }

            selection.Set(animation);
        }

        private void Remove()
        {
            if (selection.Animation == null) return;
            Undo.DestroyObjectImmediate(selection.Animation.Component);
            selection.Clear();
        }

        private void Duplicate()
        {
            if (selection.Animation == null) return;

            Undo.SetCurrentGroupName($"Duplicate {selection.Animation.Label}");

            var source = selection.Animation.Component;
            var dest = Undo.AddComponent(source.gameObject, source.GetType());
            EditorUtility.CopySerialized(source, dest);

            var animation = DottAnimation.FromComponent(dest);
            selection.Set(animation);

            var components = source.GetComponents<Component>();
            var targetIndex = Array.IndexOf(components, source) + 1;
            var index = Array.IndexOf(components, dest);
            while (index > targetIndex)
            {
                ComponentUtility.MoveComponentUp(dest);
                index--;
            }
        }

        private void ToggleLoop(bool value)
        {
            controller.Loop = value;
        }

        private void ToggleSnap()
        {
            EditorPrefs.SetBool("Dott.Snap", view.IsSnapping);
        }

        private void MoveSelectedUp()
        {
            if (selection.Animation == null) return;
            var index = animations.FindIndex(animation => animation.Component == selection.Animation.Component);
            if (index > 0)
            {
                ComponentUtility.MoveComponentUp(selection.Animation.Component);
            }
        }

        private void MoveSelectedDown()
        {
            if (selection.Animation == null) return;
            var index = animations.FindIndex(animation => animation.Component == selection.Animation.Component);
            if (index < animations.Length - 1)
            {
                ComponentUtility.MoveComponentDown(selection.Animation.Component);
            }
        }

        #endregion
    }
}
