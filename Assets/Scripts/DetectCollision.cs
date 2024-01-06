using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name != "Player_Snow")
        {
            Destroy(gameObject);
            //Destroy(other.gameObject);
            // Debug.Log("Game over");
            SceneManager.LoadScene("GameOver");
        }

    }
}
