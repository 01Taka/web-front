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

    [Header("�`���[�W����̍U���p")]
    public bool usesCharge = false;
    public float maxChargeAmount = 10f;
    public float maxChargeMultiplier = 2f;
    public float minRequiredCharge = 0f;
    public float incompleteMultiplier = 0.5f;

    [Header("������ʂ���̍U���p")]
    public bool usesEffects = false;
    public float effectDuration;
    public float effectInterval;
    public float effectRadius;
}
