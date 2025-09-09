using UnityEngine;
using System.Collections.Generic;

public interface IPoolable
{
    bool IsReusable { get; set; }
    void SetPool<T>(ObjectPool<T> pool) where T : Component, IPoolable;
    void ReturnToPool();
}

public class ObjectPool<T> where T : Component, IPoolable
{
    private readonly Queue<T> _objects = new Queue<T>();
    private readonly T _prefab;
    private readonly Transform _parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateNew();
            obj.gameObject.SetActive(false);
            obj.IsReusable = true;  // 初期状態では再利用可能
            _objects.Enqueue(obj);
        }
    }

    private T CreateNew()
    {
        return Object.Instantiate(_prefab, _parent);
    }

    public T Get()
    {
        // 再利用可能なオブジェクトを探す
        while (_objects.Count > 0)
        {
            var obj = _objects.Dequeue();

            // IsReusableがtrueでないオブジェクトはスキップ
            if (obj.IsReusable)
            {
                obj.gameObject.SetActive(true);
                obj.IsReusable = false;  // 取得した時点で再利用不可にする
                return obj;
            }
        }

        // 再利用可能なオブジェクトがない場合、新しいオブジェクトを生成
        return CreateNew();
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.IsReusable = true;  // 再利用可能状態に設定
        _objects.Enqueue(obj);
    }
}
