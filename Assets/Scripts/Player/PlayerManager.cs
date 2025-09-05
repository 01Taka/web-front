using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [SerializeField] private ScreenShake _screenShake;
    [SerializeField] private ScreenFader _fader;

    private DamageTakenManager _damageTakenManager = new DamageTakenManager();
    public float TakenDamage => _damageTakenManager.TakenDamage;

    private void Awake()
    {
        // _screenShakeが設定されているか確認
        if (_screenShake == null)
        {
            Debug.LogError("ScreenShakeコンポーネントがアタッチされていません。", this);
        }

        // _faderが設定されているか確認
        if (_fader == null)
        {
            Debug.LogError("ScreenFaderコンポーネントがアタッチされていません。", this);
        }
    }

    public void TakeDamage(float damage)
    {
        // ダメージ処理
        _damageTakenManager.TakeDamage(damage);

        // 各コンポーネントがnullでない場合のみメソッドを呼び出す
        if (_screenShake != null)
        {
            _screenShake.StartShake();
        }

        if (_fader != null)
        {
            _fader.Flash();
        }
    }
}