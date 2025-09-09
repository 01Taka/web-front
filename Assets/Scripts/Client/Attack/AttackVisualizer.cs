using UnityEngine;

public class AttackVisualizer : MonoBehaviour
{
    private GameObject circleInstance;
    private GameObject pointerInstance;
    private Transform circleCenter;

    private AttackInputSettings settings;
    private Camera mainCam;

    /// <summary>
    /// Initializes the visualizer and spawns the UI elements.
    /// </summary>
    public void Initialize(AttackInputSettings settings, Camera mainCam)
    {
        this.settings = settings;
        this.mainCam = mainCam;
        SpawnPrefabs(settings.circlePrefab, settings.pointerPrefab);
    }

    /// <summary>
    /// Cleans up the visualizer elements when disabled.
    /// </summary>
    public void Cleanup()
    {
        if (circleInstance != null)
        {
            Destroy(circleInstance);
            circleInstance = null;
        }
        if (pointerInstance != null)
        {
            Destroy(pointerInstance);
            pointerInstance = null;
        }
    }

    /// <summary>
    /// Updates the pointer's world position.
    /// </summary>
    public void UpdatePointerPosition(Vector2 screenPosition)
    {
        if (pointerInstance != null)
        {
            Vector3 worldPos = GetPointerWorldPosition(screenPosition);
            pointerInstance.transform.position = worldPos;
        }
    }

    /// <summary>
    /// Sets the pointer's active state.
    /// </summary>
    public void SetPointerActive(bool isActive)
    {
        if (pointerInstance != null)
        {
            pointerInstance.SetActive(isActive);
        }
    }

    /// <summary>
    /// Gets the world position of the circle's center.
    /// </summary>
    public Vector3 GetCircleCenterWorldPosition()
    {
        return circleCenter != null ? circleCenter.position : Vector3.zero;
    }

    /// <summary>
    /// Gets the world position of the pointer.
    /// </summary>
    public Vector3 GetPointerWorldPosition()
    {
        return pointerInstance != null ? pointerInstance.transform.position : Vector3.zero;
    }

    public void SetCircleColor(int colorIndex)
    {
        if (circleInstance != null)
        {
            Color color = settings.playerColor.GetColor(colorIndex);
            circleInstance.transform.localScale = new Vector3(settings.radius * 2, settings.radius * 2, 1);
            color.a = settings.circleAlpha;
            circleInstance.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private void SpawnPrefabs(GameObject circlePrefab, GameObject pointerPrefab)
    {
        if (circlePrefab != null && mainCam != null)
        {
            Vector3 screenBottomCenter = new Vector3(Screen.width / 2f, 0, 0f);
            Vector3 worldBottomCenter = mainCam.ScreenToWorldPoint(screenBottomCenter);
            worldBottomCenter.z = 0f;

            circleInstance = Instantiate(circlePrefab, worldBottomCenter, Quaternion.identity);
            circleCenter = circleInstance.transform;
        }

        if (pointerPrefab != null)
        {
            pointerInstance = Instantiate(pointerPrefab);
            pointerInstance.SetActive(false);
        }
    }

    private Vector3 GetPointerWorldPosition(Vector2 screenPos)
    {
        float zDistance = Vector3.Distance(mainCam.transform.position, circleCenter.position);
        return mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
    }
}