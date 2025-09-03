using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class BossAttackPortsManager
{
    private Dictionary<BossFiringPort, TimerUtils> _portTimers = new Dictionary<BossFiringPort, TimerUtils>();
    private Dictionary<BossFiringPort, BossAttackPort> _allPorts;
    private IBossAttackLogic _bossAttackLogic;

    // 現在実行中の通常攻撃パターンをポートごとに保持
    private Dictionary<BossFiringPort, AttackPattern> _activeAttackPatterns = new Dictionary<BossFiringPort, AttackPattern>();
    // 現在実行中の大技パターンを保持
    private AttackPattern _activeUltimatePattern;

    private readonly string ULTIMATE_ATTACK_TIMER = "UltimateAttackTimer";

    public BossAttackPortsManager(MonoBehaviour coroutineRunner, Dictionary<BossFiringPort, BossAttackPort> ports, IBossAttackLogic bossAttackLogic)
    {
        _allPorts = ports;
        _bossAttackLogic = bossAttackLogic;

        foreach (var pair in _allPorts)
        {
            _portTimers[pair.Key] = new TimerUtils(coroutineRunner);
        }
    }

    public List<BossAttackPort> GetAvailablePorts()
    {
        return _allPorts.Values.Where(p => p.IsAvailable).ToList();
    }

    public void StartAttack(BossFiringPort portType, AttackPattern pattern)
    {
        if (!_allPorts.ContainsKey(portType) || !_allPorts[portType].IsAvailable)
        {
            MyLogger.LogWarning($"指定されたポート ({portType}) は利用不可能です。");
            return;
        }

        var port = _allPorts[portType];
        port.SetAvailability(false);
        MyLogger.Log($"ポート ({portType}) から {pattern.AttackName} 攻撃準備中...");

        _bossAttackLogic.OnBeginPreparation(pattern, port);
        _activeAttackPatterns[portType] = pattern;

        UnityEvent onCompleteEvent = new UnityEvent();
        onCompleteEvent.AddListener(() => OnAttackTimerComplete(port, pattern));

        _portTimers[portType].StartTimer(portType.ToString(), pattern.AttackPreparationTime, onCompleteEvent);
    }

    public void StartUltimateAttack(AttackPattern pattern)
    {
        CancelAllAttacks();

        MyLogger.Log($"すべてのポートを使った大技の準備開始...");

        _bossAttackLogic.OnBeginMultiPortAttack(pattern, _allPorts.Values.ToList());
        _activeUltimatePattern = pattern;

        UnityEvent onCompleteEvent = new UnityEvent();
        onCompleteEvent.AddListener(() => OnUltimateAttackTimerComplete(pattern));

        if (_portTimers.Count > 0)
        {
            _portTimers.First().Value.StartTimer(ULTIMATE_ATTACK_TIMER, pattern.AttackPreparationTime, onCompleteEvent);
        }
        else
        {
            MyLogger.LogError("ポートが初期化されていません。大技タイマーを開始できません。");
        }
    }

    public void CancelAllAttacks()
    {
        // 大技のタイマーが動いているかチェック
        if (_portTimers.Count > 0 && _portTimers.First().Value.HasTimer(ULTIMATE_ATTACK_TIMER))
        {
            _portTimers.First().Value.StopTimer(ULTIMATE_ATTACK_TIMER);
            _bossAttackLogic.OnCanceledMultiPortAttack(_activeUltimatePattern, _allPorts.Values.ToList());
            _activeUltimatePattern = null;
        }

        // 個別のポートタイマーを停止
        foreach (var pair in _portTimers)
        {
            var portType = pair.Key;
            var timerUtils = pair.Value;
            var port = _allPorts[portType];

            if (timerUtils.HasTimer(portType.ToString()))
            {
                CancelAttackOnPort(portType);
            }
        }

        MyLogger.Log("すべての攻撃がキャンセルされました。");
    }

    /// <summary>
    /// 特定のポートで実行中の攻撃をキャンセルします。
    /// </summary>
    /// <param name="portType">キャンセルするポートのタイプ</param>
    public void CancelAttackOnPort(BossFiringPort portType)
    {
        if (_portTimers.ContainsKey(portType) && _portTimers[portType].HasTimer(portType.ToString()))
        {
            var timerUtils = _portTimers[portType];
            var port = _allPorts[portType];

            timerUtils.StopTimer(portType.ToString());

            // 保存しておいた攻撃パターンを渡す
            if (_activeAttackPatterns.ContainsKey(portType))
            {
                _bossAttackLogic.OnCanceledAttack(_activeAttackPatterns[portType], port);
                _activeAttackPatterns.Remove(portType);
            }

            port.SetAvailability(true);
            MyLogger.Log($"ポート ({portType}) の攻撃がキャンセルされました。");
        }
    }

    private void OnAttackTimerComplete(BossAttackPort port, AttackPattern pattern)
    {
        MyLogger.Log($"ポート ({port.FiringPortType}) から {pattern.AttackName} 攻撃実行！");
        _bossAttackLogic.ExecuteAttack(pattern, port);

        // 攻撃完了時にアクティブパターン情報を削除
        _activeAttackPatterns.Remove(port.FiringPortType);

        port.SetAvailability(true);
    }

    private void OnUltimateAttackTimerComplete(AttackPattern pattern)
    {
        MyLogger.Log("大技発動！");
        _bossAttackLogic.ExecuteMultiPortAttack(pattern, _allPorts.Values.ToList());

        _activeUltimatePattern = null;

        foreach (var port in _allPorts.Values)
        {
            port.SetAvailability(true);
        }
    }
}