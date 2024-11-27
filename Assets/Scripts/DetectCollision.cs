using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectCollision : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;
    private Rigidbody rb;
    private Spawn spawn;
    public AudioClip success;
    public AudioClip failure;
    public AudioClip bounce;
    public AudioClip bark;
    private AudioSource playerAudio;

    [SerializeField] float bounceForce;

    // Start is called before the first frame update
    
    void Start()
    {
        spawn = GameObject.Find("SpawnManager").GetComponent<Spawn>();
        playerAudio = GetComponent<AudioSource>();
        this.rb = GetComponent<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other)
    {
        {
            if (other.name == "SM_Prop_Fence_02(Clone)")
            {
                spawn.updateScore(20);
                playerAudio.PlayOneShot(success, 1.0f);
                Destroy(other.gameObject);
            }
            else
            if (other.name == "Ground")
            {
                // don't do anything
            }
            else if (other.name.StartsWith("Enemy"))
            {
                playerAudio.PlayOneShot(bounce, 1.0f);
                Vector3 dir = other.transform.position - this.transform.position;
                // dir.y = 0;
                dir = dir.normalized;
                rb.AddForce(dir * bounceForce);
                //rb.AddExplosionForce(bounceForce, other.transform.position, 20);
            }

            else if (other.name.StartsWith("Wolf"))
            {
                playerAudio.PlayOneShot(bark, 1.0f);
                gameOverText.gameObject.SetActive(true);
                Destroy(other.gameObject);
                spawn.setFail();

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
