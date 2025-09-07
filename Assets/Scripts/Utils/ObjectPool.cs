using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> where T : Component
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
            _objects.Enqueue(obj);
        }
    }

    private T CreateNew()
    {
        return Object.Instantiate(_prefab, _parent);
    }

    public T Get()
    {
        if (_objects.Count > 0 && !_objects.Peek().gameObject.activeSelf)
        {
            var obj = _objects.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            return CreateNew();
        }
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        _objects.Enqueue(obj);
    }
}
