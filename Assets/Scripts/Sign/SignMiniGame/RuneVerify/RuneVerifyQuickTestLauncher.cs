using System;
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
    public sealed class RuneVerifyQuickTestLauncher : MonoBehaviour, IController
    {
        [Header("Quick Test")]
        [SerializeField] private bool openOnStart = true;
        [SerializeField] private int clientId = 1;
        [SerializeField] private float maxRoundSeconds = 30f;
        [SerializeField] private int fixedSeed = 91357;
        [SerializeField] private bool forceAlwaysTrigger = true;

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

        public void ConfigureForQuickTest(
            int newClientId,
            float newMaxRoundSeconds,
            int newFixedSeed,
            bool newForceAlwaysTrigger,
            bool newAllowHotkeyReopen,
            KeyCode newReopenKey,
            bool newOpenOnStart)
        {
            clientId = Mathf.Max(1, newClientId);
            maxRoundSeconds = Mathf.Max(10f, newMaxRoundSeconds);
            fixedSeed = newFixedSeed;
            forceAlwaysTrigger = newForceAlwaysTrigger;
            allowHotkeyReopen = newAllowHotkeyReopen;
            reopenKey = newReopenKey;
            openOnStart = newOpenOnStart;
        }

        [ContextMenu("一键启动符文核验测试")]
        private void RunQuickTestNow()
        {
            if (bootstrapped && !opening)
            {
                StartCoroutine(OpenOnceRoutine());
            }
        }

        private IEnumerator OpenOnceRoutine()
        {
            if (opening)
            {
                yield break;
            }

            opening = true;
            var safeClientId = Mathf.Max(1, clientId);
            var configModel = this.GetModel<SignMiniGameClientConfigModel>();
            var flowState = this.GetModel<SignMiniGameFlowStateModel>();
            var runtimeConfig = configModel.RuneVerifyRuleConfig.Clone();
            if (forceAlwaysTrigger)
            {
                runtimeConfig.TriggerProbability = 1f;
            }

            var seed = fixedSeed >= 0 ? fixedSeed : Environment.TickCount;
            this.SendCommand(new BeginRuneVerifyCommand(safeClientId, true, seed));

            var finished = false;
            var panelData = new RuneVerifyPanelData
            {
                ClientId = safeClientId,
                RuntimeConfig = runtimeConfig,
                RuntimeSeed = seed,
                OnCompleted = payload =>
                {
                    MainMenuApp.Interface.SendCommand(new SubmitRuneVerifyResultCommand(payload));
                    UIKit.ClosePanel<RuneVerifyPanel>();
                    finished = true;
                }
            };

            UIKit.OpenPanelAsync<RuneVerifyPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "sign_minigame_ui",
                    prefabName: nameof(RuneVerifyPanel))
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
                LogKit.E("[符文核验快测] 超时未完成，请检查面板是否正常提交与关闭。");
                yield break;
            }

            LogKit.I(
                $"[符文核验快测] 客户={safeClientId}, 结果={flowState.RouteRuneVerifyResult.Value}, " +
                $"Debuff={flowState.RouteRuneVerifyDebuff.Value}, 找齐={flowState.RuneVerifyFoundCount.Value}/{flowState.RuneVerifyDistortedCount.Value}, Seed={flowState.RuneVerifySeed.Value}");

            opening = false;
        }

        private static void EnsureEventSystemExists()
        {
            var allEventSystems = UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);

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
                UnityEngine.Object.DontDestroyOnLoad(eventSystemObj);
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
