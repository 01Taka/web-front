using UnityEngine;
using System.Collections.Generic;

public class OverloadFistAttack : BossAttackBase
{
    // �U���^�C�v���`
    public override BossAttackType AttackType => BossAttackType.OverloadFist;

    [Header("Overload Fist Specific")]
    [SerializeField] private MechanicalSpiderLegPart[] _legParts; // �C���X�y�N�^�[�Őݒ�

    // �����G�t�F�N�g�ƃT�E���h�G�t�F�N�g
    [SerializeField] private ExplosionType _explosionType; // �����G�t�F�N�g�̎��
    [SerializeField] protected float _explosionSize;
    [SerializeField] private AudioClip _explosionSound; // ������AudioClip

    // �U�����ƂɃC���N�������g�����ÓI�ϐ�
    private static int _recordIdCounter = 0;

    // �U���J�n���ɐ������郌�R�[�hID���Ǘ����鎫��
    private Dictionary<int, int> _recordIds;

    private void Awake()
    {
        _recordIds = new Dictionary<int, int>();
    }

    /// <summary>
    /// �U�������J�n���̏����B
    /// ���R�[�h���J�n���A�_���[�W�R�[���o�b�N��o�^���܂��B
    /// </summary>
    public override void OnBeginPreparation(BossAttackContext context)
    {
        // �U�����ƂɈ�ӂ�ID�𐶐�
        _recordIdCounter++;

        int index = MechanicalSpiderUtils.ConvertToPortToIndex(context.SinglePort);
        MechanicalSpiderLegPart part = _legParts[index];
        _recordIds[index] = _recordIdCounter;

        // ���R�[�h���J�n���A�R�[���o�b�N��o�^
        part.StartRecordAndRegisterCallback(_recordIdCounter, (amount) => OnDamageTakenDuringAttack(context));

        TargetMarkManager.Instance.StartLockOn(part.TargetMarkPosition);

        // �e�N���X�̏��������s
        base.OnBeginPreparation(context);
    }

    /// <summary>
    /// �_���[�W���󂯂��Ƃ��ɌĂяo�����R�[���o�b�N���\�b�h�B
    /// </summary>
    private void OnDamageTakenDuringAttack(BossAttackContext context)
    {
        // ���R�[�hID�ƑΉ�����r�����擾
        if (TryGetRecordInfo(context, out int recordId, out MechanicalSpiderLegPart part))
        {
            float recordedDamage = part.GetRecordedDamage(recordId);

            // 臒l�𒴂������m�F
            if (recordedDamage > context.Pattern.CancelDamageThreshold)
            {
                // �U�����L�����Z��
                _callbacks.CancelAttackOnPort(context.SinglePort.FiringPortType);
            }
        }
    }

    /// <summary>
    /// �U�����L�����Z�����ꂽ�Ƃ��̏����B
    /// ���R�[�h���I�����A�G�t�F�N�g��SE���Đ����܂��B
    /// </summary>
    public override void OnCanceledAttack(BossAttackContext context)
    {
        CleanUpAttack(context);

        // �����G�t�F�N�g��SE���Đ�
        SpawnExplosion(context.SinglePort);

        base.OnCanceledAttack(context);
    }

    /// <summary>
    /// �U�����s���̏����B
    /// ���R�[�h�ƃR�[���o�b�N���I�����܂��B
    /// </summary>
    public override void ExecuteAttack(BossAttackContext context)
    {
        CleanUpAttack(context);
        base.ExecuteAttack(context);
    }

    /// <summary>
    /// �U���I�����̃N���[���A�b�v���������ʉ�
    /// </summary>
    /// <param name="context"></param>
    private void CleanUpAttack(BossAttackContext context)
    {
        TargetMarkManager.Instance.ReleaseLockOn();

        if (TryGetRecordInfo(context, out int recordId, out MechanicalSpiderLegPart part))
        {
            // ���R�[�h���I�����A�R�[���o�b�N������
            part?.EndRecordAndUnregisterCallback(recordId);

            // �������烌�R�[�hID���폜
            int index = MechanicalSpiderUtils.ConvertToPortToIndex(context.SinglePort);
            _recordIds.Remove(index);
        }
    }

    /// <summary>
    /// �R���e�L�X�g���烌�R�[�hID�ƑΉ�����r�������S�Ɏ擾���܂��B
    /// </summary>
    /// <param name="context">�{�X�U���R���e�L�X�g</param>
    /// <param name="recordId">�擾�������R�[�hID</param>
    /// <param name="part">�擾�����r��</param>
    /// <returns>���̎擾�ɐ����������ǂ���</returns>
    private bool TryGetRecordInfo(BossAttackContext context, out int recordId, out MechanicalSpiderLegPart part)
    {
        recordId = -1;
        part = null;
        int index = MechanicalSpiderUtils.ConvertToPortToIndex(context.SinglePort);

        if (_recordIds.TryGetValue(index, out recordId))
        {
            part = _legParts[index];
            return true;
        }

        return false;
    }

    /// <summary>
    /// �����G�t�F�N�g��SE�𐶐��E�Đ����܂��B
    /// </summary>
    /// <param name="port">�U���|�[�g</param>
    private void SpawnExplosion(BossAttackPort port)
    {
        int index = MechanicalSpiderUtils.ConvertToPortToIndex(port);
        Vector3 spawnPosition = _legParts[index].transform.position;
        ExplosionEffectPoolManager.Instance.PlayExplosion(spawnPosition, _explosionSize, _explosionType);
        SoundManager.Instance.PlayEffect(_explosionSound);
    }
}