
public class DamageTakenManager
{
    private float _damage = 0f;
    public float TakenDamage {  get { return _damage; } }

    public void TakeDamage(float damage)
    {
        if (_damage < 0f) return;
        _damage += damage;
    }
}
