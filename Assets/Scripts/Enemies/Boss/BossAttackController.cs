using System.Collections;
using UnityEngine;

/// <summary>
/// ボスの攻撃パターンと実行を管理するクラス。
/// </summary>
public class BossAttackController : MonoBehaviour
{
    private BaseBossManager _bossManager;
    private Coroutine _attackRoutine;

    private BossPhaseSettings _currentPhaseSettings;

    // 攻撃が実行中かどうかを示すプロパティ
    public bool IsAttacking { get; private set; }

    public void Initialize(BaseBossManager bossManager, BossPhaseSettings phaseSettings)
    {
        _bossManager = bossManager;
        _currentPhaseSettings = phaseSettings;
    }

    public void UpdatePhaseSettings(BossPhaseSettings phaseSettings)
    {
        _currentPhaseSettings = phaseSettings;
    }

    /// <summary>
    /// 攻撃ルーチンを開始する
    /// </summary>
    public void StartAttacking()
    {
        StopAttacking();
        _attackRoutine = StartCoroutine(AttackRoutine());
    }

    /// <summary>
    /// 攻撃ルーチンを停止する
    /// </summary>
    public void StopAttacking()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
            IsAttacking = false; // 攻撃停止時もフラグをリセット
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 最初の攻撃までの待機時間
        yield return new WaitForSeconds(Random.Range(_currentPhaseSettings.minAttackInterval, _currentPhaseSettings.maxAttackInterval));

        while (true)
        {
            IsAttacking = true; // 攻撃開始をマーク
            AttackPattern executedAttack = PerformRandomAttack();

            float waitTime = Random.Range(_currentPhaseSettings.minAttackInterval, _currentPhaseSettings.maxAttackInterval);

            if (executedAttack != null)
            {
                waitTime += executedAttack.AttackPreparationTime + executedAttack.AttackDuration;
            }

            // 攻撃の実行が終わるまで待つ
            yield return new WaitForSeconds(executedAttack.AttackPreparationTime + executedAttack.AttackDuration);

            IsAttacking = false; // 攻撃終了をマーク

            // 次の攻撃までの待機時間
            yield return new WaitForSeconds(waitTime);
        }
    }

    public AttackPattern PerformRandomAttack()
    {
        AttackPattern[] patterns = _currentPhaseSettings.attackPatterns;

        if (patterns == null || patterns.Length == 0)
        {
            Debug.LogWarning("No attack patterns found for the current phase.");
            return null;
        }

        int randomIndex = Random.Range(0, patterns.Length);
        AttackPattern selectedPattern = patterns[randomIndex];
        _bossManager.ExecuteAttack(selectedPattern);
        return selectedPattern;
    }
}