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
    [SerializeField] private List<DeviceStateViewSet> _deviceStateViewSets;
    [SerializeField] private GameObject[] _hostScriptsParents;
    [SerializeField] private DeviceState _currentDeviceState = DeviceState.None;

    public DeviceState CurrentDeviceState => _currentDeviceState;

    private void Start()
    {
        SetDeviceState(DeviceState.Offline);
    }

    public void SetDeviceState(DeviceState state, bool isSettingHostScriptsActive = false)
    {
        Debug.Log($"Device State Changed to {state}");
        _currentDeviceState= state;
        if (isSettingHostScriptsActive)
        {
            foreach (var obj in _hostScriptsParents)
            {
                obj.SetActive(state == DeviceState.Host);
            }
        }

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