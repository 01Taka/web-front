using UnityEngine;

public class SceneComponentManager : MonoBehaviour
{
    public static SceneComponentManager Instance { get; private set; }

    [SerializeField] private Camera gameCamera;
    [SerializeField] private AttackManager attackManager;
    [SerializeField] private AttackRecognizer attackRecognizer;
    [SerializeField] private AttackPointManager attackPointManager;
    [SerializeField] private CameraRig cameraRig;
    [SerializeField] private GameManager _gameManager;

    public Camera GameCamera => gameCamera;
    public AttackManager AttackManager => attackManager;
    public AttackRecognizer AttackRecognizer => attackRecognizer;
    public AttackPointManager AttackPointManager => attackPointManager;
    public CameraRig CameraRig => cameraRig;
    public GameManager GameManager => _gameManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (attackManager == null) Debug.LogWarning("AttackManager is not assigned in Inspector.");
        if (cameraRig == null) Debug.LogWarning("CameraRig is not assigned in Inspector.");
    }
}
