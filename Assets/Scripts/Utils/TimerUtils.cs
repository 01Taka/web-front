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
    /// タイマー管理のためのMonoBehaviourインスタンスを初期化
    /// </summary>
    /// <param name="coroutineRunner">コルーチンを実行するMonoBehaviour</param>
    public TimerUtils(MonoBehaviour coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    /// <summary>
    /// 指定した名前で新しいタイマーを開始する (オーバーロード)
    /// </summary>
    /// <param name="timerName">タイマーの一意な名前</param>
    /// <param name="duration">タイマーの継続時間</param>
    /// <param name="onComplete">タイマー完了時に発火するUnityEvent</param>
    /// <param name="onStop">タイマーが停止されたときに発火するUnityEvent</param>
    public void StartTimer(string timerName, float duration, UnityEvent onComplete, UnityEvent onStop = null)
    {
        if (_activeTimers.ContainsKey(timerName))
        {
            StopTimer(timerName);
        }

        // 停止イベントを辞書に保存
        if (onStop != null)
        {
            _stopEvents[timerName] = onStop;
        }

        Coroutine newTimer = _coroutineRunner.StartCoroutine(TimerCoroutine(timerName, duration, onComplete));
        _activeTimers[timerName] = newTimer;
    }

    /// <summary>
    /// 指定した名前のタイマーを停止する
    /// </summary>
    /// <param name="timerName">停止するタイマーの名前</param>
    public void StopTimer(string timerName)
    {
        if (_activeTimers.ContainsKey(timerName))
        {
            _coroutineRunner.StopCoroutine(_activeTimers[timerName]);
            _activeTimers.Remove(timerName);

            // 停止イベントを発火し、辞書から削除
            if (_stopEvents.ContainsKey(timerName))
            {
                _stopEvents[timerName]?.Invoke();
                _stopEvents.Remove(timerName);
            }
        }
    }

    /// <summary>
    /// すべてのタイマーを停止する
    /// </summary>
    public void StopAllTimers()
    {
        foreach (var timer in _activeTimers.Values)
        {
            _coroutineRunner.StopCoroutine(timer);
        }
        _activeTimers.Clear();

        // すべての停止イベントを発火
        foreach (var stopEvent in _stopEvents.Values)
        {
            stopEvent?.Invoke();
        }
        _stopEvents.Clear();
    }

    /// <summary>
    /// 指定された名前のタイマーがアクティブかどうかを確認する
    /// </summary>
    /// <param name="timerName">確認するタイマーの名前</param>
    /// <returns>タイマーが存在すればtrue、なければfalse</returns>
    public bool HasTimer(string timerName)
    {
        return _activeTimers.ContainsKey(timerName);
    }

    /// <summary>
    /// タイマーのコルーチン
    /// </summary>
    private IEnumerator TimerCoroutine(string timerName, float duration, UnityEvent onComplete)
    {
        yield return new WaitForSeconds(duration);

        onComplete?.Invoke();

        // 完了したタイマーの停止イベントを削除
        if (_stopEvents.ContainsKey(timerName))
        {
            _stopEvents.Remove(timerName);
        }

        _activeTimers.Remove(timerName);
    }
}