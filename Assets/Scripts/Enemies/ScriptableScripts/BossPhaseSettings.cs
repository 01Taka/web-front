using UnityEngine;

[CreateAssetMenu(fileName = "NewBossPhaseSettings", menuName = "Boss/BossPhaseSettings")]
public class BossPhaseSettings : ScriptableObject
{
    public int maxHealth;
    public float phaseTransitionTime = 1.0f; // フェーズ移行にかかる時間
    public float minAttackInterval = 2f;     // このフェーズでの最小攻撃間隔
    public float maxAttackInterval = 5f;     // このフェーズでの最大攻撃間隔
    public BossFiringPort[] firingPorts;
    public AttackPattern[] attackPatterns;
    public int FiringPortCount => firingPorts.Length;
}