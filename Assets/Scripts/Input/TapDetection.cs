using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class TapDetection : MonoBehaviour
{
    // ① InputActionsのインスタンスをフィールドとして保持する
    private InputActions inputActions;
    private InputAction tapAction;
    private int tapCount = 0;
    private float lastTapTime = 0f;

    [Header("Settings")]
    [SerializeField] private int _targetTapCount = 3;
    [SerializeField] private float _minTimeBetweenTaps = 0.02f;
    [SerializeField] private float _maxTimeBetweenTaps = 0.5f;
    [SerializeField] private UnityEvent _onDetectedTaps;

    void Awake()
    {
        // ② インスタンスを生成してフィールドに代入
        inputActions = new InputActions();
        // ③ 生成したインスタンスからアクションを取得
        tapAction = inputActions.Touch.TouchPress;
        tapAction.canceled += ctx => OnTapPerformed();
        tapAction.Enable();
    }

    void OnTapPerformed()
    {
        float currentTime = Time.time;

        // 最後のタップからの経過時間が最小時間未満の場合は処理を中断
        if (currentTime - lastTapTime < _minTimeBetweenTaps)
        {
            lastTapTime = currentTime;
            return;
        }

        // 最後のタップからの経過時間が最大時間より大きい場合はタップ数をリセット
        if (currentTime - lastTapTime > _maxTimeBetweenTaps)
        {
            tapCount = 0;
        }

        tapCount++;
        lastTapTime = currentTime;

        // 目標のタップ数に達したらイベントを発火し、タップ数をリセット
        if (tapCount >= _targetTapCount)
        {
            _onDetectedTaps?.Invoke();
            tapCount = 0;
        }
    }

    void OnDestroy()
    {
        // ④ インスタンス全体を破棄する
        inputActions.Disable();
        inputActions.Dispose();
    }
}