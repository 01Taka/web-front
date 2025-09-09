using UnityEngine;

public class TargetMarkManager : MonoBehaviour
{
    // シングルトンインスタンスを管理するプライベートな静的変数
    private static TargetMarkManager _instance;

    // 外部からアクセスするための公開プロパティ
    public static TargetMarkManager Instance
    {
        get
        {
            // インスタンスがまだ存在しない場合にエラーを発生させる
            if (_instance == null)
            {
                Debug.LogError("TargetMarkManager is not initialized. Please ensure the instance exists in the scene.");
                return null;
            }
            return _instance;
        }
    }

    // シングルトン化する対象のコンポーネント
    [SerializeField] private TargetMarkController _targetMarkController;

    private void Awake()
    {
        // 既にインスタンスが存在し、かつそれが自分自身ではない場合は、重複しているため破棄する
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // インスタンスがまだ存在しない場合は、自分自身をインスタンスに設定する
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        // インスタンスが破棄される際に、静的変数もクリアする
        if (_instance == this)
        {
            _instance = null;
        }
    }

    // --- 新規追加のラッパーメソッド ---

    /// <summary>
    /// ターゲットをセットして、ロックオンを開始します。
    /// 現在ロックオン中ではない場合にのみ実行可能です。
    /// </summary>
    /// <param name="target">ロックオンするターゲットのTransform</param>
    public void StartLockOn(Transform target)
    {
        // TargetMarkControllerが存在しない、またはロックオン中であればエラー
        if (_targetMarkController == null)
        {
            Debug.LogError("TargetMarkController is not assigned to the TargetMarkManager.");
            return;
        }

        if (_targetMarkController.CurrentState != TargetMarkController.MarkState.Idle)
        {
            Debug.LogError("Cannot start new lock-on. A lock-on is already in progress. Please call ReleaseLockOn() first.");
            return;
        }

        _targetMarkController.SetTarget(target);
    }

    /// <summary>
    /// 現在のロックオンを解除します。
    /// ロックオン中ではない場合は何もしません。
    /// </summary>
    public void ReleaseLockOn()
    {
        if (_targetMarkController == null)
        {
            Debug.LogError("TargetMarkController is not assigned to the TargetMarkManager.");
            return;
        }

        if (_targetMarkController.CurrentState != TargetMarkController.MarkState.Idle)
        {
            _targetMarkController.ReleaseLockOn();
        }
        else
        {
            Debug.LogWarning("ReleaseLockOn was called, but no lock-on was active.");
        }
    }
}