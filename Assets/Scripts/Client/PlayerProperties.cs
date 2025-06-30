using Fusion;
using UnityEngine;
using System;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnRoleChanged))]
    public PlayerRole Role { get; set; }

    [Networked, OnChangedRender(nameof(OnPlayerStateChanged))]
    public NetworkPlayerState PlayerState { get; set; }

    // === イベント ===
    public event Action<NetworkObject, PlayerRole> OnRoleChangedEvent;
    public event Action<NetworkObject, NetworkPlayerState> OnPlayerStateChangedEvent;

    public override void Spawned()
    {
        if (!Object.HasStateAuthority) return;

        try
        {
            Role = DetermineInitialRole();

            if (Role == PlayerRole.Waiting)
                InitializeClientState();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PlayerProperties] Spawn initialization failed: {ex.Message}");
        }
    }

    private PlayerRole DetermineInitialRole()
    {
        if (Runner == null || Runner.LocalPlayer == null)
        {
            Debug.LogWarning("[PlayerProperties] Runner or LocalPlayer is null. Defaulting to Client.");
            return PlayerRole.Waiting;
        }

        return SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(Runner.LocalPlayer)
            ? PlayerRole.Host
            : PlayerRole.Client;
    }

    private void InitializeClientState()
    {
        PlayerState = new NetworkPlayerState
        {
            Level = 1,
            UpDirection = Direction.Up
        };
    }

    // === OnChangedRender コールバック ===
    private void OnRoleChanged()
    {
        Debug.Log("[PlayerProperties] Role changed (OnChangedRender).");
        OnRoleChangedEvent?.Invoke(Object, Role);
    }

    private void OnPlayerStateChanged()
    {
        Debug.Log("[PlayerProperties] PlayerState changed (OnChangedRender).");
        OnPlayerStateChangedEvent?.Invoke(Object, PlayerState);
    }

    public void UpdatePlayerState(int level, Direction direction)
    {
        if (Role != PlayerRole.Client)
        {
            Debug.LogWarning("[PlayerProperties] Cannot update PlayerState: Role is not Client.");
            return;
        }

        if (!Object.HasStateAuthority)
        {
            Debug.LogWarning("[PlayerProperties] Cannot update PlayerState: No state authority.");
            return;
        }

        PlayerState = new NetworkPlayerState
        {
            Level = level,
            UpDirection = direction
        };
    }
}
