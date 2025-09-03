// ファイル名: MultiPortAttackController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiPortAttackController
{
    public event Action<List<BossFiringPortController>> OnAttackCompleted;
    public event Action OnAttackCancelled;

    private PortDamageManagementType damageType;
    private AttackTimer overallTimer;
    private DamageThresholdHandler overallDamageHandler;
    private List<BossFiringPortController> managedPorts;
    private bool isAttackActive = false;

    public MultiPortAttackController(List<BossFiringPortController> ports, float attackDuration, float cancelThreshold, PortDamageManagementType type)
    {
        managedPorts = ports;
        damageType = type;

        overallTimer = new AttackTimer();
        overallTimer.StartTimer(attackDuration);
        overallTimer.OnTimerComplete += HandleAttackComplete;

        if (damageType == PortDamageManagementType.Shared)
        {
            overallDamageHandler = new DamageThresholdHandler();
            overallDamageHandler.SetThreshold(cancelThreshold);
            overallDamageHandler.OnThresholdReached += HandleAttackCancelled;

            foreach (var port in managedPorts)
            {
                port.OnDamageTaken.AddListener(overallDamageHandler.AddDamage);
            }
        }
        else // Individual
        {
            foreach (var port in managedPorts)
            {
                port.SetDamageThreshold(cancelThreshold);
                port.OnPreparationCancelled.AddListener(HandlePortCancelled);
            }
        }

        isAttackActive = true;
    }

    // MonoBehaviourのUpdate()から呼び出す更新メソッド
    public void Tick(float deltaTime)
    {
        if (isAttackActive)
        {
            overallTimer.Tick(deltaTime);
        }
    }

    private void HandleAttackComplete()
    {
        if (isAttackActive)
        {
            isAttackActive = false;
            var remainingPorts = managedPorts.Where(p => p.IsPreparing).ToList();
            OnAttackCompleted?.Invoke(remainingPorts);
            CleanUp();
        }
    }

    private void HandleAttackCancelled()
    {
        if (isAttackActive)
        {
            isAttackActive = false;
            OnAttackCancelled?.Invoke();
            CleanUp();
        }
    }

    private void HandlePortCancelled()
    {
        if (damageType == PortDamageManagementType.Individual)
        {
            if (managedPorts.All(p => !p.IsPreparing))
            {
                HandleAttackCancelled();
            }
        }
    }

    private void CleanUp()
    {
        overallTimer.OnTimerComplete -= HandleAttackComplete;

        if (damageType == PortDamageManagementType.Shared)
        {
            overallDamageHandler.OnThresholdReached -= HandleAttackCancelled;
            foreach (var port in managedPorts)
            {
                port.OnDamageTaken.RemoveListener(overallDamageHandler.AddDamage);
            }
        }
        else // Individual
        {
            foreach (var port in managedPorts)
            {
                port.OnPreparationCancelled.RemoveListener(HandlePortCancelled);
            }
        }

        foreach (var port in managedPorts)
        {
            port.CancelPreparation();
        }
    }
}