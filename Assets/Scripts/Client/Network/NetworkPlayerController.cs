using Fusion;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    // MonoBehaviour
    private InputAttackHandler _inputAttackHandler;
    // NetworkBehaviour
    private AttackRequestSender _attackSender;

    private int _playerLevel = 1;
    private Direction _upDirection = Direction.Up;

    public override void Spawned()
    {
        DeviceStateManager deviceStateManager = FindAnyObjectByType<DeviceStateManager>();

        if (deviceStateManager == null)
        {
            Debug.LogError("DeviceStateManager Not Found.");
            return;
        }

        bool isMasterClient = Runner.IsSharedModeMasterClient;
        deviceStateManager.SetDeviceState(isMasterClient ? DeviceState.Host : DeviceState.Client);

        TransferStateAuthority();

        if (HasInputAuthority)
        {
            SetupAttackSystem();
        }
    }

    private void TransferStateAuthority()
    {
        if (HasStateAuthority &&
            !SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(Runner.LocalPlayer))
        {
            StateAuthorityManager manager = GlobalRegistry.Instance.GetStateAuthorityManager();
            if (manager != null)
            {
                Object.ReleaseStateAuthority();
                manager.RequestStateAuthorityTransferToMaster(Object.Id);
            }
            else
            {
                Debug.LogError("StateAuthorityManager not found.");
            }
        }
    }

    private void SetupAttackSystem()
    {
        _attackSender = GetComponent<AttackRequestSender>();
        _inputAttackHandler = GetComponent<InputAttackHandler>();

        if (_attackSender == null)
        {
            Debug.LogError("AttackRequestSender component missing.");
            return;
        }

        if (_inputAttackHandler == null)
        {
            Debug.LogError("InputAttackHandler component missing.");
            return;
        }

        _attackSender.Setup(_playerLevel);

        _inputAttackHandler.Setup(
            _attackSender,
            SceneComponentManager.Instance.AttackRecognizer,
            SceneComponentManager.Instance.GameCamera,
            DirectionExtensions.ToVector2(_upDirection)
        );
    }
}
