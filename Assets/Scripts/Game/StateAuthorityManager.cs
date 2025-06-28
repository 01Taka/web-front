using UnityEngine;
using Fusion;

public class StateAuthorityManager : NetworkBehaviour
{
    public void RequestStateAuthorityTransferToMaster(NetworkId netId)
    {
        if (!SharedModeMasterClientTracker.TryGetSharedModeMasterClientPlayerRef(out var masterRef))
        {
            Debug.LogWarning($"[AuthorityTransfer] Failed to transfer StateAuthority for {netId}: Master client is unavailable.");
            return;
        }

        RequestStateAuthorityTransfer(netId, masterRef);
    }

    public void RequestStateAuthorityTransfer(NetworkId netId, PlayerRef target)
    {
        if (!Runner.IsRunning)
        {
            Debug.LogWarning("RequestStateAuthorityTransfer failed: Runner is not active.");
            return;
        }

        if (target.IsNone)
        {
            Debug.LogWarning("RequestStateAuthorityTransfer failed: Target PlayerRef is None.");
            return;
        }

        RPC_HandleTransferRequest(netId, target);
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority)]
    private void RPC_HandleTransferRequest(NetworkId netId, PlayerRef target, RpcInfo info = default)
    {
        // Only the designated target should process this RPC
        if (Runner.LocalPlayer != target)
        {
            Debug.LogWarning($"RPC_HandleTransferRequest ignored: Not the intended target. (Expected: {target}, Local: {Runner.LocalPlayer})");
            return;
        }

        if (!Runner.TryFindObject(netId, out NetworkObject obj))
        {
            Debug.LogError($"RPC_HandleTransferRequest failed: NetworkObject with ID {netId} not found.");
            return;
        }

        Debug.Log($"Requesting StateAuthority for object {obj.name} ({netId}) by {Runner.LocalPlayer}.");

        obj.RequestStateAuthority();
    }
}
