using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Linq;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace QFramework
{
    public class ConsoleWindow : MonoBehaviour
    {
        /// <summary>
        /// Update回调
        /// </summary>
        public delegate void OnUpdateCallback();

        /// <summary>
        /// OnGUI回调
        /// </summary>
        public delegate void OnGUICallback();

        public OnUpdateCallback onUpdateCallback = null;

        public OnGUICallback onGUICallback = null;

        public bool ShowGUI
        {
            get => showGUI;
            set => showGUI = value;
        }
        private bool showGUI = true;

#if UNITY_IOS
        bool                 mTouching = false;
#endif

        const int margin = 20;

        Rect windowRect = new Rect(margin + 960 * 0.5f, margin, 960 * 0.5f - (2 * margin),
            540 - (2 * margin));

        public bool OpenInAwake = false;

#if ENABLE_INPUT_SYSTEM
        public InputActionReference ToggleConsoleAction;
#endif


        void Awake()
        {
            ConsoleKit.Modules.ForEach(m => m.OnInit());
            this.showGUI = OpenInAwake;
            DontDestroyOnLoad(this);

            mIndex = ConsoleKit.GetDefaultIndex();
        }

        void OnDestroy()
        {
            ConsoleKit.Modules.ForEach(m => m.OnDestroy());
            ConsoleKit.RemoveAllModules();
        }

        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            // New Input System Logic
            bool toggle = false;

            // 1. Check Action
            if (ToggleConsoleAction != null && ToggleConsoleAction.action != null && ToggleConsoleAction.action.WasPerformedThisFrame())
            {
                toggle = true;
            }
            // 2. Check Default Keys (Fallback)
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            else if (Keyboard.current != null && Keyboard.current.f1Key.wasReleasedThisFrame)
            {
                toggle = true;
            }
#elif UNITY_ANDROID
            else if (Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                toggle = true;
            }
#elif UNITY_IOS
            if (Touchscreen.current != null)
            {
                // Note: Getting exact touch count of 4 pressing fingers
                var touchCount = Touchscreen.current.touches.Count(t => t.press.isPressed);
                if (!mTouching && touchCount == 4) 
                {
                    mTouching = true;
                    toggle = true;
                } 
                else if (touchCount == 0) 
                {
                    mTouching = false;
                }
            }
#endif

            if (toggle)
            {
                this.showGUI = !this.showGUI;
            }
#else
            // Legacy Input System (Commented out by Antigravity but kept for reference/fallback if symbol not defined)
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            if (Input.GetKeyUp(KeyCode.F1))
                this.showGUI = !this.showGUI;
#elif UNITY_ANDROID
            if (Input.GetKeyUp (KeyCode.Escape))
                this.showGUI = !this.showGUI;
#elif UNITY_IOS
            if (!mTouching && Input.touchCount == 4) {
                mTouching = true;
                this.showGUI = !this.showGUI;
            } else if (Input.touchCount == 0) {
                mTouching = false;
            }
#endif
#endif

            if (this.onUpdateCallback != null)
                this.onUpdateCallback();
        }

        private int mIndex = 0;


        void OnGUI()
        {
            if (!showGUI)
                return;

            if (onGUICallback != null)
                onGUICallback();

            var cachedMatrix = GUI.matrix;
            IMGUIHelper.SetDesignResolution(960, 540);
            windowRect = GUILayout.Window(int.MaxValue / 2, windowRect, DrawConsoleWindow, "控制台");
            GUI.matrix = cachedMatrix;
        }


        /// <summary>
        /// A window displaying the logged messages.
        /// </summary>
        void DrawConsoleWindow(int windowID)
        {
            mIndex = GUILayout.Toolbar(mIndex, ConsoleKit.Modules.Select(m => m.Title).ToArray());
            ConsoleKit.Modules[mIndex].DrawGUI();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}