using UnityEngine;

[CreateAssetMenu(fileName = "NewBossPhaseSettings", menuName = "Boss/BossPhaseSettings")]
public class BossPhaseSettings : ScriptableObject
{
    [Tooltip("このフェーズから次のフェーズへ移行する体力割合。0.0から1.0の値で設定してください。")]
    [Range(0f, 1f)] public float phaseTransitionHealthPercentage;

    public float phaseTransitionTime = 1.0f; // フェーズ移行にかかる時間
    public float minAttackInterval = 2f;      // このフェーズでの最小攻撃間隔
    public float maxAttackInterval = 5f;      // このフェーズでの最大攻撃間隔
    public BossFiringPort[] firingPorts;
    public AttackPattern[] attackPatterns;
    public int FiringPortCount => firingPorts.Length;
}