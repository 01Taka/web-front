using UnityEngine;

public class CommandPanelManager : MonoBehaviour
{
    public void OnActiveButtonClicked()
    {
        gameObject.SetActive(true);
    }

    public void OnInactiveButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
