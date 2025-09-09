using UnityEngine;

public class BomTargetMarker : MonoBehaviour
{
    private Transform _targetTransform;
    private float _speed;
    private IBomMovement _movementStrategy;
    private bool _isMoving;

    public void Initialize(Vector3 startPosition, Transform target, float speed, IBomMovement movementStrategy)
    {
        transform.position = startPosition;
        _targetTransform = target;
        _speed = speed;
        _movementStrategy = movementStrategy;
        _isMoving = true;

        // TargetMarkManager‚Å•\Ž¦
        TargetMarkManager.Instance.StartLockOn(transform);
    }

    private void Update()
    {
        if (_targetTransform != null && _isMoving)
        {
            _movementStrategy.Move(transform, _targetTransform.position, _speed);
        }
    }

    public void ReleaseMarker()
    {
        TargetMarkManager.Instance.ReleaseLockOn();
        _isMoving = false;
    }
}