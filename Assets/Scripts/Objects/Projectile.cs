using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float radius = 0.3f; // �ڐG���a
    public LayerMask enemyLayer;

    private Vector2 direction;

    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        // �ړ�
        transform.Translate(direction * speed * Time.deltaTime);

        // �ڐG����i�I�[�o�[���b�v�T�[�N���j
        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, enemyLayer);
        if (hit != null && hit.CompareTag("Enemy"))
        {
            // �����蔻��F������HP�����炷�����ȂǁiHost���ł݂̂��ׂ��j
            // ��: hit.GetComponent<Enemy>().TakeDamage();

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ����͈͂����o�I�Ɋm�F�ł���
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
