using UnityEngine;

public class GlobalRegistry : MonoBehaviour
{
    public static GlobalRegistry Instance { get; private set; }
    private NetworkPlayerManager _networkPlayerManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool CheckIsOnline()
    {
        NetworkPlayerManager manager = GetNetworkPlayerManager();
        return manager != null && manager.IsOnline;
    }

    public bool CheckIsMasterClient()
    {
        NetworkPlayerManager manager = GetNetworkPlayerManager();
        return manager != null && manager.IsMasterClient;
    }

    public NetworkPlayerManager GetNetworkPlayerManager()
    {
        if (_networkPlayerManager == null)
        {
            _networkPlayerManager = FindFirstObjectByType<NetworkPlayerManager>();
        }
        return _networkPlayerManager;
    }
}
