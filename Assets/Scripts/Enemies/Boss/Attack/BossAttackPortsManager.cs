using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

public class BossAttackPortsManager
{
    private Dictionary<BossFiringPort, TimerUtils> _portTimers = new Dictionary<BossFiringPort, TimerUtils>();
    private Dictionary<BossFiringPort, BossAttackPort> _allPorts;
    private IBossAttackLogic _bossAttackLogic;

    // ���ݎ��s���̒ʏ�U���p�^�[�����|�[�g���Ƃɕێ�
    private Dictionary<BossFiringPort, AttackPattern> _activeAttackPatterns = new Dictionary<BossFiringPort, AttackPattern>();
    // ���ݎ��s���̑�Z�p�^�[����ێ�
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
            MyLogger.LogWarning($"�w�肳�ꂽ�|�[�g ({portType}) �͗��p�s�\�ł��B");
            return;
        }

        var port = _allPorts[portType];
        port.SetAvailability(false);
        MyLogger.Log($"�|�[�g ({portType}) ���� {pattern.AttackName} �U��������...");

        _bossAttackLogic.OnBeginPreparation(pattern, port);
        _activeAttackPatterns[portType] = pattern;

        UnityEvent onCompleteEvent = new UnityEvent();
        onCompleteEvent.AddListener(() => OnAttackTimerComplete(port, pattern));

        _portTimers[portType].StartTimer(portType.ToString(), pattern.AttackPreparationTime, onCompleteEvent);
    }

    public void StartUltimateAttack(AttackPattern pattern)
    {
        CancelAllAttacks();

        MyLogger.Log($"���ׂẴ|�[�g���g������Z�̏����J�n...");

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
            MyLogger.LogError("�|�[�g������������Ă��܂���B��Z�^�C�}�[���J�n�ł��܂���B");
        }
    }

    public void CancelAllAttacks()
    {
        // ��Z�̃^�C�}�[�������Ă��邩�`�F�b�N
        if (_portTimers.Count > 0 && _portTimers.First().Value.HasTimer(ULTIMATE_ATTACK_TIMER))
        {
            _portTimers.First().Value.StopTimer(ULTIMATE_ATTACK_TIMER);
            _bossAttackLogic.OnCanceledMultiPortAttack(_activeUltimatePattern, _allPorts.Values.ToList());
            _activeUltimatePattern = null;
        }

        // �ʂ̃|�[�g�^�C�}�[���~
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

        MyLogger.Log("���ׂĂ̍U�����L�����Z������܂����B");
    }

    /// <summary>
    /// ����̃|�[�g�Ŏ��s���̍U�����L�����Z�����܂��B
    /// </summary>
    /// <param name="portType">�L�����Z������|�[�g�̃^�C�v</param>
    public void CancelAttackOnPort(BossFiringPort portType)
    {
        if (_portTimers.ContainsKey(portType) && _portTimers[portType].HasTimer(portType.ToString()))
        {
            var timerUtils = _portTimers[portType];
            var port = _allPorts[portType];

            timerUtils.StopTimer(portType.ToString());

            // �ۑ����Ă������U���p�^�[����n��
            if (_activeAttackPatterns.ContainsKey(portType))
            {
                _bossAttackLogic.OnCanceledAttack(_activeAttackPatterns[portType], port);
                _activeAttackPatterns.Remove(portType);
            }

            port.SetAvailability(true);
            MyLogger.Log($"�|�[�g ({portType}) �̍U�����L�����Z������܂����B");
        }
    }

    private void OnAttackTimerComplete(BossAttackPort port, AttackPattern pattern)
    {
        MyLogger.Log($"�|�[�g ({port.FiringPortType}) ���� {pattern.AttackName} �U�����s�I");
        _bossAttackLogic.ExecuteAttack(pattern, port);

        // �U���������ɃA�N�e�B�u�p�^�[�������폜
        _activeAttackPatterns.Remove(port.FiringPortType);

        port.SetAvailability(true);
    }

    private void OnUltimateAttackTimerComplete(AttackPattern pattern)
    {
        MyLogger.Log("��Z�����I");
        _bossAttackLogic.ExecuteMultiPortAttack(pattern, _allPorts.Values.ToList());

        _activeUltimatePattern = null;

        foreach (var port in _allPorts.Values)
        {
            port.SetAvailability(true);
        }
    }
}