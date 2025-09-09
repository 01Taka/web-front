using System;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalSpiderLegPart : BossPart, IDamageable
{
    [SerializeField] private Transform _targetMarkPosition;
    public Transform TargetMarkPosition => _targetMarkPosition;

    private DamageTakenManager _damageManager;

    // 複数のレコードIDとコールバックを管理するための辞書
    private readonly Dictionary<int, Action<float>> _recordedCallbacks = new Dictionary<int, Action<float>>();

    protected override void Awake()
    {
        _damageManager = new DamageTakenManager();
        base.Awake();
    }

    private void OnDestroy()
    {
        // 破棄時にすべてのレコードとコールバックを安全に終了させる
        if (_damageManager != null)
        {
            List<int> recordIdsToCleanup = new List<int>(_recordedCallbacks.Keys);
            foreach (int recordId in recordIdsToCleanup)
            {
                EndRecordAndUnregisterCallback(recordId);
            }
        }
    }

    public override void TakeDamage(float amount)
    {
        _damageManager.TakeDamage(amount);
        base.TakeDamage(amount);
    }

    /// <summary>
    /// 指定されたIDでダメージ記録を開始し、ダメージを受けたときのコールバックを登録します。
    /// 既に同じIDが登録されている場合は、何もしません。
    /// </summary>
    /// <param name="recordId">開始する記録ID</param>
    /// <param name="callback">登録するコールバック関数</param>
    public void StartRecordAndRegisterCallback(int recordId, Action<float> callback)
    {
        // 既に同じIDのレコードが登録されている場合は、何もしない
        if (_recordedCallbacks.ContainsKey(recordId))
        {
            return;
        }

        _damageManager.StartRecord(recordId);
        _damageManager.RegisterOnDamageTakenCallback(callback);
        _recordedCallbacks.Add(recordId, callback);
    }

    /// <summary>
    /// 指定されたIDのダメージ記録を終了し、対応するコールバックを解除して、期間中に受けたダメージを返します。
    /// </summary>
    /// <param name="recordId">終了する記録ID</param>
    /// <returns>記録期間中に受けた総ダメージ量。指定IDが存在しない場合は0。</returns>
    public float EndRecordAndUnregisterCallback(int recordId)
    {
        // 指定されたIDが存在するか確認
        if (!_recordedCallbacks.TryGetValue(recordId, out Action<float> callback))
        {
            return 0f;
        }

        // ダメージを計算し、レコードを終了する
        float recordedDamage = _damageManager.GetRecordedDamage(recordId);
        _damageManager.EndRecord(recordId);

        // コールバックを解除する
        if (callback != null)
        {
            _damageManager.UnregisterOnDamageTakenCallback(callback);
        }

        // 辞書からエントリを削除する
        _recordedCallbacks.Remove(recordId);

        return recordedDamage;
    }

    public float GetRecordedDamage(int recordId)
    {
        // このメソッドは、レコードを終了させずにダメージを取得するために使用される
        return _damageManager.GetRecordedDamage(recordId);
    }
}