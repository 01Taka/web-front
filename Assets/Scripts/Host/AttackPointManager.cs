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
    // ���ˈʒu�̌v�Z�Ɏg�p����2�̃g�����X�t�H�[��
    [Header("Dynamic Spawn Points")]
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    private Dictionary<int, Transform> _attackPointMap;
    // ��������Ԃ�ǐՂ��邽�߂̃t���O
    private bool _isInitialized = false;

    // �ȑO��Awake���\�b�h�̑���ɁA�����I�ȏ��������\�b�h��p��
    public void Initialize(int numberOfPoints)
    {
        // ���d��������h�����߂̃`�F�b�N
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

        // �l�����̔��ˈʒu�𓮓I�ɐ���
        GenerateSpawnPoints(numberOfPoints);

        _isInitialized = true;
    }

    /// <summary>
    /// startPoint��endPoint�̊ԂɁA�w�肳�ꂽ���̔��ˈʒu��space-around�`���Ő������܂��B
    /// </summary>
    /// <param name="count">�������锭�ˈʒu�̐�</param>
    private void GenerateSpawnPoints(int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning("Cannot generate spawn points. Count must be greater than 0.");
            return;
        }

        Vector3 startPos = _startPoint.position;
        Vector3 endPos = _endPoint.position;

        // 2�_�Ԃ̋����ƕ������v�Z
        Vector3 direction = endPos - startPos;
        float totalLength = direction.magnitude;
        direction.Normalize();

        // space-around�`���Ŋe�ʒu���v�Z
        for (int i = 0; i < count; i++)
        {
            // 0����1�͈̔͂ŁA�e�|�C���g�̐��K�����ꂽ�ʒu���v�Z
            float normalizedPosition = (i + 0.5f) / count;
            Vector3 newPosition = startPos + direction * (totalLength * normalizedPosition);

            // �V����GameObject���쐬���A���̈ʒu��ݒ�
            GameObject newPoint = new GameObject($"SpawnPoint_{i}");
            newPoint.transform.position = newPosition;
            // �K�w�𐮗����邽�߂ɐe��ݒ�
            newPoint.transform.parent = this.transform;

            // ���������|�C���g���}�b�v�ɒǉ�
            _attackPointMap.Add(i, newPoint.transform);
        }
    }

    public bool TryGetAttackPoint(int pointIndex, out Vector3 attackPosition)
    {
        // ���������s���Ă��邩�`�F�b�N
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

    // �f�o�b�O�ړI�ŁA�������ꂽ�|�C���g���V�[���r���[�ɕ\��
    private void OnDrawGizmos()
    {
        // ���������s���Ă��邩�`�F�b�N
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

    // �N���[���A�b�v������ǉ�
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