using UnityEngine;

public class BubbleShooterScript : MonoBehaviour
{
    [SerializeField] private Transform initialFirePosition;
    [SerializeField] private Rigidbody projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireRate = 1f; // shots per second
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private Camera aimCamera;

    private float nextFireTime;

    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }

        UpdateTrajectoryLine();
    }

    private Vector3 GetMouseWorldPosition()
    {
        float cameraDepth = Vector3.Dot(
            initialFirePosition.position - aimCamera.transform.position,
            aimCamera.transform.forward
        );

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = cameraDepth;

        return aimCamera.ScreenToWorldPoint(mousePosition);
    }

    private void Fire()
    {
        if (Time.time < nextFireTime || fireRate <= 0f)
        {
            return;
        }

        if (initialFirePosition == null ||
            projectilePrefab == null ||
            aimCamera == null)
        {
            Debug.LogWarning(
                "Assign the fire position, projectile prefab, and aim camera.",
                this
            );
            return;
        }

        Vector3 direction =
            (GetMouseWorldPosition() - initialFirePosition.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Rigidbody projectile = Instantiate(
            projectilePrefab,
            initialFirePosition.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        projectile.linearVelocity = direction * projectileSpeed;
        nextFireTime = Time.time + 1f / fireRate;
    }

    private void UpdateTrajectoryLine()
    {
        if (trajectoryLine == null ||
            initialFirePosition == null ||
            aimCamera == null)
        {
            return;
        }

        trajectoryLine.positionCount = 2;
        trajectoryLine.SetPosition(0, initialFirePosition.position);
        trajectoryLine.SetPosition(1, GetMouseWorldPosition());
    }
}