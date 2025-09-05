using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [SerializeField] private ScreenShake _screenShake;
    [SerializeField] private ScreenFader _fader;

    private DamageTakenManager _damageTakenManager = new DamageTakenManager();
    public float TakenDamage => _damageTakenManager.TakenDamage;

    private void Awake()
    {
        // _screenShake���ݒ肳��Ă��邩�m�F
        if (_screenShake == null)
        {
            Debug.LogError("ScreenShake�R���|�[�l���g���A�^�b�`����Ă��܂���B", this);
        }

        // _fader���ݒ肳��Ă��邩�m�F
        if (_fader == null)
        {
            Debug.LogError("ScreenFader�R���|�[�l���g���A�^�b�`����Ă��܂���B", this);
        }
    }

    public void TakeDamage(float damage)
    {
        // �_���[�W����
        _damageTakenManager.TakeDamage(damage);

        // �e�R���|�[�l���g��null�łȂ��ꍇ�̂݃��\�b�h���Ăяo��
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