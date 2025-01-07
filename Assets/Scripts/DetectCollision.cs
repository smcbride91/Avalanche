using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class DetectCollision : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    public AudioClip success;
    public AudioClip failure;
    public AudioClip bounce;
    public AudioClip bark;
    [SerializeField] private float bounceForce;

    private Rigidbody rb;
    private SpawnManager spawnManager;
    private AudioSource playerAudio;

    // Constants for object names
    private const string SpawnManagerName = "SpawnManager";
    private const string FenceName = "SM_Prop_Fence_02(Clone)";
    private const string GroundName = "Ground";
    private const string EnemyPrefix = "Enemy";
    private const string WolfPrefix = "Wolf";

    private void Start()
    {
        spawnManager = GameObject.Find(SpawnManagerName)?.GetComponent<SpawnManager>();
        playerAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();

        if (spawnManager == null)
        {
            Debug.LogError($"SpawnManager not found. Ensure a GameObject named '{SpawnManagerName}' exists with the SpawnManager script attached.");
        }

        if (playerAudio == null)
        {
            Debug.LogError("AudioSource component not found. Ensure this GameObject has an AudioSource attached.");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found. Ensure this GameObject has a Rigidbody attached.");
        }
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
                // Do nothing for ground collision
                break;

            default:
                if (objectName.StartsWith(EnemyPrefix))
                {
                    HandleEnemyCollision(other);
                }
                else if (objectName.StartsWith(WolfPrefix))
                {
                    HandleWolfCollision();
                }
                else
                {
                    HandleDefaultCollision();
                }
                break;
        }
    }

    private void HandleFenceCollision(Collider other)
    {
        spawnManager?.UpdateScore(20);
        PlayAudioClip(success);
        Destroy(other.gameObject);
    }

    private void HandleEnemyCollision(Collider other)
    {
        // Play bounce audio
        PlayAudioClip(bounce);

        // Calculate the bounce direction
        Vector3 direction = (transform.position - other.transform.position).normalized;

        // Apply an impulse force for the bounce
        Vector3 bounceDirection = new Vector3(direction.x, 0.5f, direction.z).normalized;
        rb?.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);

        // Reset velocity after a short duration to stop the movement
        StartCoroutine(ResetVelocityAfterBounce());
    }

    private IEnumerator ResetVelocityAfterBounce()
    {
        // Wait for a brief moment to allow the bounce to take effect
        yield return new WaitForSeconds(0.1f);

        // Stop the Rigidbody's velocity to end the bounce effect
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }




    private void HandleWolfCollision()
    {
        PlayAudioClip(bark);
      //  TriggerGameOver();
    }

    private void HandleDefaultCollision()
    {
        PlayAudioClip(failure);
    //    TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }
        spawnManager?.EndGame();
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (playerAudio != null && clip != null)
        {
            playerAudio.PlayOneShot(clip, 1.0f);
        }
    }
}
