using Fusion;
using UnityEngine;

public class AttackRequestSender: NetworkBehaviour, IAttackSender
{
    private int level;

    public void Setup(int level)
    {
        this.level = level;
    }

    public void SendAttack(AttackInputData inputData)
    {
        if (!HasInputAuthority)
        {
            Debug.LogWarning("[AttackRequestSender] No input authority to send attack.");
            return;
        }

        var data = new AttackDataNetwork
        {
            Level = level,
            Type = inputData.Type,
            Direction = inputData.Direction,
            ChargeAmount = inputData.ChargeAmount,
        };

        RPC_HandleAttackRequest(data);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_HandleAttackRequest(AttackDataNetwork data, RpcInfo info = default)
    {
        if (!HasStateAuthority)
        {
            Debug.LogWarning("[NetworkPlayerController] RPC_HandleAttackRequest called without StateAuthority.");
            return;
        }

        var attackManager = SceneComponentManager.Instance.AttackManager;
        if (attackManager == null)
        {
            Debug.LogError("[NetworkPlayerController] AttackManager not found in scene.");
            return;
        }

        if (!GlobalRegistry.Instance.TryGetNetworkPlayerManager(out var networkPlayerManager))
        {
            Debug.LogError("[NetworkPlayerController] NetworkPlayerManager not found in GlobalRegistry.");
            return;
        }

        if (!networkPlayerManager.TryGetCompactedIndex(info.Source, out int index))
        {
            Debug.LogError($"[NetworkPlayerController] Could not get compacted index for source: {info.Source}");
            return;
        }

        var attackData = new AttackData
        {
            AttackerRef = info.Source,
            AttackerIndex = index,
            Type = data.Type,
            Direction = data.Direction,
            ChargeAmount = data.ChargeAmount,
        };

        try
        {
            attackManager.HandleAttack(attackData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NetworkPlayerController] Failed to handle attack: {ex}");
        }
    }
}