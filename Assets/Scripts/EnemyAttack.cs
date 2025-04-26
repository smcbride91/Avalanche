using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 0.0f; // Adjusted default attack range
    [SerializeField] private float movementSpeed = 50.0f; // Custom movement speed for the enemy

    private Transform playerTarget;

    private void Start()
    {
        AssignClosestPlayer();

        // Set the NavMeshAgent speed explicitly
        if (agent != null)
        {
            agent.speed = movementSpeed; // Ensure the NavMeshAgent uses the desired speed
        }
    }

    private void Update()
    {
        // If the current player target is dead, assign a new target
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

    /// <summary>
    /// Finds the closest player and assigns it as the target.
    /// </summary>
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
                    break; // Assign the first alive player as the target
                }
            }
        }
        else
        {
            Debug.LogWarning("No objects with the tag 'Player' were found.");
        }
    }

    /// <summary>
    /// Checks if a player is alive.
    /// </summary>
    private bool IsPlayerAlive(Transform player)
    {
        // This function assumes you have a script with a 'isAlive' flag or similar on your player objects.
        // Replace with your actual method of checking if the player is alive.
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        return playerHealth != null && playerHealth.IsAlive();
    }

    /// <summary>
    /// Moves the enemy towards the assigned player target if available.
    /// </summary>
    private void MoveTowardsPlayer()
    {
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);

            // Trigger the walking animation
            if (animator != null)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);

                // Sync animation speed with NavMeshAgent speed
                float currentSpeed = agent.velocity.magnitude;
                animator.speed = currentSpeed > 0 ? currentSpeed / agent.speed : 1f;
            }
        }
    }

    /// <summary>
    /// Stops the enemy and triggers the attack animation.
    /// </summary>
    private void StopAndAttack()
    {
        agent.ResetPath(); // Stop the agent from moving

        // Trigger the attack animation
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
        }

        // Implement your attack logic here (e.g., dealing damage to the player)
    }
}
