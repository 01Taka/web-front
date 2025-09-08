using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    private readonly Dictionary<GameObject, object> _pools = new Dictionary<GameObject, object>();

    /// <summary>
    /// �w�肵��Prefab�̃C���X�^���X���v�[������擾����
    /// </summary>
    public T Get<T>(T prefab, Transform parent = null, int preloadCount = 5) where T : Component, IPoolable
    {
        if (!_pools.ContainsKey(prefab.gameObject))
        {
            var newPool = new ObjectPool<T>(prefab, preloadCount, parent);
            _pools[prefab.gameObject] = newPool;
        }

        var pool = (ObjectPool<T>)_pools[prefab.gameObject];
        T instance = pool.Get();
        instance.SetPool(pool);

        return instance;
    }
}
