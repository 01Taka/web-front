using System.Collections.Generic;
using UnityEngine;

public class DamageDealtManager
{
    private float _totalDamage = 0f;
    public Dictionary<int, float> attackerResults = new Dictionary<int, float>();

    // ���_���[�W�̎擾
    public float TotalDealtDamage => _totalDamage;

    // �v���C���[��ǉ�����i�����l0�Łj
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

    // �_���[�W���󂯎��
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

    // ���Z�b�g�֐��i�_���[�W�ƃv���C���[�����Z�b�g�j
    public void Reset()
    {
        _totalDamage = 0f;
        foreach (var playerId in attackerResults.Keys)
        {
            attackerResults[playerId] = 0f;
        }
    }

    // �v���C���[ID���w�肵�ă��Z�b�g����
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
