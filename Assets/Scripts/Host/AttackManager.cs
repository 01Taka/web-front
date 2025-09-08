using Fusion;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] private float zPos = 0f;
    [SerializeField] private AttackDatabase attackDatabase;

    [Header("TestOptions")]
    [SerializeField] private Transform _testAttackPoint;
    [SerializeField] public bool IsActiveTestMode;

    public void SetActiveTestMode(bool isActiveTestMode)
    {
        IsActiveTestMode = isActiveTestMode;
    }

    private ProjectileSpawner projectileSpawner;

    private void Start()
    {
        // Nullチェックを追加して、コンポーネントの割り当て漏れを防ぐ
        projectileSpawner = GetComponent<ProjectileSpawner>();
        if (projectileSpawner == null)
        {
            Debug.LogError("ProjectileSpawner component is missing on this GameObject.", this);
        }
    }

    public bool TryGetAttackPosition(PlayerRef attacker, out Vector3 attackPosition)
    {
        attackPosition = Vector3.zero;

        if (IsActiveTestMode)
        {
            attackPosition = _testAttackPoint.position;
            return true;
        }

        if (!GlobalRegistry.Instance.GetNetworkPlayerManager().TryGetCompactedIndex(attacker, out int pointIndex))
        {
            Debug.LogWarning("Failed to get player index. Attack aborted.");
            return false;
        }

        if (!SceneComponentManager.Instance.AttackPointManager.TryGetAttackPoint(pointIndex, out Vector3 attackPos))
        {
            Debug.LogWarning("Failed to get attack position.");
            return false;
        }

        attackPosition = attackPos;
        return true;
    }

    public void HandleAttack(AttackData data)
    {
        // 1. 早期リターン: 必須コンポーネントのチェック
        if (projectileSpawner == null)
        {
            Debug.LogError("ProjectileSpawner is not available. Cannot handle attack.", this);
            return;
        }

        // 2. 早期リターン: AttackDataの取得
        var attackConfig = attackDatabase.GetData(data.Type);
        if (attackConfig == null)
        {
            Debug.LogWarning($"No config found for AttackType {data.Type}. Attack aborted.");
            return;
        }

        // 3. 早期リターン: 発射位置の取得
        if (!TryGetAttackPosition(data.AttackerRef, out Vector3 attackPos))
        {
            return;
        }

        attackPos.z = zPos;

        // 4. ベースパラメータの初期化
        Vector3 direction = data.Direction.normalized;
        float damage = attackConfig.baseDamage;
        float speed = attackConfig.baseSpeed;
        float range = attackConfig.range;
        float detectionRadius = attackConfig.detectionRadius;
        float effectDuration = attackConfig.effectDuration;
        float effectInterval = attackConfig.effectInterval;
        float effectRadius = attackConfig.effectRadius;

        float chargeRatio = 0f;
        float multiplier = 1f;

        // 5. 各タイプごとの固有挙動
        switch (data.Type)
        {
            case AttackType.ChargedPierce:
            case AttackType.WebMine:
                // チャージ処理の共通ロジック
                chargeRatio = Mathf.Clamp01(data.ChargeAmount / attackConfig.maxChargeAmount);

                // 最小チャージしきい値チェック
                if (chargeRatio < attackConfig.minRequiredCharge)
                {
                    damage *= attackConfig.incompleteMultiplier;
                    // 他のパラメータにも適用したい場合はここで乗算
                    if (data.Type == AttackType.ChargedPierce)
                    {
                        speed *= attackConfig.incompleteMultiplier;
                        range *= attackConfig.incompleteMultiplier;
                    }
                    else // WebMineの場合
                    {
                        effectRadius *= attackConfig.incompleteMultiplier;
                        effectDuration *= attackConfig.incompleteMultiplier;
                    }
                }
                else
                {
                    // 有効チャージ範囲を再正規化（0-1）
                    float t = (chargeRatio - attackConfig.minRequiredCharge) / (1f - attackConfig.minRequiredCharge);
                    t = Mathf.Clamp01(t); // 念のためクランプ
                    multiplier = Mathf.Lerp(1f, attackConfig.maxChargeMultiplier, t);

                    Debug.Log($"[{data.Type}] Charge Multiplier: {multiplier}, Amount: {data.ChargeAmount}");
                    damage *= multiplier;

                    if (data.Type == AttackType.ChargedPierce)
                    {
                        speed *= multiplier;
                        range *= multiplier;
                    }
                    else // WebMineの場合
                    {
                        effectRadius *= multiplier;
                        effectDuration *= multiplier;
                    }
                }
                break;

            case AttackType.VolleyBurst:
                float angleOffset = Random.Range(-attackConfig.spreadAngle, attackConfig.spreadAngle);
                direction = Quaternion.Euler(0, 0, angleOffset) * direction;

                float distanceMultiplier = Random.Range(1f - attackConfig.rangeVariance, 1f + attackConfig.rangeVariance);
                range *= distanceMultiplier;
                break;

            case AttackType.SilkSnare:
                // シンプルな攻撃のため、ここでは特別な処理なし
                break;
        }

        // 6. ProjectileSpawnParamsの生成
        var spawnParams = new ProjectileSpawnParams
        {
            Type = data.Type,
            Position = attackPos,
            Direction = direction,
            Damage = damage,
            Speed = speed,
            Range = range,
            DetectionRadius = detectionRadius,
            EffectDuration = effectDuration,
            EffectInterval = effectInterval,
            EffectRadius = effectRadius,
        };

        // 7. 発射処理
        projectileSpawner.SpawnProjectile(spawnParams, attackConfig.projectilePrefab);
    }
}