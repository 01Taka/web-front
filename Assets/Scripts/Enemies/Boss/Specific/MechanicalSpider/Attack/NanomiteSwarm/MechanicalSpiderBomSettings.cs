using UnityEngine;

[CreateAssetMenu(fileName = "MechanicalSpiderBomSettings", menuName = "Boss/MechanicalSpider/Bom", order = 1)]
public class MechanicalSpiderBomSettings : ScriptableObject
{
    [Header("SpawnerSettings")]
    public int NumberOfBoms = 5;
    public float SpawnRadius = 5f;
    public float ExplosionTime = 3f;

    [Header("��]�ݒ�")]
    [Tooltip("Z������̉�]���x (�x/�b)")]
    public float RotationSpeed = 100f;
    [Tooltip("�j���̉�]���x (�x/�b)")]
    public float RotationSpeedOnDestroyed = 300f;

    [Header("�_�Őݒ�")]
    [Tooltip("�_�ł̋K��F")]
    public Color BlinkColor = Color.red;
    [Tooltip("�_�ł̊J�n�p�x (�b)")]
    public float StartBlinkInterval = 0.5f;
    [Tooltip("�_�ł̏I���p�x (�b)")]
    public float EndBlinkInterval = 0.1f;

    [Header("�����ݒ� (�j���)")]
    [Tooltip("�����܂ł̎��Ԃɉ��Z���郉���_���l�͈̔� (�b)")]
    public float ExplosionTimeRandomRange = 0.5f;
    [Tooltip("�������Ƀ^�[�Q�b�g���W���烉���_���ɂ��炷���a")]
    public float ExplosionRadius = 0.5f;

    [Header("�ǐՂ�HP�ݒ�")]
    [Tooltip("�ǐՑ��x")]
    public float SeekSpeed = 5f;
    [Tooltip("�j���̒ǐՑ��x")]
    public float SeekSpeedOnDestroyed = 5f;

    [Header("�T�E���h�ݒ�")]
    public AudioClip BomClip;
    public AudioClip ExplosionClipOnDestroy;
    public AudioClip HitClip;

    public float HitClipVolume = 0.5f;
}