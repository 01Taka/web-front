using UnityEngine;

[CreateAssetMenu(fileName = "AttackSettings", menuName = "Attack/AttackSettings")]
public class AttackInputSettings : ScriptableObject
{
    [Header("UI and Visuals")]
    public GameObject circlePrefab;
    public float radius;
    public bool mirrorHorizontally;

    [Header("Attack Parameters")]
    public float volleyBurstHoldDuration = 0.2f;
    public float webMineChargeMaxAngle = 60f;
    public float webMineFireBorder = 60f;

    //[SerializeField] private Direction _inputDirection = Direction.Up;

}
