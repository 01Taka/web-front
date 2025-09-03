using System.Collections.Generic;
using UnityEngine;

public class ConcreteBossManager : BaseBossManager
{
    [Header("�O���R���|�[�l���g")]
    [SerializeField]
    private Transform _bossAttackHolder;

    public IDamageable _playerTarget;

    private BossAttackManager _attackManager;

    protected override void Awake()
    {
        base.Awake();

        // BossAttackManager�̃C���X�^���X���쐬���āA�U���Ǘ���S��������
        _attackManager = new BossAttackManager();
        _attackManager.Initialize(this, InitializeBossAttackLogicExecutor(), CurrentPhaseSettings.firingPorts);
    }

    private BossAttackLogicExecutor InitializeBossAttackLogicExecutor()
    {
        Dictionary<BossAttackType, IBossAttack> attackLogics = new Dictionary<BossAttackType, IBossAttack>();

        // _bossAttackHolder �ɃA�^�b�`����Ă��邷�ׂĂ� IBossAttack ���擾
        IBossAttack[] allAttacks = _bossAttackHolder.GetComponents<IBossAttack>();

        foreach (var attack in allAttacks)
        {
            Debug.Log(attack.AttackType);

            // �e�U���R���|�[�l���g�� AttackType �v���p�e�B���g�p���Ď����ɒǉ�
            attackLogics.Add(attack.AttackType, attack);
        }

        IBossAnimationController animationController = GetComponent<IBossAnimationController>();
        return new BossAttackLogicExecutor(animationController, this, _playerTarget, attackLogics);
    }

    protected override void ExecuteAttack(AttackPattern pattern)
    {
        // �U���}�l�[�W���[�ɏ������Ϗ�
        _attackManager.ExecuteAttack(pattern);
    }
}
