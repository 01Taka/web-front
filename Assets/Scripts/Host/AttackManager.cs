using UnityEngine;


public class AttackManager : MonoBehaviour
{
    public void HandleAttack(AttackData data)
    {
        SimulateAttack(data);
    }

    void SimulateAttack(AttackData data)
    {
        Debug.Log($"[SimAttack] Type:{data.Type} Dir:{data.Direction} ChargeAmount:{data.ChargeAmount} ShotCount:{data.ShotCount}");

        switch (data.Type)
        {
            case AttackType.SilkSnare:
                SimulateSilkSnare(data);
                break;
            case AttackType.BowstringPiercer:
                SimulateBowstringPiercer(data);
                break;
            case AttackType.WebVolley:
                SimulateWebVolley(data);
                break;
            case AttackType.OrbWeaver:
                SimulateOrbWeaver(data);
                break;
            default:
                Debug.LogWarning($"Unknown attack type: {data.Type}");
                break;
        }
    }

    void SimulateSilkSnare(AttackData data)
    {
        // TODO: �������ʂȂǏ���
    }

    void SimulateBowstringPiercer(AttackData data)
    {
        // TODO: �ђʍU���̈З͌v�Z�A����Ȃ�
    }

    void SimulateWebVolley(AttackData data)
    {
        // TODO: �e�̔��˂�͈͔���
    }

    void SimulateOrbWeaver(AttackData data)
    {
        // TODO: �͈�DoT�ƌ������ʂ̏���
    }
}
