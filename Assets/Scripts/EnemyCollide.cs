using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollide : MonoBehaviour
{
    private Spawn spawn;
    public AudioClip shock;
    private AudioSource playerAudio;

    // Start is called before the first frame update
    void Start()
    {
        spawn = GameObject.Find("SpawnManager").GetComponent<Spawn>();
        playerAudio = GetComponent<AudioSource>();
    }



    private void OnTriggerEnter(Collider other)
    {
    if (other.gameObject.tag == "Obstacle")
        {
              playerAudio.PlayOneShot(shock);
//            AudioSource.PlayClipAtPoint(shock, transform.position);
            spawn.SpawnRandomEnemy();
            Destroy(this.gameObject);
            spawn.updateScore(20);
        }
    }
}
