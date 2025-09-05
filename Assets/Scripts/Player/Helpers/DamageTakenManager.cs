using System;
using System.Collections.Generic;

public class DamageTakenManager
{
    private float _damage = 0f;
    public float TakenDamage { get { return _damage; } }

    private Dictionary<int, float> _recordedDamageStarts = new Dictionary<int, float>();

    // イベントを private にして外部からの直接アクセスを制限する
    private event Action<float> OnDamageTakenEvent;

    /// <summary>
    /// ダメージを受けたときのコールバック関数を登録します。
    /// </summary>
    /// <param name="callback">登録するコールバック関数</param>
    public void RegisterOnDamageTakenCallback(Action<float> callback)
    {
        OnDamageTakenEvent += callback;
    }

    /// <summary>
    /// ダメージを受けたときのコールバック関数を削除します。
    /// </summary>
    /// <param name="callback">削除するコールバック関数</param>
    public void UnregisterOnDamageTakenCallback(Action<float> callback)
    {
        OnDamageTakenEvent -= callback;
    }

    /// <summary>
    /// 登録されているすべてのコールバック関数を削除します。
    /// </summary>
    public void ClearAllCallbacks()
    {
        // イベントに登録されたすべてのデリゲートを解放する
        OnDamageTakenEvent = null;
    }

    public void TakeDamage(float damage)
    {
        if (damage < 0f) return;
        _damage += damage;

        // イベントを呼び出す
        OnDamageTakenEvent?.Invoke(damage);
    }

    public void StartRecord(int recordId)
    {
        if (_recordedDamageStarts.ContainsKey(recordId))
        {
            return;
        }
        _recordedDamageStarts[recordId] = _damage;
    }

    public float GetRecordedDamage(int recordId)
    {
        if (!_recordedDamageStarts.ContainsKey(recordId))
        {
            return 0f;
        }
        float startDamage = _recordedDamageStarts[recordId];
        return _damage - startDamage;
    }

    public void EndRecord(int recordId)
    {
        _recordedDamageStarts.Remove(recordId);
    }
}