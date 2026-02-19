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
    public sealed class DocumentReviewQuickTestLauncher : MonoBehaviour, IController
    {
        [Header("Quick Test")]
        [SerializeField] private bool openOnStart = true;
        [SerializeField] private int clientId = 1;
        [SerializeField] private bool tutorialMode;
        [SerializeField] private float baselineSatisfaction = 3f;
        [SerializeField] private int baselineSignMistake = 0;
        [SerializeField] private float maxRoundSeconds = 180f;

        [Header("Hotkey")]
        [SerializeField] private bool allowHotkeyReopen = true;
        [SerializeField] private KeyCode reopenKey = KeyCode.R;

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

        [ContextMenu("一键启动文书审核测试")]
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
            float newBaselineSatisfaction,
            int newBaselineSignMistake,
            float newMaxRoundSeconds,
            bool newAllowHotkeyReopen,
            KeyCode newReopenKey,
            bool newOpenOnStart)
        {
            clientId = Mathf.Max(1, newClientId);
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

            this.SendCommand(new BeginDocumentReviewCommand(safeClientId, safeSatisfaction, safeMistake));
            var configModel = this.GetModel<ContractClientConfigModel>();
            var flowState = this.GetModel<ContractFlowStateModel>();
            var clientConfig = configModel.GetDocumentReviewClientConfig(safeClientId);

            var finished = false;
            var panelData = new DocumentReviewPanelData
            {
                ClientId = safeClientId,
                TutorialMode = tutorialMode,
                ExpectedAction = clientConfig.CorrectDecision,
                RuntimeConfig = configModel.DocumentReviewRuleConfig,
                ClientConfig = clientConfig,
                AllowedRejectReasons = clientConfig.AllowedRejectReasons,
                OnCompleted = payload =>
                {
                    MainMenuApp.Interface.SendCommand(new SubmitDocumentReviewResultCommand(payload));
                    UIKit.ClosePanel<DocumentReviewPanel>();
                    finished = true;
                }
            };

            UIKit.OpenPanelAsync<DocumentReviewPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(DocumentReviewPanel))
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
                LogKit.E("[文书审核快测] 超时未完成，请检查面板是否正常提交与关闭。");
                yield break;
            }

            LogKit.I(
                $"[文书审核快测] 客户={safeClientId}, 结果={flowState.RouteDocReviewResult.Value}, " +
                $"满意度={flowState.Satisfaction.Value:0}, 失误标记={flowState.SignMistake.Value}, 检查数={flowState.LastInspectCount.Value}");

            opening = false;
        }

        private static void EnsureEventSystemExists()
        {
            var eventSystem = Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
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
