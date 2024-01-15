using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private float lowerBound = -14.0f;
    private Spawn spawn;

    // Start is called before the first frame update
    void Start()
    {
        spawn = GameObject.Find("SpawnManager").GetComponent<Spawn>();
    }

    // Update is called once per frame
    void Update()
    {


       if (transform.position.z < lowerBound)
        {
            Destroy(gameObject);
            spawn.updateScore(5);
        }
    }
}
