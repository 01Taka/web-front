using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AttackPointSet
{
    public int pointIndex;
    public Transform point;
}

public class AttackPointManager : MonoBehaviour
{
    [SerializeField] private List<AttackPointSet> attackPoints;

    private Dictionary<int, Transform> attackPointMap;

    private void Awake()
    {
        attackPointMap = new Dictionary<int, Transform>();

        foreach (var aps in attackPoints)
        {
            if (aps.point != null && !attackPointMap.ContainsKey(aps.pointIndex))
            {
                attackPointMap.Add(aps.pointIndex, aps.point);
            }
        }
    }

    public Vector3 GetAttackPoint(int pointIndex)
    {
        if (attackPointMap.TryGetValue(pointIndex, out Transform point) && point != null)
        {
            return point.position;
        }

        Debug.LogWarning($"Attack point with index {pointIndex} not found.");
        return Vector3.zero;
    }
}
