using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MechanicalSpiderBomSettings", menuName = "Boss/MechanicalSpider/Bom", order = 1)]
public class MechanicalSpiderBomSettings : ScriptableObject
{
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

    [Header("�����ݒ� (���Ԑ؂�)")]
    public AudioClip BomClip;
    [Tooltip("�������ɐ�������v���n�u")]
    public GameObject ExplosionPrefab;

    [Header("�����ݒ� (�j���)")]
    [Tooltip("�j���ɖڕW���B�������̔�����")]
    public AudioClip ExplosionClipOnDestroy;
    [Tooltip("�j���ɖڕW���B�������̔����v���n�u")]
    public GameObject ExplosionPrefabOnDestroy;
    [Tooltip("�����܂ł̎��Ԃɉ��Z���郉���_���l�͈̔� (�b)")]
    public float ExplosionTimeRandomRange = 0.5f;
    [Tooltip("�������Ƀ^�[�Q�b�g���W���烉���_���ɂ��炷���a")]
    public float ExplosionRadius = 0.5f;

    [Header("�ǐՂ�HP�ݒ�")]
    [Tooltip("�ǐՑ��x")]
    public float SeekSpeed = 5f;
    [Tooltip("�j���̒ǐՑ��x")]
    public float SeekSpeedOnDestroyed = 5f;
}