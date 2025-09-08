using Fusion;
using UnityEngine;

public class NetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private AttackInputSettings _attackInputSettings;
    private int _playerLevel = 1;

    private AttackRequestSender _attackSender;

    public override void Spawned()
    {
        Debug.Log($"---> Spawned player: {Runner.LocalPlayer}, StateAuthority: {Object.StateAuthority}, InputAuthority: {Object.InputAuthority}", this);

        bool isLocalMasterClient = SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(Runner.LocalPlayer);

        // ��Ԍ����̈ڍs�����s
        if (HasStateAuthority && !isLocalMasterClient)
        {
            TransferStateAuthority();
        }

        // ���͌��������v���C���[�݂̂����[�J���ŃA�^�b�N�V�X�e�����Z�b�g�A�b�v
        if (HasInputAuthority)
        {
            SetupAttackSystem();
        }

        Debug.Log($"<--- Spawned player initialization complete: {Runner.LocalPlayer}");
    }

    private void TransferStateAuthority()
    {
        // GlobalRegistry�̃C���X�^���X���L�����`�F�b�N
        if (GlobalRegistry.Instance == null)
        {
            Debug.LogError("GlobalRegistry instance is not available. Cannot transfer state authority.", this);
            return;
        }

        StateAuthorityManager manager = GlobalRegistry.Instance.GetStateAuthorityManager();
        if (manager == null)
        {
            Debug.LogError("StateAuthorityManager not found in GlobalRegistry. Cannot transfer state authority.", this);
            return;
        }

        // �����̕����ƈڍs���N�G�X�g
        Object.ReleaseStateAuthority();
        manager.RequestStateAuthorityTransferToMaster(Object.Id);
        Debug.Log($"State authority for NetworkObject {Object.Id} has been released and transfer requested to Master Client.", this);
    }

    private void SetupAttackSystem()
    {
        // �K�{�R���|�[�l���g�̑��݃`�F�b�N
        _attackSender = GetComponent<AttackRequestSender>();
        if (_attackSender == null)
        {
            Debug.LogError("AttackRequestSender component missing on this GameObject. Attack system cannot be set up.", this);
            return;
        }

        // SceneComponentManager�̃C���X�^���X�`�F�b�N
        if (SceneComponentManager.Instance == null)
        {
            Debug.LogError("SceneComponentManager instance is not available. Attack system setup aborted.", this);
            return;
        }

        // �U���n���h���[�̃Z�b�g�A�b�v
        AttackInputHandler attackHandler = gameObject.AddComponent<AttackInputHandler>();
        if (attackHandler == null)
        {
            Debug.LogError("Failed to add AttackInputHandler component.", this);
            return;
        }

        // �^�b�`�n���h���[�̃Z�b�g�A�b�v
        BetterTouchHandler touchHandler = gameObject.AddComponent<BetterTouchHandler>();
        if (touchHandler == null)
        {
            Debug.LogError("Failed to add BetterTouchHandler component.", this);
            return;
        }

        if (!SceneComponentManager.Instance.ClientCamera)
        {
            Debug.LogError("Client Camera Not Found");
            return;
        }

        _attackSender.Setup(_playerLevel);

        InputActions controls = new InputActions();
        attackHandler.Initialize(
            _attackInputSettings,
            _attackSender,
            controls.Player.Press,
            controls.Player.Hold,
            SceneComponentManager.Instance.ClientCamera
        );

        Debug.Log("Attack system successfully set up for the local player.", this);
    }
}