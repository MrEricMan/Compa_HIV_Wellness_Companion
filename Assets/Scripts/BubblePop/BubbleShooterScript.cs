using UnityEngine;

public class BubbleShooterScript : MonoBehaviour
{
    [SerializeField] private Transform movingTarget;
    [SerializeField] private Transform initialFirePosition;
    [SerializeField] private Rigidbody projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireRate = 1f;

    private float nextFireTime;

    private void OnMouseDown()
    {
        if (Time.time < nextFireTime || fireRate <= 0f)
        {
            return;
        }

        if (movingTarget == null ||
            initialFirePosition == null ||
            projectilePrefab == null)
        {
            Debug.LogWarning(
                "Assign the moving target, initial fire position, and projectile prefab.",
                this
            );
            return;
        }

        Vector3 direction =
            (movingTarget.position - initialFirePosition.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Rigidbody projectile = Instantiate(
            projectilePrefab,
            initialFirePosition.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        projectile.linearVelocity = direction * projectileSpeed;
        nextFireTime = Time.time + 1f / fireRate;
    }
}