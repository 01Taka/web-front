//using System.Collections.Generic;
//using UnityEngine;

//public class AttackPortManager : MonoBehaviour
//{
//    [SerializeField] private List<BossFiringPortController> allPorts;
//    private MultiPortAttackController multiPortController;
//    private List<BossFiringPortController> activePorts = new List<BossFiringPortController>();

//    private void Update()
//    {
//        if (multiPortController != null)
//        {
//            multiPortController.Tick(Time.deltaTime);
//        }
//    }

//    public void SelectPortsAndPrepare(AttackPattern attackPattern)
//    {
//        if (IsAnyPortOccupied() && attackPattern.occupiedBehavior == PortOccupiedBehavior.WaitUntilFree)
//        {
//            Debug.Log("ポートが使用中のため、攻撃を待機します。");
//            return;
//        }

//        ResetAllPorts();

//        if (attackPattern.firingPortType == FiringPortType.All)
//        {
//            PrepareAllPorts(attackPattern);
//        }
//        else
//        {
//            PrepareSinglePort(attackPattern);
//        }
//    }

//    private void PrepareSinglePort(AttackPattern pattern)
//    {
//        var availablePorts = GetAvailablePorts();
//        ShuffleList(availablePorts);

//        if (availablePorts.Count == 0)
//        {
//            Debug.LogWarning("利用可能なポートがありません。");
//            return;
//        }

//        var port = availablePorts[0];
//        port.StartPreparation(pattern, (float)pattern.cancelDamageThreshold);
//        activePorts.Add(port);
//        SubscribeToPortEvents(port);
//        Debug.Log($"{port.name} が攻撃準備を開始しました。");
//    }

//    private void PrepareAllPorts(AttackPattern pattern)
//    {
//        // 全体攻撃コントローラーを初期化
//        multiPortController = new MultiPortAttackController(
//            allPorts,
//            pattern.attackPreparationTime,
//            (float)pattern.cancelDamageThreshold,
//            pattern.damageManagementType 
//        );

//        multiPortController.OnAttackCompleted += OnAllPortsAttackCompleted;
//        multiPortController.OnAttackCancelled += OnAllPortsAttackCancelled;

//        foreach (var port in allPorts)
//        {
//            // 個別のダメージ管理はMultiPortAttackControllerに委譲
//            port.StartPreparation(pattern, (float)pattern.cancelDamageThreshold);
//        }

//        Debug.Log("すべてのポートが全体攻撃の準備を開始しました。");
//    }

//    private void OnAllPortsAttackCompleted(List<BossFiringPortController> remainingPorts)
//    {
//        Debug.Log($"全体攻撃が完了しました。生き残ったポート数: {remainingPorts.Count}");
//        multiPortController = null;
//    }

//    private void OnAllPortsAttackCancelled()
//    {
//        Debug.Log("全体攻撃がキャンセルされました。");
//        multiPortController = null;
//    }

//    private bool IsAnyPortOccupied()
//    {
//        foreach (var port in allPorts)
//        {
//            if (port.IsOccupied)
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    private void ResetAllPorts()
//    {
//        if (multiPortController != null)
//        {
//            multiPortController.OnAttackCompleted -= OnAllPortsAttackCompleted;
//            multiPortController.OnAttackCancelled -= OnAllPortsAttackCancelled;
//            multiPortController = null;
//        }

//        foreach (var port in activePorts)
//        {
//            port.CancelPreparation();
//        }
//        activePorts.Clear();
//    }

//    private List<BossFiringPortController> GetAvailablePorts()
//    {
//        var result = new List<BossFiringPortController>();
//        foreach (var port in allPorts)
//        {
//            if (!port.IsOccupied)
//            {
//                result.Add(port);
//            }
//        }
//        return result;
//    }

//    private void SubscribeToPortEvents(BossFiringPortController port)
//    {
//        port.OnPreparationCancelled.AddListener(() => OnPortCancelled(port));
//        port.OnAttackLaunched.AddListener(() => OnPortLaunched(port));
//    }

//    private void OnPortCancelled(BossFiringPortController port)
//    {
//        if (activePorts.Contains(port))
//        {
//            activePorts.Remove(port);
//        }
//    }

//    private void OnPortLaunched(BossFiringPortController port)
//    {
//        if (activePorts.Contains(port))
//        {
//            activePorts.Remove(port);
//        }
//    }

//    private void ShuffleList<T>(List<T> list)
//    {
//        for (int i = 0; i < list.Count; i++)
//        {
//            T temp = list[i];
//            int randomIndex = Random.Range(i, list.Count);
//            list[i] = list[randomIndex];
//            list[randomIndex] = temp;
//        }
//    }
//}