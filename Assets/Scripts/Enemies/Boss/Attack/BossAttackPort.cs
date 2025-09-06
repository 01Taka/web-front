
public class BossAttackPort
{
    private BossFiringPort _firingPortType;
    private bool isAvailable = true;

    // �O�����痘�p�\���`�F�b�N���邽�߂̃v���p�e�B
    public bool IsAvailable => isAvailable;

    // FiringPort�̃^�C�v���擾����v���p�e�B��ǉ�
    public BossFiringPort FiringPortType => _firingPortType;

    public BossAttackPort(BossFiringPort firingPortType)
    {
        _firingPortType = firingPortType;
    }

    public void SetAvailability(bool isAvailable)
    {
        this.isAvailable = isAvailable;
    }
}