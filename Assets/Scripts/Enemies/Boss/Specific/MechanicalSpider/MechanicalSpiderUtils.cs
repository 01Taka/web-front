using UnityEngine;

public static class MechanicalSpiderUtils
{
    public static int ConvertToPortToIndex(BossAttackPort attackPort)
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
}
