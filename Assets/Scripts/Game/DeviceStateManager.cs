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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetDeviceState(DeviceState.Offline);
    }

    public void SetDeviceState(DeviceState state)
    {
        Debug.Log($"Device State Changed to {state}");
        foreach (var set in _deviceStateViewSets)
        {
            bool isActive = set.State == state;
            if (set.Canvas != null) set.Canvas.SetActive(isActive);
            if (set.Camera != null) set.Camera.SetActive(isActive);
        }
    }

    public void OnSetDeviceStateToOffline()
    {
        SetDeviceState(DeviceState.Offline);
    }
}