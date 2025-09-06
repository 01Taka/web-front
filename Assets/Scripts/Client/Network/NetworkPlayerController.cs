using Fusion;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnDeviceStateChanged))]
    public DeviceState PlayerDeviceState { get; set; }

    // MonoBehaviour
    private InputAttackHandler _inputAttackHandler;
    // NetworkBehaviour
    private AttackRequestSender _attackSender;

    private int _playerLevel = 1;
    private Direction _upDirection = Direction.Up;

    public override void Spawned()
    {
        bool isLocalMasterClient = SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(Runner.LocalPlayer);
        Debug.Log($"Local: {Runner.LocalPlayer}, State: {Object.StateAuthority}, Input: {Object.InputAuthority}, IsMasterClient: {isLocalMasterClient}", this);

        if (Object.InputAuthority == null || Object.InputAuthority.IsNone)
        {
            Debug.LogError("InputAuthority is none");
            return;
        }

        if (HasStateAuthority)
        {
            if (!isLocalMasterClient)
            {
                TransferStateAuthority();
            }

            DeviceState deviceState = SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(Object.InputAuthority)
                    ? DeviceState.Host
                    : DeviceState.Client;

            PlayerDeviceState = deviceState;
        }

        if (HasInputAuthority)
        {
            SetupAttackSystem();
        }
    }

    private void OnDeviceStateChanged()
    {
        if (HasInputAuthority)
        {
            Debug.Log($"DeviceState of {Runner.LocalPlayer} changed to {PlayerDeviceState}");
            DeviceStateManager.Instance.SetDeviceState(PlayerDeviceState);
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
