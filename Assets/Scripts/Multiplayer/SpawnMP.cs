using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class SpawnMP : NetworkBehaviour
{
    public GameObject[] obstaclePrefabs;
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    private int p1Score;
    private int p2Score;
    private float spawnRange = 20.0f;
    private float spawnPosZ = 40.23f;
    private float spawnPosY = 0f;
    private float startDelay = 2;
    private float spawnInterval = 1.5f;
    public bool isGameActive;
    public bool isP1Active;
    public bool isP2Active;
    public bool playerDeath;

    // Start is called before the first frame update
    void Start()
    {
        p1Score = 0;
        p2Score = 0;

        setActive();
        updateScore(0);
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InvokeSpawn()
    {
        InvokeRepeating("SpawnRandomObstacle", startDelay, spawnInterval);
    }

    void SpawnRandomObstacle()
    {
        if (isGameActive == true)
        {
          //  Instantiate(spawnedObjectPrefab);
            int obstacleIndex = Random.Range(1, obstaclePrefabs.Length);
            GameObject spawnedObjectTransform = Instantiate(obstaclePrefabs[obstacleIndex], GenerateSpawnPosition(), obstaclePrefabs[obstacleIndex].transform.rotation);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);

        return randomPos;

    }
    public void updateScore(int scoreToAdd)
    {
        if (isGameActive == true)
        {
            if (isP1Active == true)
            {
                p1Score += scoreToAdd;
                p1ScoreText.text = "P1 Score: " + p1Score;
            }
            if (isP2Active == true)
            {
                p2Score += scoreToAdd;
                p2ScoreText.text = "P2 Score: " + p2Score;
            }
        }
    }

    public void bonusScore(int scoreToAdd, bool secondPlayer)
    {
        if (isGameActive == true)
        {
            if (secondPlayer == false)
            {
                p1Score += scoreToAdd;
                p1ScoreText.text = "P1 Score: " + p1Score;
            }
            else
            {
                p2Score += scoreToAdd;
                p2ScoreText.text = "P2 Score: " + p2Score;
            }
        }
    }

    public void setActive()
    {
        isGameActive = true;
        isP1Active = true;
        isP2Active = true;
        playerDeath = false;
    }

    public void setFail()
    {
        isGameActive = false;
        playerDeath = true;
    }
    
}
