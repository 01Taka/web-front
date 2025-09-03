using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AttackPointSet
{
    public int pointIndex;
    public Transform point;
}

public class AttackPointManager : MonoBehaviour
{
    // 発射位置の計算に使用する2つのトランスフォーム
    [Header("Dynamic Spawn Points")]
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    private Dictionary<int, Transform> _attackPointMap;
    // 初期化状態を追跡するためのフラグ
    private bool _isInitialized = false;

    // 以前のAwakeメソッドの代わりに、明示的な初期化メソッドを用意
    public void Initialize(int numberOfPoints)
    {
        // 多重初期化を防ぐためのチェック
        if (_isInitialized)
        {
            Debug.LogWarning("AttackPointManager has already been initialized.");
            return;
        }

        if (_startPoint == null || _endPoint == null)
        {
            Debug.LogError("Start Point or End Point is not assigned. Initialization failed.");
            return;
        }

        _attackPointMap = new Dictionary<int, Transform>();

        // 人数分の発射位置を動的に生成
        GenerateSpawnPoints(numberOfPoints);

        _isInitialized = true;
    }

    /// <summary>
    /// startPointとendPointの間に、指定された数の発射位置をspace-around形式で生成します。
    /// </summary>
    /// <param name="count">生成する発射位置の数</param>
    private void GenerateSpawnPoints(int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning("Cannot generate spawn points. Count must be greater than 0.");
            return;
        }

        Vector3 startPos = _startPoint.position;
        Vector3 endPos = _endPoint.position;

        // 2点間の距離と方向を計算
        Vector3 direction = endPos - startPos;
        float totalLength = direction.magnitude;
        direction.Normalize();

        // space-around形式で各位置を計算
        for (int i = 0; i < count; i++)
        {
            // 0から1の範囲で、各ポイントの正規化された位置を計算
            float normalizedPosition = (i + 0.5f) / count;
            Vector3 newPosition = startPos + direction * (totalLength * normalizedPosition);

            // 新しいGameObjectを作成し、その位置を設定
            GameObject newPoint = new GameObject($"SpawnPoint_{i}");
            newPoint.transform.position = newPosition;
            // 階層を整理するために親を設定
            newPoint.transform.parent = this.transform;

            // 生成したポイントをマップに追加
            _attackPointMap.Add(i, newPoint.transform);
        }
    }

    public bool TryGetAttackPoint(int pointIndex, out Vector3 attackPosition)
    {
        // 初期化が行われているかチェック
        if (!_isInitialized || _attackPointMap == null)
        {
            Debug.LogError("Cannot get attack point. The manager has not been initialized.");
            attackPosition = Vector3.zero;
            return false;
        }

        if (_attackPointMap.TryGetValue(pointIndex, out Transform point) && point != null)
        {
            attackPosition = point.position;
            return true;
        }

        Debug.LogWarning($"Attack point with index {pointIndex} not found.");
        attackPosition = Vector3.zero;
        return false;
    }

    // デバッグ目的で、生成されたポイントをシーンビューに表示
    private void OnDrawGizmos()
    {
        // 初期化が行われているかチェック
        if (!_isInitialized || _attackPointMap == null) return;

        Gizmos.color = Color.red;
        foreach (var pair in _attackPointMap)
        {
            if (pair.Value != null)
            {
                Gizmos.DrawSphere(pair.Value.position, 0.2f);
            }
        }
    }

    // クリーンアップ処理を追加
    private void OnDestroy()
    {
        if (_attackPointMap != null)
        {
            foreach (var pair in _attackPointMap)
            {
                if (pair.Value != null)
                {
                    Destroy(pair.Value.gameObject);
                }
            }
            _attackPointMap.Clear();
        }
    }
}