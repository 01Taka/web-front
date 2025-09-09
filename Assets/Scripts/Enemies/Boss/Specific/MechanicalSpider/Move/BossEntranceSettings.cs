using UnityEngine;

[CreateAssetMenu(fileName = "BossEntranceSettings", menuName = "Boss/Boss Entrance Settings", order = 1)]
public class BossEntranceSettings : ScriptableObject
{
    public ScreenShakeSettings ScreenShakeSettings;

    [Header("�����̐ݒ�")]
    [Tooltip("�����̃I�[�f�B�I�N���b�v�z��B�����ݒ肷��ƃ����_���ɍĐ�����܂��B")]
    public AudioClip[] FootstepClips;

    [Tooltip("������炷����")]
    public int NumberOfFootsteps = 3;

    [Tooltip("������炷�Ԋu�i�b�j")]
    public float FootstepInterval = 0.5f;

    [Header("�{�X�̒��n�ݒ�")]
    [Tooltip("�{�X�̒��n���̃I�[�f�B�I�N���b�v")]
    public AudioClip LandingClip;

    [Tooltip("�W�����v�̍���")]
    public float JumpHeight = 5f;

    [Tooltip("�W�����v�̏��v���ԁi�b�j�B���n����炷�܂ł̎���")]
    public float JumpDuration = 1.0f;

    [Tooltip("�W�����v�̈ړ��ɂ����鎞�ԁi�b�j�BJumpDuration�����Z���K�v������܂�")]
    public float JumpMoveDuration = 0.5f;
}