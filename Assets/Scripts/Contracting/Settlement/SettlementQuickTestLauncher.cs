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
    public sealed class SettlementQuickTestLauncher : MonoBehaviour, IController
    {
        [Header("Quick Test")]
        [SerializeField] private bool openOnStart = true;
        [SerializeField] private int clientId = 1;
        [SerializeField] private int day = 2;
        [SerializeField] private int satisfaction = 3;
        [SerializeField] private int signMistake;
        [SerializeField] private int money = 0;
        [SerializeField] private int numberOfSignMistake = 0;
        [SerializeField] private int globalSignMistake = 0;
        [SerializeField] private float maxRoundSeconds = 30f;

        [Header("Hotkey")]
        [SerializeField] private bool allowHotkeyReopen = true;
        [SerializeField] private KeyCode reopenKey = KeyCode.G;

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

        [ContextMenu("一键启动结算离场测试")]
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
            int newSatisfaction,
            int newSignMistake,
            int newMoney,
            int newNumberOfSignMistake,
            int newGlobalSignMistake,
            float newMaxRoundSeconds,
            bool newAllowHotkeyReopen,
            KeyCode newReopenKey,
            bool newOpenOnStart)
        {
            clientId = Mathf.Max(1, newClientId);
            day = Mathf.Max(1, newDay);
            satisfaction = Mathf.Clamp(newSatisfaction, 0, 5);
            signMistake = Mathf.Clamp(newSignMistake, 0, 1);
            money = Mathf.Max(0, newMoney);
            numberOfSignMistake = Mathf.Max(0, newNumberOfSignMistake);
            globalSignMistake = Mathf.Max(0, newGlobalSignMistake);
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

            this.SendCommand(new BeginSettlementCommand(
                safeClientId,
                safeDay,
                satisfaction,
                signMistake,
                money,
                numberOfSignMistake,
                globalSignMistake));
            this.SendCommand(new FinalizeClientContractCommand(safeDay));

            var configModel = this.GetModel<ContractClientConfigModel>();
            var flowState = this.GetModel<ContractFlowStateModel>();
            var clientConfig = configModel.GetSettlementClientConfig(safeClientId);
            var ruleConfig = configModel.BuildSettlementRuntimeConfig();

            var finished = false;
            var panelData = new SettlementPanelData
            {
                ClientId = safeClientId,
                Day = safeDay,
                ClientDisplayName = clientConfig != null ? clientConfig.ClientDisplayName : $"客户{safeClientId}",
                DocReviewResult = flowState.RouteDocReviewResult.Value,
                QteErrorCount = flowState.RouteQteErrorCount.Value,
                StampType = flowState.RouteStampType.Value,
                StampTimingResult = flowState.RouteStampTimingResult.Value,
                SoulPercent = flowState.RouteSoulCollectPercent.Value,
                BeanSellResult = flowState.RouteBeanSellResult.Value,
                FinalSatisfaction = Mathf.RoundToInt(flowState.Satisfaction.Value),
                TipAmount = flowState.RouteSettlementTip.Value,
                TotalMoney = flowState.Money.Value,
                Tier = ParseTier(flowState.RouteSettlementTier.Value),
                FeedbackDuration = ruleConfig.FeedbackDuration,
                OnCompleted = () =>
                {
                    UIKit.ClosePanel<SettlementPanel>();
                    finished = true;
                }
            };

            UIKit.OpenPanelAsync<SettlementPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(SettlementPanel))
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
                LogKit.E("[结算离场快测] 超时未完成，请检查面板是否正常提交与关闭。");
                yield break;
            }

            LogKit.I(
                $"[结算离场快测] 客户={safeClientId}, Day={safeDay}, Tier={flowState.RouteSettlementTier.Value}, Tip={flowState.RouteSettlementTip.Value}, " +
                $"满意度={flowState.Satisfaction.Value:0}, 余额={flowState.Money.Value}, 失误累计={flowState.NumberOfSignMistake.Value}/{flowState.GlobalSignMistake.Value}");

            opening = false;
        }

        private static SettlementTier ParseTier(string tier)
        {
            if (string.Equals(tier, "good", System.StringComparison.OrdinalIgnoreCase))
            {
                return SettlementTier.Good;
            }

            if (string.Equals(tier, "bad", System.StringComparison.OrdinalIgnoreCase))
            {
                return SettlementTier.Bad;
            }

            return SettlementTier.Neutral;
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
