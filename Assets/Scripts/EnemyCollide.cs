using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollide : MonoBehaviour
{
    private SpawnManager spawn;
    public AudioClip shock;
    private AudioSource enemyAudio;

    // Start is called before the first frame update
    void Start()
    {
        spawn = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        enemyAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            enemyAudio.PlayOneShot(shock);
            spawn.SpawnRandomEnemy();
            Destroy(this.gameObject);
            spawn.UpdateScore(20);
        }
    }
}