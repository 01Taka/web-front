using Fusion;
using UnityEngine;

public class AttackRequestSender: NetworkBehaviour, IAttackSender
{
    private int level;
    private int attackPoint;

    public void Setup(int level, int attackPoint)
    {
        this.level = level;
        this.attackPoint = attackPoint;
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
            AttackPoint = attackPoint,
            Type = inputData.Type,
            Direction = inputData.Direction,
            ChargeAmount = inputData.ChargeAmount,
            ShotCount = inputData.ShotCount
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

        Debug.Log($"Received Attack Request from {info.Source.PlayerId}");

        var attackData = new AttackData
        {
            Level = data.Level,
            AttackPoint = data.AttackPoint,
            Type = data.Type,
            Direction = data.Direction,
            ChargeAmount = data.ChargeAmount,
            ShotCount = data.ShotCount
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