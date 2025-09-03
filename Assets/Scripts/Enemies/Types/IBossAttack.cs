using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃実行に必要なコンテキスト情報をまとめた構造体
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
    /// 攻撃の準備開始時に呼び出される
    /// </summary>
    void OnBeginPreparation(BossAttackContext context);

    /// <summary>
    /// 攻撃実行時に呼び出される
    /// </summary>
    void ExecuteAttack(BossAttackContext context);

    /// <summary>
    /// 攻撃がキャンセルされた時に呼び出される
    /// </summary>
    void OnCanceledAttack(BossAttackContext context);
}

public interface IBossAttackLogic
{
    /// <summary>
    /// 通常攻撃の準備開始時に呼び出される
    /// </summary>
    void OnBeginPreparation(AttackPattern pattern, BossAttackPort port);

    /// <summary>
    /// 通常攻撃実行時に呼び出される
    /// </summary>
    void ExecuteAttack(AttackPattern pattern, BossAttackPort port);

    /// <summary>
    /// 攻撃がキャンセルされた時に呼び出される
    /// </summary>
    void OnCanceledAttack(AttackPattern pattern, BossAttackPort port);

    /// <summary>
    /// 複数のポートを使う攻撃の準備開始時に呼び出される
    /// </summary>
    void OnBeginMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts);

    /// <summary>
    /// 複数のポートを使う攻撃実行時に呼び出される
    /// </summary>
    void ExecuteMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts);

    /// <summary>
    /// 複数のポートを使う攻撃がキャンセルされた時に呼び出される
    /// </summary>
    void OnCanceledMultiPortAttack(AttackPattern pattern, List<BossAttackPort> allPorts);
}


public struct BossAnimationControllerContext
{
    public BossAttackType AttackType;

    // Mechanical Spider用の設定 (Settings for Mechanical Spider)
    public BossAttackPort AttackPort;
}

public interface IBossAnimationController
{
    // アニメーションを開始する (Starts the animation)
    public void StartAnimation(BossAnimationControllerContext context);
    // アニメーションを終了する (Ends the animation)
    public void EndAnimation(BossAnimationControllerContext context);
}