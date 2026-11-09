using UnityEngine;

public class TrajectoryEndPointScript : MonoBehaviour
{
    private Camera mainCamera;
    private float cameraDepth;
    private float objectZ;

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("No camera tagged MainCamera was found.", this);
            enabled = false;
            return;
        }

        objectZ = transform.position.z;
        cameraDepth = Vector3.Dot(
            transform.position - mainCamera.transform.position,
            mainCamera.transform.forward
        );
    }

    private void Update()
{
    Vector3 mousePosition = Input.mousePosition;
    mousePosition.z = cameraDepth;

    transform.position = mainCamera.ScreenToWorldPoint(mousePosition);
}
}