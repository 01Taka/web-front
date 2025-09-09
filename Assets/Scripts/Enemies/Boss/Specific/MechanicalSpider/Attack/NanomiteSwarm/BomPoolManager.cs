using UnityEngine;

// PoolManager���p��
public class BomPoolManager : PoolManager
{
    // �{���̃v���n�u�𒼐ڃA�^�b�`
    [SerializeField]
    private MechanicalSpiderBom _bomPrefab;
    [SerializeField]
    private Transform _bomParent;
    [SerializeField]
    private float _prefabInstanceScale = 0.5f;

    // Prefab�v���p�e�B�ŊO������A�N�Z�X�\�ɂ���
    public MechanicalSpiderBom BomPrefab => _bomPrefab;

    public T Get<T>(int preloadCount = 5) where T : Component, IPoolable
    {
        var component = Get(_bomPrefab as T, _bomParent, preloadCount);
        component.transform.localScale = _bomParent.localScale * _prefabInstanceScale;
        return component;
    }
}