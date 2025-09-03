using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerUtils
{
    private MonoBehaviour _coroutineRunner;
    private Dictionary<string, Coroutine> _activeTimers = new Dictionary<string, Coroutine>();
    private Dictionary<string, UnityEvent> _stopEvents = new Dictionary<string, UnityEvent>();

    /// <summary>
    /// �^�C�}�[�Ǘ��̂��߂�MonoBehaviour�C���X�^���X��������
    /// </summary>
    /// <param name="coroutineRunner">�R���[�`�������s����MonoBehaviour</param>
    public TimerUtils(MonoBehaviour coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    /// <summary>
    /// �w�肵�����O�ŐV�����^�C�}�[���J�n���� (�I�[�o�[���[�h)
    /// </summary>
    /// <param name="timerName">�^�C�}�[�̈�ӂȖ��O</param>
    /// <param name="duration">�^�C�}�[�̌p������</param>
    /// <param name="onComplete">�^�C�}�[�������ɔ��΂���UnityEvent</param>
    /// <param name="onStop">�^�C�}�[����~���ꂽ�Ƃ��ɔ��΂���UnityEvent</param>
    public void StartTimer(string timerName, float duration, UnityEvent onComplete, UnityEvent onStop = null)
    {
        if (_activeTimers.ContainsKey(timerName))
        {
            StopTimer(timerName);
        }

        // ��~�C�x���g�������ɕۑ�
        if (onStop != null)
        {
            _stopEvents[timerName] = onStop;
        }

        Coroutine newTimer = _coroutineRunner.StartCoroutine(TimerCoroutine(timerName, duration, onComplete));
        _activeTimers[timerName] = newTimer;
    }

    /// <summary>
    /// �w�肵�����O�̃^�C�}�[���~����
    /// </summary>
    /// <param name="timerName">��~����^�C�}�[�̖��O</param>
    public void StopTimer(string timerName)
    {
        if (_activeTimers.ContainsKey(timerName))
        {
            _coroutineRunner.StopCoroutine(_activeTimers[timerName]);
            _activeTimers.Remove(timerName);

            // ��~�C�x���g�𔭉΂��A��������폜
            if (_stopEvents.ContainsKey(timerName))
            {
                _stopEvents[timerName]?.Invoke();
                _stopEvents.Remove(timerName);
            }
        }
    }

    /// <summary>
    /// ���ׂẴ^�C�}�[���~����
    /// </summary>
    public void StopAllTimers()
    {
        foreach (var timer in _activeTimers.Values)
        {
            _coroutineRunner.StopCoroutine(timer);
        }
        _activeTimers.Clear();

        // ���ׂĂ̒�~�C�x���g�𔭉�
        foreach (var stopEvent in _stopEvents.Values)
        {
            stopEvent?.Invoke();
        }
        _stopEvents.Clear();
    }

    /// <summary>
    /// �w�肳�ꂽ���O�̃^�C�}�[���A�N�e�B�u���ǂ������m�F����
    /// </summary>
    /// <param name="timerName">�m�F����^�C�}�[�̖��O</param>
    /// <returns>�^�C�}�[�����݂����true�A�Ȃ����false</returns>
    public bool HasTimer(string timerName)
    {
        return _activeTimers.ContainsKey(timerName);
    }

    /// <summary>
    /// �^�C�}�[�̃R���[�`��
    /// </summary>
    private IEnumerator TimerCoroutine(string timerName, float duration, UnityEvent onComplete)
    {
        yield return new WaitForSeconds(duration);

        onComplete?.Invoke();

        // ���������^�C�}�[�̒�~�C�x���g���폜
        if (_stopEvents.ContainsKey(timerName))
        {
            _stopEvents.Remove(timerName);
        }

        _activeTimers.Remove(timerName);
    }
}