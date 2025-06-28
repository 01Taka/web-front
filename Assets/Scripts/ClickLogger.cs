using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickLoggerRPC : NetworkBehaviour
{
    // クライアント → ホスト
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_LogRequest(string senderName)
    {
        string message = $"{senderName} clicked at {Time.time:F2}";

        // 全クライアントへブロードキャスト
        RPC_DisplayLog(message);
    }

    // ホスト → 全クライアント
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DisplayLog(string message)
    {
        Debug.Log($"[All Clients] {message}");
    }
}
