using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Transform _projectileParent;
    [SerializeField] private PoolManager _poolManager;

    public void SpawnProjectile(ProjectileSpawnParams spawnParams, ProjectileBase projectilePrefab, int preloadCount = 5)
    {
        ProjectileBase instance = _poolManager.Get(projectilePrefab, _projectileParent, preloadCount);

        // Transformê›íË
        Vector2 dir2D = spawnParams.Direction;
        float angle = Mathf.Atan2(dir2D.y, dir2D.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        instance.transform.SetPositionAndRotation(spawnParams.Position, rotation);
        instance.transform.localScale = projectilePrefab.transform.localScale * spawnParams.ProjectileScaleRaito;

        instance.Initialize(spawnParams, _poolManager, _projectileParent);
    }
}
