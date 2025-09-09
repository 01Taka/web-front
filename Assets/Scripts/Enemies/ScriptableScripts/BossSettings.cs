using UnityEngine;

[CreateAssetMenu(fileName = "NewBossSettings", menuName = "Boss/BossSettings")]
public class BossSettings : ScriptableObject
{
    public BossId id;
    public GameObject bossInstance;
    public BossEntranceSettings entranceSettings;

    [Tooltip("�{�X�̑��̗́B")]
    public float maxHealth;

    [Tooltip("�t�F�[�Y���Ƃ̐ݒ�B")]
    public BossPhaseSettings[] phaseSettings;

    // �t�F�[�Y�����擾
    public int GetPhaseCount()
    {
        return phaseSettings.Length;
    }

    // OnValidate�̓G�f�B�^�Œl���ύX���ꂽ�Ƃ��ɌĂяo�����
    private void OnValidate()
    {
        if (phaseSettings == null || phaseSettings.Length == 0)
        {
            return;
        }

        // �ŏ��̃t�F�[�Y�̈ڍs�p�[�Z���e�[�W��100%�����ł��邱�Ƃ�����
        if (phaseSettings[0].phaseTransitionHealthPercentage >= 1.0f)
        {
            Debug.LogError("�ŏ��̃t�F�[�Y�̈ڍs�p�[�Z���e�[�W��100%�����ɐݒ肵�Ă��������B");
        }

        // �t�F�[�Y���Ƃ̈ڍs�p�[�Z���e�[�W���O�̃t�F�[�Y��荂���Ȃ����Ƃ�����
        for (int i = 1; i < phaseSettings.Length; i++)
        {
            if (phaseSettings[i].phaseTransitionHealthPercentage > phaseSettings[i - 1].phaseTransitionHealthPercentage)
            {
                Debug.LogError($"�t�F�[�Y {i + 1} �̈ڍs�p�[�Z���e�[�W ({phaseSettings[i].phaseTransitionHealthPercentage * 100}%) ���A�O�̃t�F�[�Y {i} �̃p�[�Z���e�[�W ({phaseSettings[i - 1].phaseTransitionHealthPercentage * 100}%) ��荂���ݒ肳��Ă��܂��B�p�[�Z���e�[�W�͏�Ɍ�������K�v������܂��B");
            }
        }
    }
}