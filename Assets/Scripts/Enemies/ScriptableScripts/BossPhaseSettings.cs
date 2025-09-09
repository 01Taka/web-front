using UnityEngine;

[CreateAssetMenu(fileName = "NewBossPhaseSettings", menuName = "Boss/BossPhaseSettings")]
public class BossPhaseSettings : ScriptableObject
{
    [Tooltip("���̃t�F�[�Y���玟�̃t�F�[�Y�ֈڍs����̗͊����B0.0����1.0�̒l�Őݒ肵�Ă��������B")]
    [Range(0f, 1f)] public float phaseTransitionHealthPercentage;

    public float phaseTransitionTime = 1.0f; // �t�F�[�Y�ڍs�ɂ����鎞��
    public float minAttackInterval = 2f;      // ���̃t�F�[�Y�ł̍ŏ��U���Ԋu
    public float maxAttackInterval = 5f;      // ���̃t�F�[�Y�ł̍ő�U���Ԋu
    public BossFiringPort[] firingPorts;
    public AttackPattern[] attackPatterns;
    public int FiringPortCount => firingPorts.Length;
}