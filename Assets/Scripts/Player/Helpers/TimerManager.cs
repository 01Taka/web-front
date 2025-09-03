using UnityEngine;
using System;

public class TimerManager
{
    private float _currentTime;
    private float _startTime;
    private bool _isTimerRunning;

    public Action OnTimerStart;       // タイマーが開始されたとき
    public Action OnTimerTick;        // タイマーがカウントダウンするたびに呼ばれる
    public Action OnTimerEnd;         // タイマーが終了したとき

    public TimerManager(float startTime)
    {
        _startTime = startTime;
        _currentTime = startTime;
        _isTimerRunning = false;
    }

    // タイマーを開始
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

    // タイマーを停止
    public void StopTimer()
    {
        if (!_isTimerRunning)
        {
            Debug.LogWarning("Timer is not running.");
            return;
        }

        _isTimerRunning = false;
    }

    // タイマーをリセット
    public void ResetTimer()
    {
        _currentTime = _startTime;
    }

    // タイマーに時間を追加
    public void AddTime(float timeToAdd)
    {
        if (timeToAdd < 0f)
        {
            Debug.LogError("Cannot add negative time.");
            return;
        }

        _currentTime += timeToAdd;
    }

    // タイマーを更新
    public void UpdateTimer()
    {
        if (!_isTimerRunning) return;

        // 時間が経過
        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0f)
        {
            _currentTime = 0f;
            StopTimer();
            OnTimerEnd?.Invoke();
        }

        // コールバックを呼び出す（オプション）
        OnTimerTick?.Invoke();
    }

    // 現在のタイマーの残り時間を取得
    public float GetRemainingTime()
    {
        return _currentTime;
    }
}
