using UnityEngine;

public class ColliderCounter : MonoBehaviour
{
    // 検出の中心位置
    public Transform detectionCenter;

    // 検出の半径（OverlapCircle用）
    public float detectionRadius = 5.0f;

    // 検出の矩形サイズ（OverlapBox用）
    public Vector2 detectionBoxSize = new Vector2(5.0f, 5.0f);

    // 検出の方向（Raycast用）
    public Vector2 raycastDirection = Vector2.right;

    // 検出対象のレイヤー
    public LayerMask targetLayer;

    private void Update()
    {
        // ログをクリアして、見やすくする
        Debug.ClearDeveloperConsole();

        // Physics2D.OverlapCircleAll(): 円内のすべてのコライダーを検出
        CheckOverlapCircleAll();

        // Physics2D.OverlapBoxAll(): 矩形内のすべてのコライダーを検出
        CheckOverlapBoxAll();

        // Physics2D.RaycastAll(): 指定した方向のレイに当たるすべてのコライダーを検出
        CheckRaycastAll();

        // Physics2D.OverlapCircle(): 単一のコライダーを検出（最も近いもの）
        CheckOverlapCircle();
    }

    private void OnDrawGizmos()
    {
        // Gizmosを使ってデバッグ用の図形を描画
        Vector3 center = detectionCenter != null ? detectionCenter.position : transform.position;

        // OverlapCircleの円
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectionRadius);

        // OverlapBoxの矩形
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, detectionBoxSize);

        // Raycastのレイ
        Gizmos.color = Color.red;
        Gizmos.DrawLine(center, center + (Vector3)raycastDirection * 10f); // 10fは適当な長さ
    }

    //---------------------------------------------------------
    // 各種検知メソッド
    //---------------------------------------------------------

    private void CheckOverlapCircleAll()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionCenter.position, detectionRadius, targetLayer);
        Debug.Log("OverlapCircleAll: " + hitColliders.Length + " 個のオブジェクトを検出しました。");
        foreach (Collider2D collider in hitColliders)
        {
            Debug.Log("  - 検出したオブジェクト: " + collider.gameObject.name);
        }
    }

    private void CheckOverlapBoxAll()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(detectionCenter.position, detectionBoxSize, 0f, targetLayer);
        Debug.Log("OverlapBoxAll: " + hitColliders.Length + " 個のオブジェクトを検出しました。");
        foreach (Collider2D collider in hitColliders)
        {
            Debug.Log("  - 検出したオブジェクト: " + collider.gameObject.name);
        }
    }

    private void CheckRaycastAll()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(detectionCenter.position, raycastDirection, 10f, targetLayer);
        Debug.Log("RaycastAll: " + hits.Length + " 個のオブジェクトを検出しました。");
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log("  - 検出したオブジェクト: " + hit.collider.gameObject.name);
        }
    }

    private void CheckOverlapCircle()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(detectionCenter.position, detectionRadius, targetLayer);
        if (hitCollider != null)
        {
            Debug.Log("OverlapCircle: " + hitCollider.gameObject.name + " を検出しました。");
        }
    }
}