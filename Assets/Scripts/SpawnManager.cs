using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public TextMeshProUGUI scoreText;

    private float spawnRangeXLeft = -25.0f;
    private float spawnRangeXRight = 10.0f;
    private int score;
    private int scoreIncrease = 5;
    private float spawnPosZ = 23.0f;
    private float startDelay = 2;
    private float spawnInterval = 1.5f;
    private float spawnPosY = -7.36f;
    private float increaseSpeedDelay;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnRandomObstacle", startDelay, spawnInterval);
        increaseSpeedDelay= startDelay * 2;
        InvokeRepeating ("IncreaseSpeed",increaseSpeedDelay,increaseSpeedDelay);
        score = 0;
        //scoreText.text = "Score: " + Convert.ToString(score);
        //scoreText.text = "Score: " + score;

    }

    // Update is called once per frame
    void Update()
    {


    }

    void SpawnRandomObstacle()
    {
        //int obstacleIndex = UnityEngine.Random.Range(0, obstaclePrefabs.Length);
        int obstacleIndex = UnityEngine.Random.Range(0, 6);
        Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(spawnRangeXLeft, spawnRangeXRight), spawnPosY, spawnPosZ);

        Instantiate(obstaclePrefabs[obstacleIndex], spawnPos, obstaclePrefabs[obstacleIndex].transform.rotation);
        UpdateScore(scoreIncrease);

    }

    void IncreaseSpeed()
    {
        spawnInterval = spawnInterval * 2;
        Console.WriteLine("SpawnInterval " + Convert.ToString(spawnInterval));
    }


    void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        //scoreText.text = "Score: " + score;

    }
}
