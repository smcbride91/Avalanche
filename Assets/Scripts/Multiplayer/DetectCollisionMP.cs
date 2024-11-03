using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class DetectCollisionMP : NetworkBehaviour
{
    //public TextMeshProUGUI gameOverText;
    private SpawnMP spawnMP;
    public AudioClip success;
    public AudioClip failure;
    private AudioSource playerAudio;

    // Start is called before the first frame update
    
    void Start()
    {
        spawnMP = GameObject.Find("SpawnManager").GetComponent<SpawnMP>();
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
                // check which player
                if (IsServer)
                {
                    spawnMP.bonusScore(2, false);
                }
                else
                {
                    spawnMP.bonusScore(2, true);
                }

                playerAudio.PlayOneShot(success,1.0f);
                
            //    Destroy(other.gameObject); 

            }
            else if (other.name =="Player(Clone)")
            {
                //
            }
            else
            {
                playerAudio.PlayOneShot(failure, 1.0f);
  //              gameOverText.gameObject.SetActive(true);
                spawnMP.setFail();
            }
        }

    }
}
