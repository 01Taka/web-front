using System.Collections.Generic;
using UnityEngine;

public class DamageDealtManager
{
    private float _totalDamage = 0f;
    public Dictionary<int, float> attackerResults = new Dictionary<int, float>();

    // 総ダメージの取得
    public float TotalDealtDamage => _totalDamage;

    // プレイヤーを追加する（初期値0で）
    public void AddPlayer(int playerId)
    {
        if (!attackerResults.ContainsKey(playerId))
        {
            attackerResults.Add(playerId, 0f);
        }
        else
        {
            Debug.LogWarning($"Player {playerId} is already registered.");
        }
    }

    // ダメージを受け取る
    public void DealteDamage(float damage, int playerId)
    {
        if (damage < 0f)
        {
            Debug.LogError("Damage cannot be negative.");
            return;
        }

        if (!attackerResults.ContainsKey(playerId))
        {
            Debug.LogError($"Player with ID {playerId} not found.");
            return;
        }

        _totalDamage += damage;
        attackerResults[playerId] += damage;
    }

    // リセット関数（ダメージとプレイヤーをリセット）
    public void Reset()
    {
        _totalDamage = 0f;
        foreach (var playerId in attackerResults.Keys)
        {
            attackerResults[playerId] = 0f;
        }
    }

    // プレイヤーIDを指定してリセットする
    public void ResetPlayer(int playerId)
    {
        if (!attackerResults.ContainsKey(playerId))
        {
            Debug.LogError($"Player with ID {playerId} not found.");
            return;
        }

        attackerResults[playerId] = 0f;
    }
}
