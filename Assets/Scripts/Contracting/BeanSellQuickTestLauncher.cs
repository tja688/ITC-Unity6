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
    public sealed class BeanSellQuickTestLauncher : MonoBehaviour, IController
    {
        [Header("Quick Test")]
        [SerializeField] private bool openOnStart = true;
        [SerializeField] private int clientId = 1;
        [SerializeField] private int day = 2;
        [SerializeField] private float baselineSatisfaction = 3f;
        [SerializeField] private int baselineSignMistake;
        [SerializeField] private int baselineSoldCount;
        [SerializeField] private float maxRoundSeconds = 60f;

        [Header("Hotkey")]
        [SerializeField] private bool allowHotkeyReopen = true;
        [SerializeField] private KeyCode reopenKey = KeyCode.B;

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

        [ContextMenu("一键启动豆罐头推销测试")]
        private void RunQuickTestNow()
        {
            if (bootstrapped && !opening)
            {
                StartCoroutine(OpenOnceRoutine());
            }
        }

        public void ConfigureForQuickTest(
            int newClientId,
            int newDay,
            float newBaselineSatisfaction,
            int newBaselineSignMistake,
            int newBaselineSoldCount,
            float newMaxRoundSeconds,
            bool newAllowHotkeyReopen,
            KeyCode newReopenKey,
            bool newOpenOnStart)
        {
            clientId = Mathf.Max(1, newClientId);
            day = Mathf.Max(1, newDay);
            baselineSatisfaction = Mathf.Max(0f, newBaselineSatisfaction);
            baselineSignMistake = Mathf.Max(0, newBaselineSignMistake);
            baselineSoldCount = Mathf.Max(0, newBaselineSoldCount);
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
            var safeDay = Mathf.Max(1, day);
            var safeSatisfaction = Mathf.Max(0f, baselineSatisfaction);
            var safeMistake = Mathf.Max(0, baselineSignMistake);
            var safeSoldCount = Mathf.Max(0, baselineSoldCount);

            var configModel = this.GetModel<ContractClientConfigModel>();
            var flowState = this.GetModel<ContractFlowStateModel>();
            var runtimeConfig = configModel.BuildBeanSellRuntimeConfig();
            var clientConfig = configModel.GetBeanSellClientConfig(safeClientId);

            this.SendCommand(new BeginBeanSellCommand(
                safeClientId,
                safeDay,
                safeSatisfaction,
                safeMistake,
                safeSoldCount));

            var finished = false;

            if (safeDay < runtimeConfig.EnabledFromDay)
            {
                MainMenuApp.Interface.SendCommand(new SubmitBeanSellResultCommand(new BeanSellResultPayload
                {
                    ClientId = safeClientId,
                    Day = safeDay,
                    PitchType = BeanPitchType.Benefit,
                    FinalScore = 0,
                    SuccessThreshold = runtimeConfig.SuccessThreshold,
                    IsSuccess = false,
                    IsSkipped = true,
                    WasFallback = false
                }));
                finished = true;
            }
            else
            {
                var panelData = new BeanSellPanelData
                {
                    ClientId = safeClientId,
                    Day = safeDay,
                    RuntimeConfig = runtimeConfig,
                    ClientConfig = clientConfig,
                    OnCompleted = payload =>
                    {
                        MainMenuApp.Interface.SendCommand(new SubmitBeanSellResultCommand(payload));
                        UIKit.ClosePanel<BeanSellPanel>();
                        finished = true;
                    }
                };

                UIKit.OpenPanelAsync<BeanSellPanel>(
                        UILevel.PopUI,
                        panelData,
                        assetBundleName: "contracting_ui",
                        prefabName: nameof(BeanSellPanel))
                    .ToAction()
                    .StartGlobal();
            }

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
                LogKit.E("[豆罐头推销快测] 超时未完成，请检查面板是否正常提交与关闭。");
                yield break;
            }

            LogKit.I(
                $"[豆罐头推销快测] Day={safeDay}, 客户={safeClientId}, 结果={flowState.RouteBeanSellResult.Value}, 已售={flowState.RouteBeanSoldCount.Value}, " +
                $"满意度={flowState.Satisfaction.Value:0}");

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
