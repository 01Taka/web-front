using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float radius = 0.3f; // 接触半径
    public LayerMask enemyLayer;

    private Vector2 direction;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        // 移動
        transform.Translate(direction * speed * Time.deltaTime);

        // 接触判定（オーバーラップサークル）
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, enemyLayer);
        if (hit != null && hit.CompareTag("Enemy"))
        {
            // 当たり判定：ここでHPを減らす処理など（Host側でのみやるべき）
            // 例: hit.GetComponent<Enemy>().TakeDamage();

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 判定範囲を視覚的に確認できる
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
