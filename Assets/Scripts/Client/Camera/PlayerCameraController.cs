using Fusion;
using UnityEngine;

public class PlayerCameraController
{
    private CameraRig cameraRig;
    private PlayerProperties properties;

    public PlayerCameraController(CameraRig cameraRig, PlayerProperties properties)
    {
        this.cameraRig = cameraRig;
        this.properties = properties;

        if (cameraRig == null || properties == null)
        {
            Debug.LogError($"[PlayerCameraController] Initialization failed: " +
                $"cameraRig is {(cameraRig == null ? "null" : "ok")}, " +
                $"properties is {(properties == null ? "null" : "ok")}.");
            return;
        }

        cameraRig.SetupRoleBasedView(properties.Role);
        properties.OnRoleChangedEvent += HandleRoleChanged;
    }

    public void Dispose()
    {
        if (properties != null)
        {
            properties.OnRoleChangedEvent -= HandleRoleChanged;
        }
    }

    private void HandleRoleChanged(NetworkObject networkObject, PlayerRole role)
    {
        if (networkObject.HasStateAuthority)
        {
            cameraRig.SetupRoleBasedView(role);
        }
    }
}
