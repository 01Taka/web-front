// �V����enum�̒ǉ�
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
    Shared,     // �S�|�[�g�Ń_���[�W�����L
    Individual, // �|�[�g���ƂɃ_���[�W���Ǘ�
}