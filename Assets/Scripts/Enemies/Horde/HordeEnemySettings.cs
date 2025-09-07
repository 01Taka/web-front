using UnityEngine;

/// <summary>
/// HordeEnemy�̐ݒ��ێ�����ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "HordeEnemySettings", menuName = "Enemy/HordeEnemy Settings", order = 1)]
public class HordeEnemySettings : ScriptableObject
{
    [Header("�G�̊�{�ݒ�")]
    public float maxHealth = 10f;
    public float moveSpeed = 3f;
    public float enemyScale = 1.0f;
    public float adjustAngle = 0f;

    [Header("�����ݒ�")]
    public float explosionDistance = 1.5f;

    [Header("�G�t�F�N�g�ݒ�")]
    public ExplosionType DestroyExplosionType = ExplosionType.Default;
    public float DestroyEffectSize;

    [Header("�I�[�f�B�I�ݒ�")]
    public AudioClip damageSound;
    public AudioClip destroySound;
    public AudioClip explodeSound;
    [Range(0, 1)] public float SoundVolume;
}