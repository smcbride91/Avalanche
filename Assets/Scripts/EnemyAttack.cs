using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private NavMeshAgent agent;

    private Transform playerTarget;

    private void Start()
    {
        AssignClosestPlayer();
    }

    private void Update()
    {
        MoveTowardsPlayer();
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
        }
    }
}
