using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnInterval = 1f;
    public float spawnDuration = 10f;

    private void Start()
    {
        if (objectToSpawn == null || spawnInterval <= 0f || spawnDuration <= 0f)
        {
            return;
        }

        InvokeRepeating(nameof(SpawnObject), spawnInterval, spawnInterval);
        Invoke(nameof(StopSpawning), spawnDuration);
    }

    private void SpawnObject()
    {
        Instantiate(objectToSpawn, transform.position, transform.rotation);
    }

    private void StopSpawning()
    {
        CancelInvoke(nameof(SpawnObject));
    }
}
