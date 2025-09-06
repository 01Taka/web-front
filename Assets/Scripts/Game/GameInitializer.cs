using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameObject _singletonObjectsPrefab;
    private bool _initialized = false;

    // 唯一のインスタンスを格納するプライベートな静的変数
    private static GameInitializer _instance;

    // どこからでもアクセスできる唯一のインスタンスを返す静的プロパティ
    public static GameInitializer Instance
    {
        get
        {
            // インスタンスがまだ存在しない場合、シーン内から探して取得する
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameInitializer>();

                // それでも見つからない場合、警告ログを出力
                if (_instance == null)
                {
                    Debug.LogWarning("GameInitializerのインスタンスがシーンに見つかりません。");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // _instanceがまだ設定されていない場合、このオブジェクトをインスタンスとして設定
        if (_instance == null)
        {
            _instance = this;
            // シーンを跨いでもオブジェクトが破棄されないように設定
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 既にインスタンスが存在する場合、新しい方のオブジェクトを破棄
            // このチェックがないと、シーン移動などで複数のインスタンスが生成される可能性がある
            if (this != _instance)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        if (!_initialized)
        {
            Instantiate(_singletonObjectsPrefab);
            _initialized = true;
        }
    }
}