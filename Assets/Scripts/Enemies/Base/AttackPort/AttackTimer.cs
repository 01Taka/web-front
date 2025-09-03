// ファイル名: AttackTimer.cs
using System;

/// <summary>
/// 攻撃までの猶予時間を管理する通常のC#クラス
/// </summary>
public class AttackTimer
{
    public event Action OnTimerComplete;
    private float remainingTime;
    private bool isRunning = false;

    public void StartTimer(float duration)
    {
        remainingTime = duration;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void Tick(float deltaTime)
    {
        if (isRunning)
        {
            remainingTime -= deltaTime;
            if (remainingTime <= 0)
            {
                OnTimerComplete?.Invoke();
                StopTimer();
            }
        }
    }
}