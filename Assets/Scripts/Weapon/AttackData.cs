using Fusion;
using UnityEngine;

public enum AttackType
{
    SilkSnare,          // 方向のみ
    BowstringPiercer,   // チャージ量＋方向
    WebVolley,          // 発射方向＋発射数
    OrbWeaver           // チャージ量＋方向
}

public interface IAttackSender
{
    void SendAttack(AttackInputData data);
}

public struct AttackInputData
{
    public AttackType Type;
    public Vector3 Direction;
    public float ChargeAmount;
    public int ShotCount;
}

public struct AttackDataNetwork : INetworkStruct
{
    public int Level;
    public int AttackPoint;
    public AttackType Type;
    public Vector3 Direction;
    public float ChargeAmount;
    public int ShotCount;
}

public struct AttackData
{
    public int Level;
    public int AttackPoint;
    public AttackType Type;
    public Vector3 Direction;
    public float ChargeAmount;
    public int ShotCount;
} 