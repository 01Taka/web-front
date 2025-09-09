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
            obj.IsReusable = true;  // ������Ԃł͍ė��p�\
            _objects.Enqueue(obj);
        }
    }

    private T CreateNew()
    {
        return Object.Instantiate(_prefab, _parent);
    }

    public T Get()
    {
        // �ė��p�\�ȃI�u�W�F�N�g��T��
        while (_objects.Count > 0)
        {
            var obj = _objects.Dequeue();

            // IsReusable��true�łȂ��I�u�W�F�N�g�̓X�L�b�v
            if (obj.IsReusable)
            {
                obj.gameObject.SetActive(true);
                obj.IsReusable = false;  // �擾�������_�ōė��p�s�ɂ���
                return obj;
            }
        }

        // �ė��p�\�ȃI�u�W�F�N�g���Ȃ��ꍇ�A�V�����I�u�W�F�N�g�𐶐�
        return CreateNew();
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.IsReusable = true;  // �ė��p�\��Ԃɐݒ�
        _objects.Enqueue(obj);
    }
}
