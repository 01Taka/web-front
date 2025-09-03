using UnityEngine;
using UnityEngine.InputSystem;

public class MechanicalSpiderLegController : MonoBehaviour, IBossAnimationController
{
    public MechanicalSpiderLegSettings settings;
    public Transform[] rightLegIKTargets;
    public Transform[] leftLegIKTargets;

    public Transform[] punchTargets;

    private MechanicalSpiderLeg[] legs;

    void Start()
    {
        int totalLegs = rightLegIKTargets.Length + leftLegIKTargets.Length;
        legs = new MechanicalSpiderLeg[totalLegs];

        int index = 0;

        foreach (var t in rightLegIKTargets)
        {
            legs[index] = new MechanicalSpiderLeg(t, settings, true);
            index++;
        }

        foreach (var t in leftLegIKTargets)
        {
            legs[index] = new MechanicalSpiderLeg(t, settings, false);
            index++;
        }
    }

    void Update()
    {
        foreach (var leg in legs)
        {
            leg.UpdateLeg();
        }
    }

    private int ConvartToPortToIndex(BossAttackPort attackPort)
    {
        BossFiringPort firingPort = attackPort.FiringPortType;
        switch (firingPort)
        {
            case BossFiringPort.RightFrontLeg:
                return 1; // Assuming the first right leg is at index 0.
            case BossFiringPort.LeftFrontLeg:
                return 2; // Assuming the first left leg is at index 2.
            case BossFiringPort.RightRearLeg:
                return 0; // Assuming the second right leg is at index 1.
            case BossFiringPort.LeftRearLeg:
                return 3; // Assuming the second left leg is at index 3.
            default:
                Debug.LogWarning("Unsupported firing port: " + firingPort);
                return -1; // Return an invalid index.
        }
    }

    // IBossAnimationController‚ÌŽÀ‘• (Implementation of IBossAnimationController)
    public void StartAnimation(BossAnimationControllerContext context)
    {
        int legIndex = ConvartToPortToIndex(context.AttackPort);
        if (legIndex < 0 || legIndex >= legs.Length) return;

        switch (context.AttackType)
        {
            case BossAttackType.OverloadFist:
                Punch(legIndex);
                break;
            case BossAttackType.NanomiteSwarm:
                Swing(legIndex);
                break;
            default:
                Debug.LogWarning("Unsupported attack type: " + context.AttackType);
                break;
        }
    }

    public void EndAnimation(BossAnimationControllerContext context)
    {
        int legIndex = ConvartToPortToIndex(context.AttackPort);
        if (legIndex < 0 || legIndex >= legs.Length) return;

        switch (context.AttackType)
        {
            case BossAttackType.OverloadFist:
                EndPunch(legIndex);
                break;
            default:
                Debug.LogWarning("Unsupported attack type: " + context.AttackType);
                break;
        }
    }

    public void Punch(int legIndex)
    {
        if (legIndex < 0 || legIndex >= legs.Length) return;
        legs[legIndex].StartPunch(punchTargets[legIndex].position);
    }

    public void EndPunch(int legIndex)
    {
        if (legIndex < 0 || legIndex >= legs.Length) return;
        legs[legIndex].EndPunchAndReturn();
    }

    public void Swing(int legIndex)
    {
        if (legIndex < 0 || legIndex >= legs.Length) return;
        legs[legIndex].StartSwing();
    }

    public void ResetLeg(int legIndex)
    {
        if (legIndex < 0 || legIndex >= legs.Length) return;
        legs[legIndex].ResetToIdle();
    }
}