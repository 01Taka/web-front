using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class RoleViewSet
{
    public PlayerRole role;
    public GameObject camera;
    public GameObject canvas;
}

public class CameraRig : MonoBehaviour
{
    [SerializeField] private List<RoleViewSet> roleViewSets;

    private Dictionary<PlayerRole, RoleViewSet> roleViewMap;

    private void Awake()
    {
        roleViewMap = new Dictionary<PlayerRole, RoleViewSet>();
        foreach (var set in roleViewSets)
        {
            if (!roleViewMap.ContainsKey(set.role))
            {
                roleViewMap.Add(set.role, set);
            }
            else
            {
                Debug.LogWarning($"Duplicate role found: {set.role}");
            }
        }
    }

    public void SetupRoleBasedView(PlayerRole role)
    {
        Debug.Log($"Set Role {role}");

        foreach (var set in roleViewSets)
        {
            bool isActive = set.role == role;
            if (set.canvas != null) set.canvas.SetActive(isActive);
            if (set.camera != null) set.camera.SetActive(isActive);
        }
    }

    public void OnSetRoleToWaitingButtonClicked()
    {
        SetupRoleBasedView(PlayerRole.Waiting);
    }
}
