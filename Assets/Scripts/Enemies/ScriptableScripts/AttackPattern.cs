using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackPattern", menuName = "Boss/AttackPattern")]
public class AttackPattern : ScriptableObject
{
    // �ǂ̍U�������s���邩
    public BossAttackType AttackType;

    // �U�����i�f�o�b�O�p��UI�Ŏg�p�j
    public string AttackName;

    // ��{�_���[�W�i�{�X�ݒ�Ŋ�ƂȂ�U���́j
    public int BaseDamage;

    // �U���ɂ����鎞��
    public float AttackDuration;

    // �U������������܂ł̗P�\���ԁi�U���J�n�܂ł̑ҋ@���ԁj
    public float AttackPreparationTime;

    // �U���L�����Z���ɕK�v�ȃ_���[�W�i���̃_���[�W�ʂ��󂯂�ƍU�����L�����Z������j
    public float CancelDamageThreshold;

    public float CancelSelfInflictedDamage;

    public FiringPortType FiringPortType;

    //public int SimultaneousPorts = 1;

    // �U�������s�����܂ł̗P�\���Ԃ��o�߂������ǂ����������v���p�e�B�i�f�o�b�O�p�j
    public bool IsAttackReady(float elapsedTime)
    {
        return elapsedTime >= AttackPreparationTime;
    }
}

    //public PortOccupiedBehavior occupiedBehavior;

    //public PortDamageManagementType damageManagementType;