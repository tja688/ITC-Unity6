using System.Collections;
using UnityEngine;

/// <summary>
/// TestScene smoke runner: automatically starts one Hedium contract stage on Play.
/// </summary>
public sealed class HediumContractAutoSmoke : MonoBehaviour
{
    private enum SmokeStage
    {
        DocumentVerifier = 0,
        RuneInput = 1,
        Stamp = 2,
        SoulHarvest = 3,
        SpecialEvent = 4
    }

    [SerializeField] private bool runOnStart = true;
    [SerializeField] private float startDelaySeconds = 1.0f;
    [SerializeField] private SmokeStage stage = SmokeStage.DocumentVerifier;

    private IEnumerator Start()
    {
        if (!runOnStart)
        {
            yield break;
        }

        yield return new WaitForSeconds(startDelaySeconds);

        var flow = FindFirstObjectByType<SigningFlowManager>();
        if (flow == null)
        {
            Debug.LogError("[HediumContractAutoSmoke] SigningFlowManager not found.");
            yield break;
        }

        Debug.Log($"[HediumContractAutoSmoke] Triggering stage: {stage}");

        switch (stage)
        {
            case SmokeStage.DocumentVerifier:
                flow.DocumentVerifierStageStart();
                break;
            case SmokeStage.RuneInput:
                flow.RuneInputStageStart();
                break;
            case SmokeStage.Stamp:
                flow.StampStageStart();
                break;
            case SmokeStage.SoulHarvest:
                flow.SoulHarvestStageStart();
                break;
            case SmokeStage.SpecialEvent:
                flow.SpecialEventSystem();
                break;
        }
    }
}
