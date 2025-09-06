using Fusion;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DeviceStateViewSet
{
    public DeviceState State;
    public GameObject Camera;
    public GameObject Canvas;
}

public class DeviceStateManager : MonoBehaviour
{
    // Singletonインスタンスを格納する静的変数
    public static DeviceStateManager Instance { get; private set; }

    [SerializeField] private List<DeviceStateViewSet> _deviceStateViewSets;

    // Awakeメソッドでインスタンスを初期化する
    private void Awake()
    {
        // インスタンスが既に存在する場合は、このオブジェクトを破棄する
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // このオブジェクトをSingletonインスタンスとして設定する
        Instance = this;
        // シーンを切り替えてもこのオブジェクトが破棄されないようにする
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetDeviceState(DeviceState state)
    {
        foreach (var set in _deviceStateViewSets)
        {
            bool isActive = set.State == state;
            if (set.Canvas != null) set.Canvas.SetActive(isActive);
            if (set.Camera != null) set.Camera.SetActive(isActive);
        }
    }

    public DeviceState GetDeviceStateByPlayerRef(PlayerRef player)
    {
        if (player == null || player.IsNone)
        {
            Debug.LogError("LocalPlayer is null. Defaulting to Client.");
            return DeviceState.Waiting;
        }

        if (SharedModeMasterClientTracker.MasterClientPlayerRef == null || SharedModeMasterClientTracker.MasterClientPlayerRef.IsNone)
        {
            Debug.LogError("SharedModeMasterClientTracker was not initialized");
            return DeviceState.Waiting;
        }

        return SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(player)
            ? DeviceState.Host
            : DeviceState.Client;
    }

    public void SetDeviceStateByPlayerRef(PlayerRef playerRef)
    {
        DeviceState deviceState = GetDeviceStateByPlayerRef(playerRef);
        SetDeviceState(deviceState);
        Debug.Log(deviceState.ToString());
    }

    public void OnSetDeviceStateToWaiting()
    {
        SetDeviceState(DeviceState.Waiting);
    }
}