using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour���p�����Ȃ�������C#�N���X
public class BossAttackLogicExecutor : IBossAttackLogic
{
    private readonly IBossAnimationController _animationController;
    private readonly BaseBossManager _baseBossManager;
    private readonly IDamageable _playerTarget;
    private readonly Dictionary<BossAttackType, IBossAttack> _attackLogics;

    /// <summary>
    /// �R���X�g���N�^�ŕK�v�Ȉˑ��֌W�ƍU�����W�b�N�̎������󂯎��܂��B
    /// </summary>
    /// <param name="manager">�{�X�}�l�[�W���[</param>
    /// <param name="player">�v���C���[�̃_���[�W�\�R���|�[�l���g</param>
    /// <param name="attackLogics">�U���^�C�v��IBossAttack�̎���</param>
    public BossAttackLogicExecutor(IBossAnimationController animationController, BaseBossManager manager, IDamageable player, Dictionary<BossAttackType, IBossAttack> attackLogics)
    {
        _animationController = animationController;
        _baseBossManager = manager;
        _playerTarget = player;
        _attackLogics = attackLogics;
    }

    /// <summary>
    /// �U�����s�ɕK�v�ȃR���e�L�X�g�𐶐����܂��B
    /// </summary>
    private BossAttackContext CreateContext(AttackPattern pattern, BossAttackPort singlePort = null, List<BossAttackPort> multiPorts = null)
    {
        return new BossAttackContext
        {
            Pattern = pattern,
            AnimationController = _animationController,
            BossManager = _baseBossManager,
            PlayerDamageable = _playerTarget,
            SinglePort = singlePort,
            MultiPorts = multiPorts
        };
    }

    /// <summary>
    /// �U���^�C�v�ɑΉ�����IBossAttack�C���X�^���X���擾���܂��B
    /// </summary>
    private IBossAttack GetAttackLogic(BossAttackType attackType)
    {
        if (_attackLogics.TryGetValue(attackType, out var logic))
        {
            return logic;
        }
        MyLogger.LogError($"�s���ȍU���^�C�v�ł�: {attackType}");
        return null;
    }

    // --- IBossAttackLogic�C���^�[�t�F�[�X�̎��� ---

    public void OnBeginPreparation(AttackPattern pattern, BossAttackPort port)
    {
        var logic = GetAttackLogic(pattern.AttackType);
        if (logic != null)
        {
            var context = CreateContext(pattern, singlePort: port);
            logic.OnBeginPreparation(context);
        }
    }

    public void ExecuteAttack(AttackPattern pattern, BossAttackPort port)
    {
        var logic = GetAttackLogic(pattern.AttackType);
        if (logic != null)
        {
            var context = CreateContext(pattern, singlePort: port);
            logic.ExecuteAttack(context);
        }
    }

    public void OnCanceledAttack(AttackPattern pattern, BossAttackPort port)
    {
        var logic = GetAttackLogic(pattern.AttackType);
        if (logic != null)
        {
            var context = CreateContext(pattern, singlePort: port);
            logic.OnCanceledAttack(context);
        }
    }

    public void OnBeginMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts)
    {
        var logic = GetAttackLogic(pattern.AttackType);
        if (logic != null)
        {
            var context = CreateContext(pattern, multiPorts: allPorts);
            logic.OnBeginPreparation(context);
        }
    }

    public void ExecuteMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts)
    {
        var logic = GetAttackLogic(pattern.AttackType);
        if (logic != null)
        {
            var context = CreateContext(pattern, multiPorts: allPorts);
            logic.ExecuteAttack(context);
        }
    }

    public void OnCanceledMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts)
    {
        var logic = GetAttackLogic(pattern.AttackType);
        if (logic != null)
        {
            var context = CreateContext(pattern, multiPorts: allPorts);
            logic.OnCanceledAttack(context);
        }
    }
}