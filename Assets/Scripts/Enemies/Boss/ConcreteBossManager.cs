using System.Collections.Generic;
using UnityEngine;

public class ConcreteBossManager : BaseBossManager
{
    [Header("�O���R���|�[�l���g")]
    [SerializeField]
    private Transform _bossAttackHolder;
    private BossAttackManager _attackManager;

    public void Initialize(IDamageable attackTarget)
    {

        // �K�{�̊O���R���|�[�l���g���ݒ肳��Ă��邩�`�F�b�N
        if (_bossAttackHolder == null)
        {
            Debug.LogError("`_bossAttackHolder`���ݒ肳��Ă��܂���B", this);
            return;
        }

        // �K�{�̊O���R���|�[�l���g���ݒ肳��Ă��邩�`�F�b�N
        if (attackTarget == null)
        {
            Debug.LogError("`_attackTarget`���ݒ肳��Ă��܂���B", this);
            return;
        }

        // BossAttackManager�̃C���X�^���X���쐬���āA�U���Ǘ���S��������
        _attackManager = new BossAttackManager();
        _attackManager.Initialize(this, InitializeBossAttackLogicExecutor(attackTarget), CurrentPhaseSettings.firingPorts);
    }

    private BossAttackLogicExecutor InitializeBossAttackLogicExecutor(IDamageable attackTarget)
    {
        Dictionary<BossAttackType, IBossAttack> attackLogics = new Dictionary<BossAttackType, IBossAttack>();

        // _bossAttackHolder �ɃA�^�b�`����Ă��邷�ׂĂ� IBossAttack ���擾
        IBossAttack[] allAttacks = _bossAttackHolder.GetComponents<IBossAttack>();

        if (allAttacks.Length == 0)
        {
            Debug.LogWarning("`_bossAttackHolder`��IBossAttack�R���|�[�l���g��������܂���ł����B", this);
        }

        foreach (var attack in allAttacks)
        {
            if (attack == null)
            {
                Debug.LogWarning("IBossAttack�R���|�[�l���g��null�ł��B�X�L�b�v���܂��B", this);
                continue;
            }

            attack.InitializeAttackCallbacks(_attackManager);

            // �e�U���R���|�[�l���g�� AttackType �v���p�e�B���g�p���Ď����ɒǉ�
            // ���� AttackType �����݂��Ȃ����`�F�b�N
            if (!attackLogics.ContainsKey(attack.AttackType))
            {
                attackLogics.Add(attack.AttackType, attack);
            }
            else
            {
                Debug.LogWarning($"���ɓ���AttackType '{attack.AttackType}'�������ɑ��݂��܂��B�V�����R���|�[�l���g�͖�������܂��B", this);
            }
        }

        // �A�j���[�V�����R���g���[���[�̑��݂��`�F�b�N
        IBossAnimationController animationController = GetComponent<IBossAnimationController>();
        if (animationController == null)
        {
            Debug.LogError("`IBossAnimationController`�R���|�[�l���g�����̃Q�[���I�u�W�F�N�g�Ɍ�����܂���ł����B", this);
            return null; // �������^�[��
        }

        return new BossAttackLogicExecutor(animationController, this, attackTarget, attackLogics);
    }

    protected override void ExecuteAttack(AttackPattern pattern)
    {
        // �U���}�l�[�W���[��null�łȂ����Ƃ��m�F���Ă��珈�����Ϗ�
        if (_attackManager != null)
        {
            _attackManager.ExecuteAttack(pattern);
        }
        else
        {
            Debug.LogError("`_attackManager`������������Ă��܂���B�U�������s�ł��܂���B", this);
        }
    }
}