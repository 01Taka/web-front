using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private Transform _projectileParent;
    [SerializeField] private float _projectileScaleRaito = 1f;

    public void SpawnProjectile(ProjectileSpawnParams spawnParams, GameObject projectilePrefab)
    {
        Debug.Log($"Attack <<<<< SpawnProjectile: {spawnParams}");

        Vector2 dir2D = spawnParams.Direction;
        float angle = Mathf.Atan2(dir2D.y, dir2D.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject instance = Instantiate(projectilePrefab, spawnParams.Position, rotation, _projectileParent);

        if (!instance.TryGetComponent(out ProjectileBase projectile))
        {
            Debug.LogError("No ProjectileBase attached");
            return;
        }
        projectile.Initialize(spawnParams);
        projectile.transform.localScale *= _projectileScaleRaito;
    }
}
