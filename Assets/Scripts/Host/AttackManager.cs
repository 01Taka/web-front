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
        // TODO: Œ¸‘¬Œø‰Ê‚È‚Çˆ—
    }

    void SimulateBowstringPiercer(AttackData data)
    {
        // TODO: ŠÑ’ÊUŒ‚‚ÌˆĞ—ÍŒvZA”»’è‚È‚Ç
    }

    void SimulateWebVolley(AttackData data)
    {
        // TODO: ’e‚Ì”­Ë‚â”ÍˆÍ”»’è
    }

    void SimulateOrbWeaver(AttackData data)
    {
        // TODO: ”ÍˆÍDoT‚ÆŒ¸‘¬Œø‰Ê‚Ìˆ—
    }
}
