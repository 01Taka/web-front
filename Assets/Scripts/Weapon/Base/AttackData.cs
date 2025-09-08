using Fusion;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    SilkSnare,
    ChargedPierce,
    VolleyBurst,
    WebMine
}

public interface IAttackSender
{
    void SendAttack(AttackInputData data);
}

public struct TouchInputData
{
    public bool StartWithInCenter;
    public bool EndWithInCenter;
    public float HoldDuration;
    public float SwipeDistance;
    public Vector2 SwipeDirection;
    public bool IsSwipeForward;
    public Vector2 FromCenterDirection;
    public Vector2 FromBottomCenterDirection;
    public float CircularGestureAmount;
}

public interface IAttackRecognizer
{
    List<AttackInputData> Recognize(TouchInputData touchData, Vector2 screenCenter, InputAttackConfig inputConfig);
}

public struct AttackInputData
{
    public AttackType Type;
    public Vector3 Direction;
    public float ChargeAmount;

    public AttackInputData(AttackType type, Vector3 dir, float charge)
    {
        Type = type;
        Direction = dir.normalized;
        ChargeAmount = charge;
    }
}


public struct AttackDataNetwork : INetworkStruct
{
    public int Level;
    public AttackType Type;
    public Vector3 Direction;
    public float ChargeAmount;
}

public struct AttackData
{
    public PlayerRef AttackerRef;
    public int AttackerIndex; 
    public AttackType Type;
    public Vector3 Direction;
    public float ChargeAmount;
}

public struct ProjectileSpawnParams
{
    public PlayerRef AttackerRef;
    public int AttackerIndex;
    public Color ProjectileColor;
    public AttackType Type;
    public Vector3 Position;
    public Vector3 Direction;
    public float Damage;
    public float Speed;
    public float Range;
    public float DetectionRadius;
    public float EffectDuration;
    public float EffectInterval;
    public float EffectRadius;
    public float ProjectileScaleRaito;
}

public interface IDamageable
{
    void TakeDamage(float amount);
}

public interface IMovable
{
    void ApplySlow(float multiplier, float duration);
}
