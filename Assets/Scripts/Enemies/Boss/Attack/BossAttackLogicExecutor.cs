using System.Collections.Generic;
using UnityEngine;

// MonoBehaviourを継承しない純粋なC#クラス
public class BossAttackLogicExecutor : IBossAttackLogic
{
    private readonly IBossAnimationController _animationController;
    private readonly BaseBossManager _baseBossManager;
    private readonly IDamageable _playerTarget;
    private readonly Dictionary<BossAttackType, IBossAttack> _attackLogics;

    /// <summary>
    /// コンストラクタで必要な依存関係と攻撃ロジックの辞書を受け取ります。
    /// </summary>
    /// <param name="manager">ボスマネージャー</param>
    /// <param name="player">プレイヤーのダメージ可能コンポーネント</param>
    /// <param name="attackLogics">攻撃タイプとIBossAttackの辞書</param>
    public BossAttackLogicExecutor(IBossAnimationController animationController, BaseBossManager manager, IDamageable player, Dictionary<BossAttackType, IBossAttack> attackLogics)
    {
        _animationController = animationController;
        _baseBossManager = manager;
        _playerTarget = player;
        _attackLogics = attackLogics;
    }

    /// <summary>
    /// 攻撃実行に必要なコンテキストを生成します。
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
    /// 攻撃タイプに対応するIBossAttackインスタンスを取得します。
    /// </summary>
    private IBossAttack GetAttackLogic(BossAttackType attackType)
    {
        if (_attackLogics.TryGetValue(attackType, out var logic))
        {
            return logic;
        }
        MyLogger.LogError($"不明な攻撃タイプです: {attackType}");
        return null;
    }

    // --- IBossAttackLogicインターフェースの実装 ---

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