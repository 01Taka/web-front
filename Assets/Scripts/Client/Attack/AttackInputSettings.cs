using UnityEngine;

[CreateAssetMenu(fileName = "AttackSettings", menuName = "Attack/AttackSettings")]
public class AttackInputSettings : ScriptableObject
{
    [Header("UI and Visuals")]
    public GameObject circlePrefab;
    public GameObject pointerPrefab;
    public float radius;
    public PlayerColor playerColor;
    public float circleAlpha = 0.3f;

    [Header("Attack Parameters")]
    public float volleyBurstHoldDuration = 0.2f;
    public float webMineChargeMaxAngle = 60f;
    public float webMineFireBorder = 60f;

    //[SerializeField] private Direction _inputDirection = Direction.Up;

}
