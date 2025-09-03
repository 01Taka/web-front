using UnityEngine;

[CreateAssetMenu(fileName = "NewBossSettings", menuName = "Boss/BossSettings")]
public class BossSettings : ScriptableObject
{
    public BossId id;
    public GameObject bossInstance;

    // �t�F�[�Y���Ƃ̐ݒ�i�����̃t�F�[�Y��ێ��j
    public BossPhaseSettings[] phaseSettings;

    // �t�F�[�Y�����擾
    public int GetPhaseCount()
    {
        return phaseSettings.Length;
    }
}
