using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackPattern", menuName = "Boss/AttackPattern")]
public class AttackPattern : ScriptableObject
{
    // どの攻撃を実行するか
    public BossAttackType AttackType;

    // 攻撃名（デバッグ用やUIで使用）
    public string AttackName;

    // 基本ダメージ（ボス設定で基準となる攻撃力）
    public int BaseDamage;

    // 攻撃にかかる時間
    public float AttackDuration;

    // 攻撃が発生するまでの猶予時間（攻撃開始までの待機時間）
    public float AttackPreparationTime;

    // 攻撃キャンセルに必要なダメージ（このダメージ量を受けると攻撃をキャンセルする）
    public float CancelDamageThreshold;

    public float CancelSelfInflictedDamage;

    public FiringPortType FiringPortType;

    //public int SimultaneousPorts = 1;

    // 攻撃が実行されるまでの猶予時間が経過したかどうかを示すプロパティ（デバッグ用）
    public bool IsAttackReady(float elapsedTime)
    {
        return elapsedTime >= AttackPreparationTime;
    }
}

    //public PortOccupiedBehavior occupiedBehavior;

    //public PortDamageManagementType damageManagementType;