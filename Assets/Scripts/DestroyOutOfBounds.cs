using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private const float LowerBound = -14.0f; // Constant for lower boundary
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager not found. Ensure there's a GameObject named 'SpawnManager' with a SpawnManager script attached.");
        }
    }

    private void Update()
    {
        if (transform.position.z < LowerBound)
        {
            HandleOutOfBounds();
        }
    }

    private void HandleOutOfBounds()
    {
        Destroy(gameObject);
        if (_spawnManager != null)
        {
            _spawnManager.UpdateScore(5);
        }
    }
}
