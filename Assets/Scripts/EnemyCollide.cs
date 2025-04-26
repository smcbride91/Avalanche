using UnityEngine;

public class EnemyCollide : MonoBehaviour
{
    private SpawnManager spawn;
    public AudioClip shock;
    private AudioSource enemyAudio;

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
            if (!GamePauseManager.isGameOver)
            {
                spawn.AwardSharedPoints(20);
            }

        }
    }
}