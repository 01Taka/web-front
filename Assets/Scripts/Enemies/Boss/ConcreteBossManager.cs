using System.Collections.Generic;
using UnityEngine;

public class ConcreteBossManager : BaseBossManager
{
    [Header("外部コンポーネント")]
    [SerializeField]
    private Transform _bossAttackHolder;
    private BossAttackManager _attackManager;

    public void Initialize(IDamageable attackTarget)
    {

        // 必須の外部コンポーネントが設定されているかチェック
        if (_bossAttackHolder == null)
        {
            Debug.LogError("`_bossAttackHolder`が設定されていません。", this);
            return;
        }

        // 必須の外部コンポーネントが設定されているかチェック
        if (attackTarget == null)
        {
            Debug.LogError("`_attackTarget`が設定されていません。", this);
            return;
        }

        // BossAttackManagerのインスタンスを作成して、攻撃管理を担当させる
        _attackManager = new BossAttackManager();
        _attackManager.Initialize(this, InitializeBossAttackLogicExecutor(attackTarget), CurrentPhaseSettings.firingPorts);
    }

    private BossAttackLogicExecutor InitializeBossAttackLogicExecutor(IDamageable attackTarget)
    {
        Dictionary<BossAttackType, IBossAttack> attackLogics = new Dictionary<BossAttackType, IBossAttack>();

        // _bossAttackHolder にアタッチされているすべての IBossAttack を取得
        IBossAttack[] allAttacks = _bossAttackHolder.GetComponents<IBossAttack>();

        if (allAttacks.Length == 0)
        {
            Debug.LogWarning("`_bossAttackHolder`にIBossAttackコンポーネントが見つかりませんでした。", this);
        }

        foreach (var attack in allAttacks)
        {
            if (attack == null)
            {
                Debug.LogWarning("IBossAttackコンポーネントがnullです。スキップします。", this);
                continue;
            }

            attack.InitializeAttackCallbacks(_attackManager);

            // 各攻撃コンポーネントの AttackType プロパティを使用して辞書に追加
            // 同じ AttackType が存在しないかチェック
            if (!attackLogics.ContainsKey(attack.AttackType))
            {
                attackLogics.Add(attack.AttackType, attack);
            }
            else
            {
                Debug.LogWarning($"既に同じAttackType '{attack.AttackType}'が辞書に存在します。新しいコンポーネントは無視されます。", this);
            }
        }

        // アニメーションコントローラーの存在をチェック
        IBossAnimationController animationController = GetComponent<IBossAnimationController>();
        if (animationController == null)
        {
            Debug.LogError("`IBossAnimationController`コンポーネントがこのゲームオブジェクトに見つかりませんでした。", this);
            return null; // 早期リターン
        }

        return new BossAttackLogicExecutor(animationController, this, attackTarget, attackLogics);
    }

    protected override void ExecuteAttack(AttackPattern pattern)
    {
        // 攻撃マネージャーがnullでないことを確認してから処理を委譲
        if (_attackManager != null)
        {
            _attackManager.ExecuteAttack(pattern);
        }
        else
        {
            Debug.LogError("`_attackManager`が初期化されていません。攻撃を実行できません。", this);
        }
    }
}