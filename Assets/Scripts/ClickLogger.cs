using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickLoggerRPC : NetworkBehaviour
{
    // �N���C�A���g �� �z�X�g
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_LogRequest(string senderName)
    {
        string message = $"{senderName} clicked at {Time.time:F2}";

        // �S�N���C�A���g�փu���[�h�L���X�g
        RPC_DisplayLog(message);
    }

    // �z�X�g �� �S�N���C�A���g
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DisplayLog(string message)
    {
        Debug.Log($"[All Clients] {message}");
    }
}
