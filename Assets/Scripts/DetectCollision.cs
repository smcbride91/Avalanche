using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectCollision : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    private Spawn spawn;
    public AudioClip success;
    public AudioClip failure;
    private AudioSource playerAudio;

    // Start is called before the first frame update
    
    void Start()
    {
        spawn = GameObject.Find("SpawnManager").GetComponent<Spawn>();
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        {
            if (other.name == "SM_Prop_Fence_02(Clone)")
            {
                spawn.updateScore(20);
                playerAudio.PlayOneShot(success,1.0f);
                Destroy(other.gameObject); 
            }
            else
            {
                playerAudio.PlayOneShot(failure, 1.0f);
                gameOverText.gameObject.SetActive(true);
                spawn.setFail();
            }
        }

    }
}
