using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class EnemyAttack : NetworkBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 0.0f;
    [SerializeField] private float movementSpeed = 50.0f;

    private Transform playerTarget;
    private bool shouldRunAI;
    private SpawnManager spawnManager;


    private void Start()
    {
        // Determine if this instance should control the enemy AI
        spawnManager = GameObject.Find("SpawnManager")?.GetComponent<SpawnManager>();
        if (!spawnManager.isNetworkMultiplayer || IsServer)
        {
            shouldRunAI = true;
            AssignClosestPlayer();

            if (agent != null)
            {
                agent.speed = movementSpeed;
            }
        }
        else
        {
            shouldRunAI = false;
        }
    }

    private void Update()
    {
        if (!shouldRunAI) return;

        if (playerTarget == null || !IsPlayerAlive(playerTarget))
        {
            AssignClosestPlayer();
        }

        if (playerTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                StopAndAttack();
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }

    private void AssignClosestPlayer()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        if (playerObjects.Length > 0)
        {
            foreach (GameObject player in playerObjects)
            {
                if (IsPlayerAlive(player.transform))
                {
                    playerTarget = player.transform;
                    break;
                }
            }
        }
        else
        {
            //            Debug.LogWarning("No objects with the tag 'Player' were found.");
        }
    }

    private bool IsPlayerAlive(Transform player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        return playerHealth != null && playerHealth.IsAlive();
    }

    private void MoveTowardsPlayer()
    {
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);

                float currentSpeed = agent.velocity.magnitude;
                animator.speed = currentSpeed > 0 ? currentSpeed / agent.speed : 1f;
            }
        }
    }

    private void StopAndAttack()
    {
        agent.ResetPath();

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
        }

        // Add attack logic here
    }
}
