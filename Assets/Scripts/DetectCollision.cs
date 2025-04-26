using System.Collections;
using TMPro;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI player1WinMessage;  // Added reference for Player 1 win message
    public TextMeshProUGUI player2WinMessage;  // Added reference for Player 2 win message
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

    // Static flags to track if both players are dead
    private static bool player1Dead = false;
    private static bool player2Dead = false;

    private void Start()
    {
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        playerAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string objectName = other.name;

        switch (objectName)
        {
            case FenceName:
                HandleFenceCollision(other);
                break;
            case GroundName:
                break;
            default:
                if (objectName.StartsWith(EnemyPrefix)) HandleEnemyCollision(other);
                else if (objectName.StartsWith(WolfPrefix)) HandleWolfCollision();
                else HandleDefaultCollision();
                break;
        }
    }

    private void HandleFenceCollision(Collider other)
    {
        spawnManager?.UpdateScore(20, playerNumber);
        PlayAudioClip(success);
        Destroy(other.gameObject);
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
        TriggerGameOver();
    }

    private void HandleDefaultCollision()
    {
        PlayAudioClip(failure);
        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        PlayAudioClip(failure); // Play sound first
        StartCoroutine(DeactivateAfterDelay(1.5f));

        if (playerNumber == 1)
        {
            player1Dead = true;
            spawnManager.isPlayer1Alive = false;

            // Get PlayerHealth and call Die() on Player 1
            PlayerHealth playerHealth1 = GetComponent<PlayerHealth>();
            if (playerHealth1 != null)
            {
                playerHealth1.Die(); // Mark Player 1 as dead
            }
        }
        else if (playerNumber == 2)
        {
            player2Dead = true;
            spawnManager.isPlayer2Alive = false;

            // Get PlayerHealth and call Die() on Player 2
            PlayerHealth playerHealth2 = GetComponent<PlayerHealth>();
            if (playerHealth2 != null)
            {
                playerHealth2.Die(); // Mark Player 2 as dead
            }
        }

        // Disable movement and renderer components when player dies
        if (GetComponent<PlayerMove>() != null) GetComponent<PlayerMove>().enabled = false;
        if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        if (GetComponent<SpriteRenderer>() != null) GetComponent<SpriteRenderer>().enabled = false;

        // Game over logic for single-player mode
        if (!spawnManager.isMultiplayer)
        {
            player1Dead = true;
            Time.timeScale = 0f;

            if (gameOverText != null)
                gameOverText.gameObject.SetActive(true);

            if (pauseMenuUI != null)
                pauseMenuUI.SetActive(true);

            spawnManager?.EndGame();

            // NEW: Tell the PauseManager the game is over
            GamePauseManager.isGameOver = true;
        }

        // Logic for game over when both players are dead in multiplayer mode
        if (player1Dead && player2Dead)
        {
            Time.timeScale = 0f;

            if (gameOverText != null)
                gameOverText.gameObject.SetActive(true);

            if (pauseMenuUI != null)
                pauseMenuUI.SetActive(true);

            spawnManager?.EndGame();

            // NEW: Tell the PauseManager the game is over
            GamePauseManager.isGameOver = true;

            // NEW: Determine winner and show corresponding message
            int player1Score = spawnManager.GetPlayerScore(1);  // Assuming this function exists
            int player2Score = spawnManager.GetPlayerScore(2);  // Assuming this function exists

            if (player1Score > player2Score)
            {
                if (player1WinMessage != null)
                    player1WinMessage.gameObject.SetActive(true);  // Display Player 1 win message
            }
            else if (player2Score > player1Score)
            {
                if (player2WinMessage != null)
                    player2WinMessage.gameObject.SetActive(true);  // Display Player 2 win message
            }
            else
            {
                // If scores are equal, show a tie message or both win messages
                // You can add a tie message logic here if necessary
            }
        }
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (playerAudio != null && clip != null)
        {
            playerAudio.PlayOneShot(clip, 1.0f);
        }
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
}
