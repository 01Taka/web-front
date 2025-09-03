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

    public NetworkPlayerManager GetNetworkPlayerManager()
    {
        if (_networkPlayerManager == null)
        {
            _networkPlayerManager = FindFirstObjectByType<NetworkPlayerManager>();
        }
        return _networkPlayerManager;
    }
}
