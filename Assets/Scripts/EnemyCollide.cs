using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollide : MonoBehaviour
{
    private Spawn spawn;

    // Start is called before the first frame update
    void Start()
    {
        spawn = GameObject.Find("SpawnManager").GetComponent<Spawn>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            // do nothing
        }
        else if (other.gameObject.tag == "Obstacle")
        {
            spawn.SpawnRandomEnemy();
            Destroy(this.gameObject);
        }
    }
}
