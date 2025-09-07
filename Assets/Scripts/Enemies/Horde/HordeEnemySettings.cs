using UnityEngine;

/// <summary>
/// HordeEnemyの設定を保持するScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "HordeEnemySettings", menuName = "Enemy/HordeEnemy Settings", order = 1)]
public class HordeEnemySettings : ScriptableObject
{
    [Header("敵の基本設定")]
    public float maxHealth = 10f;
    public float moveSpeed = 3f;
    public float enemyScale = 1.0f;
    public float adjustAngle = 0f;

    [Header("自爆設定")]
    public float explosionDistance = 1.5f;

    [Header("エフェクト設定")]
    public ExplosionType DestroyExplosionType = ExplosionType.Default;
    public float DestroyEffectSize;

    [Header("オーディオ設定")]
    public AudioClip damageSound;
    public AudioClip destroySound;
    public AudioClip explodeSound;
    [Range(0, 1)] public float SoundVolume;
}