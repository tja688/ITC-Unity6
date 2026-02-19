using System.Collections;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace ITC.Contracting
{
    [DisallowMultipleComponent]
    public sealed class StampQuickTestLauncher : MonoBehaviour, IController
    {
        [Header("Quick Test")]
        [SerializeField] private bool openOnStart = true;
        [SerializeField] private int clientId = 1;
        [SerializeField] private bool tutorialMode;
        [SerializeField] private bool hasVerifyDebuff;
        [SerializeField] private float baselineSatisfaction = 3f;
        [SerializeField] private int baselineSignMistake;
        [SerializeField] private float maxRoundSeconds = 120f;

        [Header("Hotkey")]
        [SerializeField] private bool allowHotkeyReopen = true;
        [SerializeField] private KeyCode reopenKey = KeyCode.T;

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

        [ContextMenu("一键启动印章盖印测试")]
        private void RunQuickTestNow()
        {
            if (bootstrapped && !opening)
            {
                StartCoroutine(OpenOnceRoutine());
            }
        }

        public void ConfigureForQuickTest(
            int newClientId,
            bool newTutorialMode,
            bool newHasVerifyDebuff,
            float newBaselineSatisfaction,
            int newBaselineSignMistake,
            float newMaxRoundSeconds,
            bool newAllowHotkeyReopen,
            KeyCode newReopenKey,
            bool newOpenOnStart)
        {
            clientId = Mathf.Max(1, newClientId);
            tutorialMode = newTutorialMode;
            hasVerifyDebuff = newHasVerifyDebuff;
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

            this.SendCommand(new BeginStampSelectionCommand(safeClientId, safeSatisfaction, safeMistake));
            var configModel = this.GetModel<ContractClientConfigModel>();
            var flowState = this.GetModel<ContractFlowStateModel>();

            var finished = false;
            var panelData = new StampPanelData
            {
                ClientId = safeClientId,
                TutorialMode = tutorialMode,
                HasVerifyDebuff = hasVerifyDebuff,
                RuntimeConfig = configModel.GetStampRuntimeConfig(safeClientId),
                ClientConfig = configModel.GetStampClientConfig(safeClientId),
                OnCompleted = payload =>
                {
                    MainMenuApp.Interface.SendCommand(new SubmitStampSelectionResultCommand(payload));
                    UIKit.ClosePanel<StampPanel>();
                    finished = true;
                }
            };

            UIKit.OpenPanelAsync<StampPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(StampPanel))
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
                LogKit.E("[印章盖印快测] 超时未完成，请检查面板是否正常提交与关闭。");
                yield break;
            }

            LogKit.I(
                $"[印章盖印快测] 客户={safeClientId}, 印章={flowState.RouteStampType.Value}, 时机={flowState.RouteStampTimingResult.Value}, " +
                $"满意度={flowState.Satisfaction.Value:0}, 失误标记={flowState.SignMistake.Value}, 命中值={flowState.LastStampHitNormalized.Value:0.00}");

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

