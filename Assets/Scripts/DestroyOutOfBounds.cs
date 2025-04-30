using UnityEngine;
using Unity.Netcode; // <-- You need this for NetworkObject stuff!

public class DestroyOutOfBounds : NetworkBehaviour // Inherit from NetworkBehaviour
{
    private const float LowerBound = -14.0f; // Constant for lower boundary
    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager not found in the scene. Ensure it's present and correctly named.");
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
        // If we are running in a networked environment, only the server should despawn objects
        if (_spawnManager.isNetworkMultiplayer)
        {
            
            {
                if (GetComponent<NetworkObject>()&& IsServer)
                {
                    GetComponent<NetworkObject>().Despawn(true); // Networked objects are despawned properly
                    //Debug.Log("I have despawned a network object");
                }
            }
        }
        else
        {
            Destroy(gameObject); // Destroy locally if we are not in a networked game
        }

        // Only award points if the game is not over
        if (!GamePauseManager.isGameOver)
        {
            if (!_spawnManager.isNetworkMultiplayer || IsServer)
            {
                _spawnManager.AwardSharedPoints(20);
            }
        }
    }
}
