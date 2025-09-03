using UnityEngine;

[CreateAssetMenu(fileName = "GameSetting", menuName = "Game/GameSetting")]
public class GameSetting : ScriptableObject
{
    public float TimeLimit = 120f;
    public float ExtraTimeOnPhaseChange = 60f;
}
