using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using System;

public class BossAttackPortsManager: IBossAttacdkLogicCallbasks
{
    private Dictionary<BossFiringPort, TimerUtils> _portTimers = new Dictionary<BossFiringPort, TimerUtils>();
    private Dictionary<BossFiringPort, BossAttackPort> _allPorts;
    private IBossAttackLogic _bossAttackLogic;

    private Dictionary<BossFiringPort, AttackPattern> _activeAttackPatterns = new Dictionary<BossFiringPort, AttackPattern>();
    private AttackPattern _activeUltimatePattern;

    private const string ULTIMATE_ATTACK_TIMER = "UltimateAttackTimer";

    public BossAttackPortsManager(MonoBehaviour coroutineRunner, Dictionary<BossFiringPort, BossAttackPort> ports, IBossAttackLogic bossAttackLogic)
    {
        _allPorts = ports ?? throw new ArgumentNullException(nameof(ports));
        _bossAttackLogic = bossAttackLogic ?? throw new ArgumentNullException(nameof(bossAttackLogic));

        foreach (var port in _allPorts)
        {
            _portTimers[port.Key] = new TimerUtils(coroutineRunner);
        }
    }

    public List<BossAttackPort> GetAvailablePorts()
    {
        List<BossAttackPort> availablePorts = new List<BossAttackPort>();

        foreach (var port in _allPorts)
        {
            if (port.Value.IsAvailable)
            {
                availablePorts.Add(port.Value);
            }
        }

        return availablePorts;
    }

    public void StartAttack(BossFiringPort portType, AttackPattern pattern)
    {
        if (!IsPortAvailable(portType)) return;

        var port = _allPorts[portType];
        port.SetAvailability(false);
        MyLogger.Log($"ポート ({portType}) から {pattern.AttackName} 攻撃準備中...");

        _bossAttackLogic.OnBeginPreparation(pattern, port);
        _activeAttackPatterns[portType] = pattern;

        StartPortTimer(portType, pattern, () => OnAttackTimerComplete(port, pattern));
    }

    public void StartUltimateAttack(AttackPattern pattern)
    {
        CancelAllAttacks();

        MyLogger.Log($"すべてのポートを使った大技の準備開始...");
        _bossAttackLogic.OnBeginMultiPortAttack(pattern, GetAllPorts());
        _activeUltimatePattern = pattern;

        StartPortTimer(ULTIMATE_ATTACK_TIMER, pattern, () => OnUltimateAttackTimerComplete(pattern));
    }


    public void ForceExecuteAttack(BossFiringPort portType)
    {
        if (!_allPorts.ContainsKey(portType) || !_portTimers.ContainsKey(portType)) return;

        var port = _allPorts[portType];
        var timerUtils = _portTimers[portType];

        // タイマーが実行中なら停止
        if (timerUtils.HasTimer(portType.ToString()))
        {
            timerUtils.StopTimer(portType.ToString());
        }

        if (_activeAttackPatterns.ContainsKey(portType))
        {
            // タイマーを停止した後、即座に攻撃を実行
            var pattern = _activeAttackPatterns[portType];
            MyLogger.Log($"ポート ({portType}) の攻撃を即時実行！");
            _bossAttackLogic.ExecuteAttack(pattern, port);

            _activeAttackPatterns.Remove(portType);
            port.SetAvailability(true);
        }
        else
        {
            MyLogger.LogWarning($"ポート ({portType}) に攻撃パターンが設定されていません。");
        }
    }

    public void CancelAllAttacks()
    {
        CancelUltimateAttack();
        CancelIndividualPortAttacks();
        MyLogger.Log("すべての攻撃がキャンセルされました。");
    }

    public void CancelAttackOnPort(BossFiringPort portType)
    {
        if (!_portTimers.ContainsKey(portType)) return;

        var timerUtils = _portTimers[portType];
        if (timerUtils.HasTimer(portType.ToString()))
        {
            var port = _allPorts[portType];
            timerUtils.StopTimer(portType.ToString());
            CancelAttackPatternForPort(portType, port);
            port.SetAvailability(true);
            MyLogger.Log($"ポート ({portType}) の攻撃がキャンセルされました。");
        }
    }

    private bool IsPortAvailable(BossFiringPort portType)
    {
        if (!_allPorts.ContainsKey(portType) || !_allPorts[portType].IsAvailable)
        {
            MyLogger.LogWarning($"指定されたポート ({portType}) は利用不可能です。");
            return false;
        }
        return true;
    }

    private void StartPortTimer(BossFiringPort portType, AttackPattern pattern, UnityAction onComplete)
    {
        UnityEvent onCompleteEvent = new UnityEvent();
        onCompleteEvent.AddListener(onComplete);

        _portTimers[portType].StartTimer(portType.ToString(), pattern.AttackPreparationTime, onCompleteEvent);
    }

    private void StartPortTimer(string timerKey, AttackPattern pattern, UnityAction onComplete)
    {
        if (_portTimers.Count == 0)
        {
            MyLogger.LogError("ポートが初期化されていません。大技タイマーを開始できません。");
            return;
        }

        UnityEvent onCompleteEvent = new UnityEvent();
        onCompleteEvent.AddListener(onComplete);

        // 最初のポートのタイマーを開始（第一ポートのみ）
        var firstPortTimer = _portTimers.Values.FirstOrDefault();
        if (firstPortTimer != null)
        {
            firstPortTimer.StartTimer(timerKey, pattern.AttackPreparationTime, onCompleteEvent);
        }
    }

    private void CancelUltimateAttack()
    {
        var firstPortTimer = _portTimers.Values.FirstOrDefault();
        if (firstPortTimer?.HasTimer(ULTIMATE_ATTACK_TIMER) == true)
        {
            firstPortTimer.StopTimer(ULTIMATE_ATTACK_TIMER);
            _bossAttackLogic.OnCanceledMultiPortAttack(_activeUltimatePattern, GetAllPorts());
            _activeUltimatePattern = null;
        }
    }

    private void CancelIndividualPortAttacks()
    {
        foreach (var portType in _portTimers.Keys.ToList())
        {
            if (_portTimers[portType].HasTimer(portType.ToString()))
            {
                CancelAttackOnPort(portType);
            }
        }
    }

    private void CancelAttackPatternForPort(BossFiringPort portType, BossAttackPort port)
    {
        if (_activeAttackPatterns.ContainsKey(portType))
        {
            _bossAttackLogic.OnCanceledAttack(_activeAttackPatterns[portType], port);
            _activeAttackPatterns.Remove(portType);
        }
    }

    private void OnAttackTimerComplete(BossAttackPort port, AttackPattern pattern)
    {
        MyLogger.Log($"ポート ({port.FiringPortType}) から {pattern.AttackName} 攻撃実行！");
        _bossAttackLogic.ExecuteAttack(pattern, port);
        _activeAttackPatterns.Remove(port.FiringPortType);
        port.SetAvailability(true);
    }

    private void OnUltimateAttackTimerComplete(AttackPattern pattern)
    {
        MyLogger.Log("大技発動！");
        _bossAttackLogic.ExecuteMultiPortAttack(pattern, GetAllPorts());
        _activeUltimatePattern = null;

        foreach (var port in _allPorts.Values)
        {
            port.SetAvailability(true);
        }
    }

    // DictionaryのValuesをListとして取得
    private List<BossAttackPort> GetAllPorts()
    {
        List<BossAttackPort> allPorts = new List<BossAttackPort>();
        foreach (var port in _allPorts)
        {
            allPorts.Add(port.Value);
        }
        return allPorts;
    }
}
