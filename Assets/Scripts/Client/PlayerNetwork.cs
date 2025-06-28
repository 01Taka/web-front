//using Fusion;
//using UnityEngine;

//public class PlayerNetwork : NetworkBehaviour
//{
//    private StateAuthorityManager authorityManager;
//    private AttackManager attackManager;
//    private PlayerProperties properties;

//    public override void Spawned()
//    {
//        if (HasStateAuthority && !SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(Runner.LocalPlayer))
//        {
//            // StateAuthorityManager ‚ÌŽæ“¾‚ÆŽg—p
//            authorityManager = FindAnyObjectByType<StateAuthorityManager>();
//            if (authorityManager != null)
//            {
//                Object.ReleaseStateAuthority();
//                authorityManager.RequestStateAuthorityTransferToMaster(Object.Id);
//            }
//            else
//            {
//                Debug.LogError("[Player] StateAuthorityManager not found in the scene. Authority transfer will not be requested.");
//            }
//        }

//        if (HasInputAuthority)
//        {
//            if (TryGetComponent(out InputAttackHandler attackHandler))
//            {
//                attackHandler.Setup(this);
//            }
//            else
//            {
//                Debug.LogWarning("[Player] InputAttackHandler component not found on the player object.");
//            }
//        } else
//        {
//            gameObject.SetActive(false);
//        }

//        properties = GetComponent<PlayerProperties>();
//        if (properties == null)
//        {
//            Debug.LogError("[Player] PlayerProperties component missing.");
//        }

//        attackManager = FindAnyObjectByType<AttackManager>();
//        if (attackManager == null)
//        {
//            Debug.LogError("[Player] AttackManager not found in the scene. Attacks will not function correctly.");
//        }
//    }

//    public void SendAttack(AttackType type, Vector3 dir, float chargeAmount = 0f, int shotCount = 0)
//    {
//        if (!HasInputAuthority)
//        {
//            Debug.LogWarning("Attempted to send attack without input authority.");
//            return;
//        }

//        AttackDataNetwork data = new()
//        {
//            Level = properties.PlayerState.Level,
//            AttackPoint = 1,
//            Type = type,
//            Direction = dir,
//            ChargeAmount = chargeAmount,
//            ShotCount = shotCount
//        };

//        try
//        {
//            RPC_SendAttack(data);
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"SendAttack failed: {ex}");
//        }
//    }

//    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
//    private void RPC_SendAttack(AttackDataNetwork data, RpcInfo info = default)
//    {
//        if (!HasStateAuthority)
//        {
//            Debug.LogWarning("RPC_SendAttack called but this instance lacks StateAuthority.");
//            return;
//        }

//        if (attackManager == null)
//        {
//            Debug.LogError("Cannot handle attack: AttackManager is null.");
//            return;
//        }

//        Debug.Log($"Got Attack Request from {info.Source.PlayerId}");

//        AttackData attackData = new()
//        {
//            Level = data.Level,
//            AttackPoint = data.AttackPoint,
//            Type = data.Type,
//            Direction = data.Direction,
//            ChargeAmount = data.ChargeAmount,
//            ShotCount = data.ShotCount
//        };

//        try
//        {
//            attackManager.HandleAttack(attackData);
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"Failed to handle attack: {ex}");
//        }
//    }
//}
