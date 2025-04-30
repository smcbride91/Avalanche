using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Netcode;

public class DetectCollision : NetworkBehaviour
{
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI player1WinMessage;
    public TextMeshProUGUI player2WinMessage;
    public AudioClip success;
    public AudioClip failure;
    public AudioClip bounce;
    public AudioClip bark;
    [SerializeField] private float bounceForce;
    public GameObject pauseMenuUI;
    [SerializeField] private int playerNumber = 1;

    private Rigidbody rb;
    private SpawnManager spawnManager;
    private AudioSource playerAudio;

    private const string FenceName = "SM_Prop_Fence_02(Clone)";
    private const string GroundName = "Ground";
    private const string EnemyPrefix = "Enemy";
    private const string WolfPrefix = "Wolf";

    private static bool player1Dead = false;
    private static bool player2Dead = false;

    private bool isLocalMultiplayer = false;
    private bool isNetworkMultiplayer = false;


    private void Start()
    {
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        playerAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        if (spawnManager != null)
        {
            isLocalMultiplayer = spawnManager.isLocalMultiplayer;
            isNetworkMultiplayer = spawnManager.isNetworkMultiplayer;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string objectName = other.name;

        if (objectName.Contains(FenceName))
        {
            HandleFenceCollision(other);
        }
        else if (objectName == GroundName)
        {
            // Do nothing
        }
        else if (objectName.Contains("Player"))
        {

        }
        else if (objectName.Contains(EnemyPrefix))
        {
            HandleEnemyCollision(other);
        }
        else if (objectName.Contains(WolfPrefix))
        {
            HandleWolfCollision();
        }
        else
        {
            HandleDefaultCollision();
        }


    }

    private void HandleFenceCollision(Collider other)
    {
        if (isNetworkMultiplayer && !IsServer)
        {
            NotifyServerOfFenceScoreServerRpc();

        }
        else
        {
            spawnManager?.UpdateScore(20, playerNumber);
        }

        PlayAudioClip(success);

        if (!isNetworkMultiplayer)
        {
            Destroy(other.gameObject);
        }
        else if (isNetworkMultiplayer && !IsServer && GetComponent<NetworkObject>())
        {
            GetComponent<NetworkObject>().Despawn(true);

        }

    }

    private void HandleEnemyCollision(Collider other)
    {
        PlayAudioClip(bounce);
        Vector3 direction = (transform.position - other.transform.position).normalized;
        Vector3 bounceDirection = new Vector3(direction.x, 0.5f, direction.z).normalized;
        rb?.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        StartCoroutine(ResetVelocityAfterBounce());
    }

    private IEnumerator ResetVelocityAfterBounce()
    {
        yield return new WaitForSeconds(0.1f);
        if (rb != null) rb.velocity = Vector3.zero;
    }

    private void HandleWolfCollision()
    {
        PlayAudioClip(bark);
        TriggerCollisionEvent();
    }

    private void HandleDefaultCollision()
    {
        PlayAudioClip(failure);
        TriggerCollisionEvent();
    }

    private void TriggerCollisionEvent()
    {
        if (isNetworkMultiplayer)
        {
            if (IsServer)
            {
                TriggerGameOverForAll();
            }
            else if (IsOwner)
            {
                ReportCollisionToServerRpc();
            }
        }
        else
        {
            TriggerGameOver();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReportCollisionToServerRpc(ServerRpcParams rpcParams = default)
    {
        TriggerGameOverForAll();
    }

    private void TriggerGameOverForAll()
    {
        TriggerGameOverClientRpc();
        TriggerGameOver();
    }

    [ClientRpc]
    private void TriggerGameOverClientRpc(ClientRpcParams rpcParams = default)
    {
        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        PlayAudioClip(failure);
        StartCoroutine(DeactivateAfterDelay(1.5f));

        // Debugging output to track the server status and player number
        Debug.Log("IsServer: " + IsServer.ToString());
        Debug.Log("Player number: " + playerNumber.ToString());

        if (!isNetworkMultiplayer)
        {
            // Local or single-player logic
            if (playerNumber == 1)
            {
                player1Dead = true;
                if (spawnManager != null) spawnManager.isPlayer1Alive = false;

                PlayerHealth playerHealth1 = GetComponent<PlayerHealth>();
                if (playerHealth1 != null) playerHealth1.Die();
            }
            else if (playerNumber == 2)
            {
                player2Dead = true;
                if (spawnManager != null) spawnManager.isPlayer2Alive = false;

                PlayerHealth playerHealth2 = GetComponent<PlayerHealth>();
                if (playerHealth2 != null) playerHealth2.Die();
            }

            // For local multiplayer, the game will end if both players are dead
            if (player1Dead && player2Dead)
            {
                EndGame();
            }
            else if (!isNetworkMultiplayer&&!isLocalMultiplayer&&player1Dead)
            {
                EndGame();
            }
        }
        else
        {
            // Networked multiplayer logic
            if (IsServer)
            {
                // Server directly updates the death status for both players
                if (playerNumber == 1)
                    spawnManager?.SetNetPlayerDead(1);
                else if (playerNumber == 2)
                    spawnManager?.SetNetPlayerDead(2);

                // Check if both players are dead
                if (spawnManager.netPlayer1Dead.Value && spawnManager.netPlayer2Dead.Value)
                {
                    EndGame();
                }
            }
            else
            {
                // Notify the server that this player is dead
                NotifyServerOfNetworkDeathServerRpc(playerNumber);
                if (spawnManager.netPlayer1Dead.Value && spawnManager.netPlayer2Dead.Value)
                {
                    EndGame();
                }
            }
        }

        // Disable components to indicate player death (for both local and networked multiplayer)
        if (GetComponent<PlayerMove>() != null) GetComponent<PlayerMove>().enabled = false;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;
    }

    private void EndGame()
    {
        Time.timeScale = 0f;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        spawnManager?.EndGame();
        GamePauseManager.isGameOver = true;

        // Show final results based on score
        int player1Score = spawnManager.GetPlayerScore(1);
        int player2Score = spawnManager.GetPlayerScore(2);

        if (player1Score > player2Score)
        {
            if (player1WinMessage != null)
                player1WinMessage.gameObject.SetActive(true);
        }
        else if (player2Score > player1Score)
        {
            if (player2WinMessage != null)
                player2WinMessage.gameObject.SetActive(true);
        }
        else
        {
            // Handle tie if you want
        }
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (playerAudio != null && clip != null)
            playerAudio.PlayOneShot(clip, 1.0f);
    }

    public static void ResetDeathFlags()
    {
        player1Dead = false;
        player2Dead = false;
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyServerOfFenceScoreServerRpc(ServerRpcParams rpcParams = default)
    {
        if (spawnManager == null || !spawnManager.isGameActive)
            return;

        ulong clientId = rpcParams.Receive.SenderClientId;
        int playerNum = spawnManager.GetPlayerNumberByClientId(clientId); // Add this method

        bool isAlive = playerNum == 1 ? spawnManager.isPlayer1Alive : spawnManager.isPlayer2Alive;
        if (isAlive)
        {
            spawnManager.UpdateScore(20, playerNum);
        }
    }
    [ClientRpc]
    private void ShowFinalResultsClientRpc()
    {
        Time.timeScale = 0f;

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        GamePauseManager.isGameOver = true;

        int player1Score = spawnManager.GetPlayerScore(1);
        int player2Score = spawnManager.GetPlayerScore(2);

        if (player1Score > player2Score)
        {
            if (player1WinMessage != null)
                player1WinMessage.gameObject.SetActive(true);
        }
        else if (player2Score > player1Score)
        {
            if (player2WinMessage != null)
                player2WinMessage.gameObject.SetActive(true);
        }
        else
        {
            // Handle tie if you want
        }
    }



    [ServerRpc(RequireOwnership = false)]
    private void NotifyServerOfNetworkDeathServerRpc(int playerNum)
    {
        spawnManager?.SetNetPlayerDead(playerNum);
    }



}
