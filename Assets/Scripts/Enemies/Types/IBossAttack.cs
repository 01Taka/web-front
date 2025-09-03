using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �U�����s�ɕK�v�ȃR���e�L�X�g�����܂Ƃ߂��\����
/// </summary>
public struct BossAttackContext
{
    public AttackPattern Pattern;
    public IBossAnimationController AnimationController;
    public BaseBossManager BossManager;
    public IDamageable PlayerDamageable;
    public BossAttackPort SinglePort;
    public List<BossAttackPort> MultiPorts;
}

public interface IBossAttack
{
    public abstract BossAttackType AttackType { get; }

    /// <summary>
    /// �U���̏����J�n���ɌĂяo�����
    /// </summary>
    void OnBeginPreparation(BossAttackContext context);

    /// <summary>
    /// �U�����s���ɌĂяo�����
    /// </summary>
    void ExecuteAttack(BossAttackContext context);

    /// <summary>
    /// �U�����L�����Z�����ꂽ���ɌĂяo�����
    /// </summary>
    void OnCanceledAttack(BossAttackContext context);
}

public interface IBossAttackLogic
{
    /// <summary>
    /// �ʏ�U���̏����J�n���ɌĂяo�����
    /// </summary>
    void OnBeginPreparation(AttackPattern pattern, BossAttackPort port);

    /// <summary>
    /// �ʏ�U�����s���ɌĂяo�����
    /// </summary>
    void ExecuteAttack(AttackPattern pattern, BossAttackPort port);

    /// <summary>
    /// �U�����L�����Z�����ꂽ���ɌĂяo�����
    /// </summary>
    void OnCanceledAttack(AttackPattern pattern, BossAttackPort port);

    /// <summary>
    /// �����̃|�[�g���g���U���̏����J�n���ɌĂяo�����
    /// </summary>
    void OnBeginMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts);

    /// <summary>
    /// �����̃|�[�g���g���U�����s���ɌĂяo�����
    /// </summary>
    void ExecuteMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts);

    /// <summary>
    /// �����̃|�[�g���g���U�����L�����Z�����ꂽ���ɌĂяo�����
    /// </summary>
    void OnCanceledMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts);
}


public struct BossAnimationControllerContext
{
    public BossAttackType AttackType;

    // Mechanical Spider�p�̐ݒ� (Settings for Mechanical Spider)
    public BossAttackPort AttackPort;
}

public interface IBossAnimationController
{
    // �A�j���[�V�������J�n���� (Starts the animation)
    public void StartAnimation(BossAnimationControllerContext context);
    // �A�j���[�V�������I������ (Ends the animation)
    public void EndAnimation(BossAnimationControllerContext context);
}