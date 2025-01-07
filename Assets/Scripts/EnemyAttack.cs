using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f; // Range at which the enemy attacks
    private Transform playerTarget;

    private void Start()
    {
        AssignClosestPlayer();
    }

    private void Update()
    {
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
            // For simplicity, assign the first player found. 
            // Could be expanded to calculate the closest player.
            playerTarget = playerObjects[0].transform;
        }
        else
        {
            Debug.LogWarning("No objects with the tag 'Player' were found.");
        }
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
