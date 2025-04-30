using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class SimpleNetworkStarter : MonoBehaviour
{
    private bool gameStarted = false;
    private bool showInstructions = true;

    void OnGUI()
    {
        if (showInstructions)
        {
            GUI.Label(new Rect(10, 10, 400, 30), "Press H to Host or C to Join");
        }
    }
        void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            NetworkManager.Singleton.StartHost();
            showInstructions = false;
                // Host won't start game until a client connects
            }

        if (Input.GetKeyDown(KeyCode.C))
        {
            NetworkManager.Singleton.StartClient();
            showInstructions = false;
            }

        if (Input.GetKeyDown(KeyCode.V))
        {
            NetworkManager.Singleton.StartServer();
            // Same here — delay starting the game until at least one client connects
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return; // only server/host should do this
        if (gameStarted) return; // prevent double starts

        if (NetworkManager.Singleton.ConnectedClients.Count > 1) // host + at least one client
        {
            gameStarted = true;
            StartCoroutine(DelayedSpawnInit());
        }
    }

    private IEnumerator DelayedSpawnInit()
    {
        yield return new WaitForSeconds(1f); // Give time for everything to spawn

        var manager = FindObjectOfType<SpawnManager>();
        manager.SpawnRandomEnemy();
        manager.InvokeRepeating(nameof(manager.SpawnRandomObstacle), 1f, 2f);
    }
}
