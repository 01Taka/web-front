using UnityEngine;

// PoolManagerを継承
public class BomPoolManager : PoolManager
{
    // ボムのプレハブを直接アタッチ
    [SerializeField]
    private MechanicalSpiderBom _bomPrefab;
    [SerializeField]
    private Transform _bomParent;
    [SerializeField]
    private float _prefabInstanceScale = 0.5f;

    // Prefabプロパティで外部からアクセス可能にする
    public MechanicalSpiderBom BomPrefab => _bomPrefab;

    public T Get<T>(int preloadCount = 5) where T : Component, IPoolable
    {
        var component = Get(_bomPrefab as T, _bomParent, preloadCount);
        component.transform.localScale = _bomParent.localScale * _prefabInstanceScale;
        return component;
    }
}