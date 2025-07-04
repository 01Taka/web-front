using Fusion;
using UnityEngine;
using System;

public class SharedModeMasterClientTracker : NetworkBehaviour
{
    private static SharedModeMasterClientTracker LocalInstance;

    /// <summary>
    /// トラッカーのスポーン時に通知されるイベント
    /// </summary>
    public static event Action<SharedModeMasterClientTracker> OnTrackerSpawned;

    public override void Spawned()
    {
        LocalInstance = this;
        Debug.Log("SharedModeMasterClientTracker Active");

        try
        {
            OnTrackerSpawned?.Invoke(this);
        }
        catch (Exception ex)
        {
            Debug.LogError($"OnTrackerSpawned callback threw an exception: {ex}");
        }
    }

    private void OnDestroy()
    {
        if (LocalInstance == this)
        {
            LocalInstance = null;
        }
        else
        {
            Debug.LogWarning("Another instance tried to unset LocalInstance. Possible race condition?");
        }
    }

    public static bool IsPlayerSharedModeMasterClient(PlayerRef player)
    {
        if (LocalInstance == null)
        {
            Debug.LogWarning("IsPlayerSharedModeMasterClient called but LocalInstance is null.");
            return false;
        }

        if (player.IsNone)
        {
            Debug.LogWarning("IsPlayerSharedModeMasterClient called with None PlayerRef.");
            return false;
        }

        return LocalInstance.Object.StateAuthority == player;
    }

    public static PlayerRef? GetSharedModeMasterClientPlayerRef()
    {
        if (LocalInstance == null)
        {
            Debug.LogWarning("GetSharedModeMasterClientPlayerRef called but LocalInstance is null.");
            return null;
        }

        return LocalInstance.Object.StateAuthority;
    }

    public static bool TryGetSharedModeMasterClientPlayerRef(out PlayerRef player)
    {
        player = default;

        if (LocalInstance == null)
        {
            Debug.LogWarning("GetSharedModeMasterClientPlayerRef called but LocalInstance is null.");
            return false;
        }

        player = LocalInstance.Object.StateAuthority;
        return true;
    }
}
