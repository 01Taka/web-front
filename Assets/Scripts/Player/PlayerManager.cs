using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamageable
{
    public DamageTakenManager DamageTakenManager = new DamageTakenManager();
    public DamageDealtManager DamageDealtManager = new DamageDealtManager();

    private void Start()
    {
        List<int> players = new List<int>() { 0, 1, 2, 3 };

        foreach (var playerId in players)
        {
            AddPlayer(playerId);
        }
    }

    public void AddPlayer(int playerId)
    {
        DamageDealtManager.AddPlayer(playerId);
    }

    public void TakeDamage(float damage)
    {
        DamageTakenManager.TakeDamage(damage);
    }

    public void DealtDamage(float damage, int playerId)
    {
        DamageDealtManager.DealteDamage(damage, playerId);
    }
}
