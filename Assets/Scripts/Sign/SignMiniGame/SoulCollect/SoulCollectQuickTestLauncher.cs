using System.Collections;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace ITC.SignMiniGame
{
    [DisallowMultipleComponent]
    public sealed class SoulCollectQuickTestLauncher : MonoBehaviour, IController
    {
        [Header("Quick Test")]
        [SerializeField] private bool openOnStart = true;
        [SerializeField] private int clientId = 1;
        [SerializeField] private int targetPercent = 45;
        [SerializeField] private bool tutorialMode;
        [SerializeField] private float baselineSatisfaction = 3f;
        [SerializeField] private int baselineSignMistake;
        [SerializeField] private float maxRoundSeconds = 120f;

        [Header("Hotkey")]
        [SerializeField] private bool allowHotkeyReopen = true;
        [SerializeField] private KeyCode reopenKey = KeyCode.Y;

        private bool bootstrapped;
        private bool opening;

        private void Awake()
        {
            UIKit.Config = new MainMenuUIKitConfig();
            EnsureEventSystemExists();
        }

        private IEnumerator Start()
        {
            if (bootstrapped)
            {
                yield break;
            }

            bootstrapped = true;
            yield return ResKit.InitAsync();

            if (openOnStart)
            {
                StartCoroutine(OpenOnceRoutine());
            }
        }

        private void Update()
        {
            if (!allowHotkeyReopen || !bootstrapped || opening)
            {
                return;
            }

            if (Input.GetKeyDown(reopenKey))
            {
                StartCoroutine(OpenOnceRoutine());
            }
        }

        [ContextMenu("一键启动灵魂收取测试")]
        private void RunQuickTestNow()
        {
            if (bootstrapped && !opening)
            {
                StartCoroutine(OpenOnceRoutine());
            }
        }

        public void ConfigureForQuickTest(
            int newClientId,
            int newTargetPercent,
            bool newTutorialMode,
            float newBaselineSatisfaction,
            int newBaselineSignMistake,
            float newMaxRoundSeconds,
            bool newAllowHotkeyReopen,
            KeyCode newReopenKey,
            bool newOpenOnStart)
        {
            clientId = Mathf.Max(1, newClientId);
            targetPercent = Mathf.Clamp(newTargetPercent, 0, 100);
            tutorialMode = newTutorialMode;
            baselineSatisfaction = Mathf.Max(0f, newBaselineSatisfaction);
            baselineSignMistake = Mathf.Max(0, newBaselineSignMistake);
            maxRoundSeconds = Mathf.Max(10f, newMaxRoundSeconds);
            allowHotkeyReopen = newAllowHotkeyReopen;
            reopenKey = newReopenKey;
            openOnStart = newOpenOnStart;
        }

        private IEnumerator OpenOnceRoutine()
        {
            if (opening)
            {
                yield break;
            }

            opening = true;
            var safeClientId = Mathf.Max(1, clientId);
            var safeSatisfaction = Mathf.Max(0f, baselineSatisfaction);
            var safeMistake = Mathf.Max(0, baselineSignMistake);

            var configModel = this.GetModel<SignMiniGameClientConfigModel>();
            var flowState = this.GetModel<SignMiniGameFlowStateModel>();
            var runtimeConfig = configModel.BuildSoulCollectRuntimeConfig(safeClientId, targetPercent);
            var clientConfig = configModel.GetSoulCollectClientConfig(safeClientId);

            this.SendCommand(new BeginSoulCollectCommand(
                safeClientId,
                safeSatisfaction,
                safeMistake,
                runtimeConfig.TargetPercent,
                runtimeConfig.MinPercent,
                runtimeConfig.MaxPercent));

            var finished = false;
            var panelData = new SoulCollectPanelData
            {
                ClientId = safeClientId,
                TutorialMode = tutorialMode,
                RuntimeConfig = runtimeConfig,
                ClientConfig = clientConfig,
                OnCompleted = payload =>
                {
                    MainMenuApp.Interface.SendCommand(new SubmitSoulCollectResultCommand(payload));
                    UIKit.ClosePanel<SoulCollectPanel>();
                    finished = true;
                }
            };

            UIKit.OpenPanelAsync<SoulCollectPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "sign_minigame_ui",
                    prefabName: nameof(SoulCollectPanel))
                .ToAction()
                .StartGlobal();

            var elapsed = 0f;
            var timeout = Mathf.Max(10f, maxRoundSeconds);
            while (!finished && elapsed < timeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!finished)
            {
                opening = false;
                LogKit.E("[灵魂收取快测] 超时未完成，请检查面板是否正常提交与关闭。");
                yield break;
            }

            LogKit.I(
                $"[灵魂收取快测] 客户={safeClientId}, 实际={flowState.RouteSoulCollectPercent.Value}%, 区间={flowState.RouteSoulMin.Value}-{flowState.RouteSoulMax.Value}, " +
                $"满意度={flowState.Satisfaction.Value:0}, 失误标记={flowState.SignMistake.Value}, Raw={flowState.SoulCollectActualRawPercent.Value:0.0}");

            opening = false;
        }

        private static void EnsureEventSystemExists()
        {
            var allEventSystems =
                Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            EventSystem eventSystem = null;
            foreach (var es in allEventSystems)
            {
                if (eventSystem == null)
                {
                    eventSystem = es;
                    if (!eventSystem.gameObject.activeInHierarchy)
                    {
                        eventSystem.gameObject.SetActive(true);
                    }
                }
                else
                {
                    es.gameObject.SetActive(false);
                }
            }

            if (eventSystem == null)
            {
                var eventSystemObj = new GameObject("EventSystem", typeof(EventSystem));
                Object.DontDestroyOnLoad(eventSystemObj);
                eventSystem = eventSystemObj.GetComponent<EventSystem>();
            }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            var inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputSystemModule == null)
            {
                inputSystemModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            var legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacyModule != null)
            {
                legacyModule.enabled = false;
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            var legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacyModule == null)
            {
                legacyModule = eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }

#if ENABLE_INPUT_SYSTEM
            var inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputSystemModule != null)
            {
                inputSystemModule.enabled = false;
            }
#endif
#else
            if (eventSystem.GetComponent<StandaloneInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }
#endif
        }

        public IArchitecture GetArchitecture()
        {
            return MainMenuApp.Interface;
        }
    }
}
