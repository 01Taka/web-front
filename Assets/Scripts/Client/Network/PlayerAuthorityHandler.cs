using UnityEngine;

public class PlayerAuthorityHandler
{
    private readonly NetworkPlayerController controller;

    public PlayerAuthorityHandler(NetworkPlayerController controller)
    {
        this.controller = controller;
    }

    public void Initialize()
    {
        if (controller.HasStateAuthority &&
            !SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(controller.Runner.LocalPlayer))
        {
            var manager = Object.FindAnyObjectByType<StateAuthorityManager>();
            if (manager != null)
            {
                controller.Object.ReleaseStateAuthority();
                manager.RequestStateAuthorityTransferToMaster(controller.Object.Id);
            }
            else
            {
                Debug.LogError("[AuthorityHandler] StateAuthorityManager not found.");
            }
        }
    }
}
