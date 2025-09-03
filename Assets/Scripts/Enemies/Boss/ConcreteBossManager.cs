using System.Collections.Generic;
using UnityEngine;

public class ConcreteBossManager : BaseBossManager
{
    [Header("外部コンポーネント")]
    [SerializeField]
    private Transform _bossAttackHolder;

    public IDamageable _playerTarget;

    private BossAttackManager _attackManager;

    protected override void Awake()
    {
        base.Awake();

        // BossAttackManagerのインスタンスを作成して、攻撃管理を担当させる
        _attackManager = new BossAttackManager();
        _attackManager.Initialize(this, InitializeBossAttackLogicExecutor(), CurrentPhaseSettings.firingPorts);
    }

    private BossAttackLogicExecutor InitializeBossAttackLogicExecutor()
    {
        Dictionary<BossAttackType, IBossAttack> attackLogics = new Dictionary<BossAttackType, IBossAttack>();

        // _bossAttackHolder にアタッチされているすべての IBossAttack を取得
        IBossAttack[] allAttacks = _bossAttackHolder.GetComponents<IBossAttack>();

        foreach (var attack in allAttacks)
        {
            Debug.Log(attack.AttackType);

            // 各攻撃コンポーネントの AttackType プロパティを使用して辞書に追加
            attackLogics.Add(attack.AttackType, attack);
        }

        IBossAnimationController animationController = GetComponent<IBossAnimationController>();
        return new BossAttackLogicExecutor(animationController, this, _playerTarget, attackLogics);
    }

    protected override void ExecuteAttack(AttackPattern pattern)
    {
        // 攻撃マネージャーに処理を委譲
        _attackManager.ExecuteAttack(pattern);
    }
}
