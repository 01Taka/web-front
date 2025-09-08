using UnityEngine;

[CreateAssetMenu(fileName = "PlayerColor", menuName = "Game/PlayerColor")]
public class PlayerColor : ScriptableObject
{
    public Color[] Colors;

    public Color GetColor(int index)
    {
        if (index < 0 || index >= Colors.Length)
        {
            return Color.white;
        }
        return Colors[index];
    }
}
