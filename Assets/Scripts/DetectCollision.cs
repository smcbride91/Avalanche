using System;
using TMPro;
using UnityEngine;

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
        PlayAudioClip(bounce);

        Vector3 direction = (other.transform.position - transform.position).normalized;
        rb?.AddForce(direction * bounceForce);
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
