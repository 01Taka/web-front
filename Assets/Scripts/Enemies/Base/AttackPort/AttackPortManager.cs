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
//            Debug.Log("�|�[�g���g�p���̂��߁A�U����ҋ@���܂��B");
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
//            Debug.LogWarning("���p�\�ȃ|�[�g������܂���B");
//            return;
//        }

//        var port = availablePorts[0];
//        port.StartPreparation(pattern, (float)pattern.cancelDamageThreshold);
//        activePorts.Add(port);
//        SubscribeToPortEvents(port);
//        Debug.Log($"{port.name} ���U���������J�n���܂����B");
//    }

//    private void PrepareAllPorts(AttackPattern pattern)
//    {
//        // �S�̍U���R���g���[���[��������
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
//            // �ʂ̃_���[�W�Ǘ���MultiPortAttackController�ɈϏ�
//            port.StartPreparation(pattern, (float)pattern.cancelDamageThreshold);
//        }

//        Debug.Log("���ׂẴ|�[�g���S�̍U���̏������J�n���܂����B");
//    }

//    private void OnAllPortsAttackCompleted(List<BossFiringPortController> remainingPorts)
//    {
//        Debug.Log($"�S�̍U�����������܂����B�����c�����|�[�g��: {remainingPorts.Count}");
//        multiPortController = null;
//    }

//    private void OnAllPortsAttackCancelled()
//    {
//        Debug.Log("�S�̍U�����L�����Z������܂����B");
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