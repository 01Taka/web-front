using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] private float zPos = 0f;
    [SerializeField] private AttackDatabase attackDatabase;

    [Header("VolleyBurstOptions")]
    [SerializeField] float spreadAngle = 15f;
    [SerializeField] float rangeVariance = 0.2f;

    private ProjectileSpawner projectileSpawner;

    private void Start()
    {
        projectileSpawner = GetComponent<ProjectileSpawner>();
    }

    public void HandleAttack(AttackData data)
    {
        var attackConfig = attackDatabase.GetData(data.Type);
        if (attackConfig == null)
        {
            Debug.LogWarning($"No config found for AttackType {data.Type}");
            return;
        }

        Vector3 attackPos = SceneComponentManager.Instance.AttackPointManager.GetAttackPoint(data.AttackPoint);
        attackPos.z = zPos;

        Vector3 direction = data.Direction.normalized;
        float damage = attackConfig.baseDamage;
        float speed = attackConfig.baseSpeed;
        float range = attackConfig.range;
        float detectionRadius = attackConfig.detectionRadius;
        float effectDuration = attackConfig.effectDuration;
        float effectInterval = attackConfig.effectInterval;
        float effectRadius = attackConfig.effectRadius;

        // �e�^�C�v���Ƃ̋���
        switch (data.Type)
        {
            case AttackType.ChargedPierce:
                {
                    // �`���[�W�����i0-1�j
                    float chargeRatio = Mathf.Clamp01(data.ChargeAmount / attackConfig.maxChargeAmount);

                    // �ŏ��`���[�W�������l���`�F�b�N�i��: 0.2f�j
                    if (chargeRatio < attackConfig.minRequiredCharge)
                    {
                        Debug.Log($"�`���[�W�s��: {data.ChargeAmount}�i�K�v: {attackConfig.minRequiredCharge * attackConfig.maxChargeAmount}�j");

                        // �ア�U�������ɏo�� or �s��
                        damage *= attackConfig.incompleteMultiplier;
                        speed *= attackConfig.incompleteMultiplier;
                        range *= attackConfig.incompleteMultiplier;
                        break;
                    }

                    // �L���`���[�W�͈͂��Đ��K���i0-1�j
                    float t = (chargeRatio - attackConfig.minRequiredCharge) / (1f - attackConfig.minRequiredCharge);

                    float multiplier = Mathf.Lerp(1f, attackConfig.maxChargeMultiplier, t);
                    Debug.Log($"[ChargedPierce] mul: {multiplier}, charge: {data.ChargeAmount}");

                    damage *= multiplier;
                    speed *= multiplier;
                    range *= multiplier;
                    break;
                }
            case AttackType.VolleyBurst:
                {
                    float angleOffset = Random.Range(-spreadAngle, spreadAngle);
                    direction = Quaternion.Euler(0, 0, angleOffset) * direction;

                    float distanceMultiplier = Random.Range(1f - rangeVariance, 1f + rangeVariance);
                    range *= distanceMultiplier;

                    break;
                }

            case AttackType.WebMine:
                {
                    // �`���[�W�����i0-1�j
                    float chargeRatio = Mathf.Clamp01(data.ChargeAmount / attackConfig.maxChargeAmount);

                    // �L���`���[�W�͈͂� 0-1 �ɍĐ��K��
                    float t = (chargeRatio - attackConfig.minRequiredCharge) / (1f - attackConfig.minRequiredCharge);
                    t = Mathf.Clamp01(t); // �O�̂��ߐ���

                    float multiplier = Mathf.Lerp(1f, attackConfig.maxChargeMultiplier, t);

                    Debug.Log($"[WebMine] mul: {multiplier}, charge: {data.ChargeAmount}");

                    damage *= multiplier;
                    effectRadius *= multiplier;
                    effectDuration *= multiplier;

                    break;
                }

            case AttackType.SilkSnare:
                {
                    // SilkSnare �̓V���v���ȘA�ˌn
                    // ���ʂȋ�����Projectile�ŏ���
                    break;
                }
        }

        ProjectileSpawnParams spawnParams = new()
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

        projectileSpawner.SpawnProjectile(spawnParams, attackConfig.projectilePrefab);
    }
}
