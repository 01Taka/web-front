using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] private GameObject hostCamera;
    [SerializeField] private GameObject hostCanvas;

    [SerializeField] private GameObject clientCamera;
    [SerializeField] private GameObject clientCanvas;

    public void SetupRoleBasedView(PlayerRole role)
    {
        switch (role)
        {
            case PlayerRole.Host:
                ActivateHostView();
                break;
            case PlayerRole.Client:
                ActivateClientView();
                break;
        }
    }

    private void ActivateHostView()
    {
        hostCanvas.SetActive(true);
        hostCamera.SetActive(true);

        clientCanvas.SetActive(false);
        clientCamera.SetActive(false);
    }

    private void ActivateClientView()
    {
        clientCanvas.SetActive(true);
        clientCamera.SetActive(true);

        hostCanvas.SetActive(false);
        hostCamera.SetActive(false);
    }

}
