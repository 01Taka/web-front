// ファイル名: DamageThresholdHandler.cs
using System;

/// <summary>
/// ダメージのしきい値を管理する通常のC#クラス
/// </summary>
public class DamageThresholdHandler
{
    public event Action OnThresholdReached;
    private float currentDamage = 0f;
    private float cancelThreshold;
    private bool isActive = false;

    public void SetThreshold(float threshold)
    {
        cancelThreshold = threshold;
        currentDamage = 0f;
        isActive = true;
    }

    public void AddDamage(float damage)
    {
        if (!isActive) return;

        currentDamage += damage;
        if (currentDamage >= cancelThreshold)
        {
            OnThresholdReached?.Invoke();
            ResetDamage();
        }
    }

    public void ResetDamage()
    {
        currentDamage = 0f;
        isActive = false;
    }
}