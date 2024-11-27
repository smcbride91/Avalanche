using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public NavMeshAgent agent;
    //public NavMeshAgent player;
    private GameObject[] playerObjects;


    void Start()
    {
    playerObjects = GameObject.FindGameObjectsWithTag("Player");

    }
    // Update is called once per frame
    void Update()
    {
        if (playerObjects.Length > 0)
            {
            agent.SetDestination(playerObjects[0].transform.position);
        }
    }
}
