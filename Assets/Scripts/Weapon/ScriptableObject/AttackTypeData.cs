using UnityEngine;

[CreateAssetMenu(fileName = "AttackTypeData", menuName = "Attack/AttackTypeData")]
public class AttackTypeData : ScriptableObject
{
    public AttackType type;
    public GameObject projectilePrefab;
    public float baseSpeed = 10f;
    public float baseDamage = 30f;
    public float range = 10f;
    public float detectionRadius = 1f;

    [Header("チャージありの攻撃用")]
    public bool usesCharge = false;
    public float maxChargeAmount = 10f;
    public float maxChargeMultiplier = 2f;
    public float minRequiredCharge = 0f;
    public float incompleteMultiplier = 0.5f;

    [Header("特殊効果ありの攻撃用")]
    public bool usesEffects = false;
    public float effectDuration;
    public float effectInterval;
    public float effectRadius;
}
