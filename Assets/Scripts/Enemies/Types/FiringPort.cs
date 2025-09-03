// 新しいenumの追加
public enum BossFiringPort
{
    None,
    RightFrontLeg,
    LeftFrontLeg,
    RightRearLeg,
    LeftRearLeg,
}

public enum FiringPortType
{
    None,
    Single,
    All,
}

public enum PortOccupiedBehavior
{
    CancelNewAttack,
    OverrideOldAttack,
    WaitUntilFree,
}

public enum PortDamageManagementType
{
    Shared,     // 全ポートでダメージを共有
    Individual, // ポートごとにダメージを管理
}