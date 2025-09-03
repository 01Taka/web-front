using UnityEngine;

[CreateAssetMenu(fileName = "NewBossPhaseSettings", menuName = "Boss/BossPhaseSettings")]
public class BossPhaseSettings : ScriptableObject
{
    public int maxHealth;
    public float phaseTransitionTime = 1.0f; // �t�F�[�Y�ڍs�ɂ����鎞��
    public float minAttackInterval = 2f;     // ���̃t�F�[�Y�ł̍ŏ��U���Ԋu
    public float maxAttackInterval = 5f;     // ���̃t�F�[�Y�ł̍ő�U���Ԋu
    public BossFiringPort[] firingPorts;
    public AttackPattern[] attackPatterns;
    public int FiringPortCount => firingPorts.Length;
}