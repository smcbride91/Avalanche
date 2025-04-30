using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);

    }

    public void ResetAndMove(int sceneId)
    {
        ResetSpawnManagerIfExists();
        MoveToScene(sceneId);
    }

    private void ResetSpawnManagerIfExists()
    {
        SpawnManager sm = FindObjectOfType<SpawnManager>();
        if (sm != null)
        {
            sm.ResetManager();
        }
    }


    public void QuitMultiplayer()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.Shutdown();
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        // Optionally destroy NetworkManager manually
        Destroy(NetworkManager.Singleton.gameObject);

        SceneManager.LoadScene("MainMenu");
    }
}
