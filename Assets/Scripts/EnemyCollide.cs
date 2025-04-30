using UnityEngine;
using Unity.Netcode;

public class EnemyCollide : NetworkBehaviour
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
            if (!spawn.isNetworkMultiplayer)
            {
                Destroy(this.gameObject);
            }
            else if (NetworkManager.Singleton.IsServer)
            {
                GetComponent<NetworkObject>().Despawn(true);
            }
            if (!GamePauseManager.isGameOver)
            {
                if (!spawn.isNetworkMultiplayer || IsServer)
                {
                    spawn.AwardSharedPoints(20);
                }
            }

        }
    }
}