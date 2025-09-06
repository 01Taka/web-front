
public class BossAttackPort
{
    private BossFiringPort _firingPortType;
    private bool isAvailable = true;

    // 外部から利用可能かチェックするためのプロパティ
    public bool IsAvailable => isAvailable;

    // FiringPortのタイプを取得するプロパティを追加
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