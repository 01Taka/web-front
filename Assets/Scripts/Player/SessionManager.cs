using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public void DisconnectFromSession()
    {
        NetworkPlayerManager playerManager = GlobalRegistry.Instance.GetNetworkPlayerManager();
        if (playerManager != null)
        {
            playerManager.DisconnectFromSession();
        }
    }
}
