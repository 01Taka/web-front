using UnityEngine;
using System;

public class TimerManager
{
    private float _currentTime;
    private float _startTime;
    private bool _isTimerRunning;

    public Action OnTimerStart;       // �^�C�}�[���J�n���ꂽ�Ƃ�
    public Action OnTimerTick;        // �^�C�}�[���J�E���g�_�E�����邽�тɌĂ΂��
    public Action OnTimerEnd;         // �^�C�}�[���I�������Ƃ�

    public TimerManager(float startTime)
    {
        _startTime = startTime;
        _currentTime = startTime;
        _isTimerRunning = false;
    }

    // �^�C�}�[���J�n
    public void StartTimer()
    {
        if (_isTimerRunning)
        {
            Debug.LogWarning("Timer is already running.");
            return;
        }

        _isTimerRunning = true;
        OnTimerStart?.Invoke();
    }

    // �^�C�}�[���~
    public void StopTimer()
    {
        if (!_isTimerRunning)
        {
            Debug.LogWarning("Timer is not running.");
            return;
        }

        _isTimerRunning = false;
    }

    // �^�C�}�[�����Z�b�g
    public void ResetTimer()
    {
        _currentTime = _startTime;
    }

    // �^�C�}�[�Ɏ��Ԃ�ǉ�
    public void AddTime(float timeToAdd)
    {
        if (timeToAdd < 0f)
        {
            Debug.LogError("Cannot add negative time.");
            return;
        }

        _currentTime += timeToAdd;
    }

    // �^�C�}�[���X�V
    public void UpdateTimer()
    {
        if (!_isTimerRunning) return;

        // ���Ԃ��o��
        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0f)
        {
            _currentTime = 0f;
            StopTimer();
            OnTimerEnd?.Invoke();
        }

        // �R�[���o�b�N���Ăяo���i�I�v�V�����j
        OnTimerTick?.Invoke();
    }

    // ���݂̃^�C�}�[�̎c�莞�Ԃ��擾
    public float GetRemainingTime()
    {
        return _currentTime;
    }
}
