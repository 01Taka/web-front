using UnityEngine;

[CreateAssetMenu(fileName = "AttackDatabase", menuName = "Attack/AttackDatabase")]
public class AttackDatabase : ScriptableObject
{
    public AttackTypeData[] attackTypes;

    public AttackTypeData GetData(AttackType type)
    {
        foreach (var data in attackTypes)
        {
            if (data.type == type)
                return data;
        }
        Debug.LogWarning($"AttackTypeData not found for {type}");
        return null;
    }
}
