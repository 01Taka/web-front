using UnityEngine;

public class GlobalRegistry : MonoBehaviour
{
    public static GlobalRegistry Instance { get; private set; }
    private NetworkPlayerManager _networkPlayerManager;
    private StateAuthorityManager _stateAuthorityManager;
    private AttackVisualizer _attackVisualizer;

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

    public bool TryGetNetworkPlayerManager(out NetworkPlayerManager networkPlayerManager)
    {
        if (_networkPlayerManager == null)
        {
            _networkPlayerManager = FindFirstObjectByType<NetworkPlayerManager>();
            if (!_networkPlayerManager)
            {
                networkPlayerManager = null;
                Debug.LogError("Not found NetworkPlayerManager");
                return false;
            }
        }
        networkPlayerManager = _networkPlayerManager;
        return true;
    }

    public bool TryGetAttackVisualizer(out AttackVisualizer attackVisualizer)
    {
        if (_attackVisualizer == null)
        {
            _attackVisualizer = FindFirstObjectByType<AttackVisualizer>();
            if (!_attackVisualizer)
            {
                attackVisualizer = null;
                Debug.LogError("Not found AttackVisualizer");
                return false;
            }
        }
        attackVisualizer = _attackVisualizer;
        return true;
    }

    public StateAuthorityManager GetStateAuthorityManager()
    {
        if (_stateAuthorityManager == null)
        {
            _stateAuthorityManager = FindFirstObjectByType<StateAuthorityManager>();
        }
        return _stateAuthorityManager;
    }
}
