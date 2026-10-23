using UnityEngine;

public class TrajectoryEndPointScript : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    private float bottomY;
    private float topY;
    private float startingX;
    private float startingZ;

    private void Start()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("No camera tagged MainCamera was found.", this);
            enabled = false;
            return;
        }

        float cameraDepth = Vector3.Dot(
            transform.position - mainCamera.transform.position,
            mainCamera.transform.forward
        );

        bottomY = mainCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 0f, cameraDepth)
        ).y;

        topY = mainCamera.ViewportToWorldPoint(
            new Vector3(0.5f, 1f, cameraDepth)
        ).y;

        startingX = transform.position.x;
        startingZ = transform.position.z;
    }

    private void Update()
    {
        float range = topY - bottomY;
        float offset = Mathf.PingPong(Time.time * speed + range, range);
        float y = bottomY + offset;

        transform.position = new Vector3(startingX, y, startingZ);
    }
}