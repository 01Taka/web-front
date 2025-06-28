using Fusion;
using UnityEngine;

public enum AttackType
{
    SilkSnare,          // �����̂�
    BowstringPiercer,   // �`���[�W�ʁ{����
    WebVolley,          // ���˕����{���ː�
    OrbWeaver           // �`���[�W�ʁ{����
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