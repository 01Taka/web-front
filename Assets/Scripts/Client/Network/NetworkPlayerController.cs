using Fusion;
using UnityEngine;

[RequireComponent(typeof(PlayerProperties))]
public class NetworkPlayerController : NetworkBehaviour
{
    // C# class
    private PlayerAuthorityHandler authorityHandler;
    private PlayerCameraController cameraController;

    // MonoBehaviour
    private InputAttackHandler inputAttackHandler;
    private CameraRig cameraRig;

    // NetworkBehaviour
    private PlayerProperties playerProperties;
    private AttackRequestSender attackSender;

    public override void Spawned()
    {
        authorityHandler = new PlayerAuthorityHandler(this);
        authorityHandler.Initialize();

        playerProperties = GetComponent<PlayerProperties>();
        if (playerProperties == null)
        {
            Debug.LogError("[NetworkPlayerController] PlayerProperties component missing.");
        }

        if (HasInputAuthority)
        {
            cameraRig = SceneComponentManager.Instance.CameraRig;
            cameraController = new PlayerCameraController(cameraRig, Properties);

            int attackPoint = 1;
            attackSender = GetComponent<AttackRequestSender>();
            attackSender.Setup(Properties.PlayerState.Level, attackPoint);

            inputAttackHandler = GetComponent<InputAttackHandler>();
            if (inputAttackHandler != null)
            {
                inputAttackHandler.enabled = true;
                inputAttackHandler.Setup(
                    attackSender,
                    SceneComponentManager.Instance.AttackRecognizer,
                    SceneComponentManager.Instance.GameCamera,
                    DirectionExtensions.ToVector2(playerProperties.PlayerState.UpDirection)
                );
            }
            else
            {
                Debug.LogWarning("[NetworkPlayerController] InputAttackHandler component missing.");
            }
        }
    }

    private void OnDisable()
    {
        cameraController?.Dispose();
    }

    public PlayerProperties Properties => playerProperties;
}
