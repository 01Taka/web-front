using UnityEngine;

public class MechanicalSpiderLegController : MonoBehaviour, IBossAnimationController
{
    public MechanicalSpiderLegSettings settings;
    public Transform[] rightLegIKTargets;
    public Transform[] leftLegIKTargets;

    [SerializeField] private Transform[] _punchTargets;
    [SerializeField] private Transform[] _barrierPositions;
    [SerializeField] private Transform _barriersParent;

    [SerializeField] private PunchBarrierEffectManager _punchBarrierEffectManager;

    private MechanicalSpiderLeg[] legs;

    void Start()
    {
        // nullチェックと要素数チェック
        if (rightLegIKTargets == null || leftLegIKTargets == null)
        {
            Debug.LogError("IKTargets are not assigned. Please assign them in the Inspector.");
            return;
        }

        int totalLegs = rightLegIKTargets.Length + leftLegIKTargets.Length;
        if (totalLegs == 0)
        {
            Debug.LogWarning("No legs found. Please assign IKTargets to the arrays.");
            return;
        }

        legs = new MechanicalSpiderLeg[totalLegs];

        int index = 0;

        foreach (var t in rightLegIKTargets)
        {
            if (t != null)
            {
                legs[index] = new MechanicalSpiderLeg(t, settings, true);
                index++;
            }
            else
            {
                Debug.LogWarning("One of the rightLegIKTargets is null and will be skipped.");
            }
        }

        foreach (var t in leftLegIKTargets)
        {
            if (t != null)
            {
                legs[index] = new MechanicalSpiderLeg(t, settings, false);
                index++;
            }
            else
            {
                Debug.LogWarning("One of the leftLegIKTargets is null and will be skipped.");
            }
        }
    }

    void Update()
    {
        if (legs == null) return;

        foreach (var leg in legs)
        {
            if (leg != null)
            {
                leg.UpdateLeg();
            }
        }
    }

    private int ConvertToPortToIndex(BossAttackPort attackPort)
    {
        if (attackPort == null)
        {
            Debug.LogError("AttackPort is null.");
            return -1;
        }

        BossFiringPort firingPort = attackPort.FiringPortType;
        switch (firingPort)
        {
            case BossFiringPort.RightFrontLeg:
                return 1;
            case BossFiringPort.LeftFrontLeg:
                return 2;
            case BossFiringPort.RightRearLeg:
                return 0;
            case BossFiringPort.LeftRearLeg:
                return 3;
            default:
                Debug.LogWarning($"Unsupported firing port: {firingPort}. Returning an invalid index.");
                return -1;
        }
    }

    // IBossAnimationControllerの実装 (Implementation of IBossAnimationController)
    public void StartAnimation(BossAnimationControllerContext context)
    {
        int legIndex = ConvertToPortToIndex(context.AttackPort);
        if (legIndex < 0 || legIndex >= legs.Length)
        {
            Debug.LogWarning($"Invalid leg index: {legIndex}. Animation will not start.");
            return;
        }

        switch (context.AttackType)
        {
            case BossAttackType.OverloadFist:
                Punch(legIndex);
                break;
            case BossAttackType.NanomiteSwarm:
                Swing(legIndex);
                break;
            default:
                Debug.LogWarning($"Unsupported attack type: {context.AttackType}. No animation will be played.");
                break;
        }
    }

    public void EndAnimation(BossAnimationControllerContext context)
    {
        int legIndex = ConvertToPortToIndex(context.AttackPort);
        if (legIndex < 0 || legIndex >= legs.Length)
        {
            Debug.LogWarning($"Invalid leg index: {legIndex}. Animation will not end.");
            return;
        }

        switch (context.AttackType)
        {
            case BossAttackType.OverloadFist:
                EndPunch(legIndex);
                break;
            default:
                Debug.LogWarning($"Unsupported attack type: {context.AttackType}. No animation will be ended.");
                break;
        }
    }

    public void Punch(int legIndex)
    {
        // 範囲チェックと nullチェック
        if (legs == null || legIndex < 0 || legIndex >= legs.Length || legs[legIndex] == null)
        {
            Debug.LogError($"Punch: Invalid leg index or leg is null at index {legIndex}.");
            return;
        }
        if (_barriersParent == null || _barrierPositions == null || legIndex >= _barrierPositions.Length || _barrierPositions[legIndex] == null)
        {
            Debug.LogError($"Punch: Barrier objects are not properly assigned for index {legIndex}.");
            return;
        }
        if (_punchBarrierEffectManager == null)
        {
            Debug.LogError("Punch: PunchBarrierEffectManager is not assigned.");
            return;
        }
        if (_punchTargets == null || legIndex >= _punchTargets.Length || _punchTargets[legIndex] == null)
        {
            Debug.LogError($"Punch: Punch target is not properly assigned for index {legIndex}.");
            return;
        }

        _barriersParent.position = _barrierPositions[legIndex].position;
        _punchBarrierEffectManager.StartPunchSequence(legs[legIndex], _punchTargets[legIndex].position);
    }

    public void EndPunch(int legIndex)
    {
        // 範囲チェックと nullチェック
        if (legs == null || legIndex < 0 || legIndex >= legs.Length || legs[legIndex] == null)
        {
            Debug.LogError($"EndPunch: Invalid leg index or leg is null at index {legIndex}.");
            return;
        }
        if (_punchBarrierEffectManager == null)
        {
            Debug.LogError("EndPunch: PunchBarrierEffectManager is not assigned.");
            return;
        }

        Debug.Log($"End Punch {legIndex}");
        _punchBarrierEffectManager.StopAndReturnImmediately(legs[legIndex]);
    }

    public void Swing(int legIndex)
    {
        // 範囲チェックと nullチェック
        if (legs == null || legIndex < 0 || legIndex >= legs.Length || legs[legIndex] == null)
        {
            Debug.LogError($"Swing: Invalid leg index or leg is null at index {legIndex}.");
            return;
        }
        legs[legIndex].StartSwing();
    }

    public void ResetLeg(int legIndex)
    {
        // 範囲チェックと nullチェック
        if (legs == null || legIndex < 0 || legIndex >= legs.Length || legs[legIndex] == null)
        {
            Debug.LogError($"ResetLeg: Invalid leg index or leg is null at index {legIndex}.");
            return;
        }
        legs[legIndex].ResetToIdle();
    }
}