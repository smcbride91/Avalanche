using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DestroyOutOfBoundsMP : NetworkBehaviour
{
    private float lowerBound = -14.0f;
    private SpawnMP spawnMP;

    // Start is called before the first frame update
    void Start()
    {
        spawnMP = GameObject.Find("SpawnManager").GetComponent<SpawnMP>();
    }

    // Update is called once per frame
    void Update()
    {


       if (transform.position.z < lowerBound)
        {
            if (IsServer)
            {
                Destroy(gameObject);
            }
            spawnMP.updateScore(5);
        }
    }
}
