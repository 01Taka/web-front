using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BarrierColorSet
{
    public Color barrierColor;
    public Color patternColor;
}

[CreateAssetMenu(fileName = "New Barrier Color Config", menuName = "Barrier/Color Config", order = 1)]
public class BarrierColorConfig : ScriptableObject
{
    public List<BarrierColorSet> ColorSteps = new List<BarrierColorSet>();
}