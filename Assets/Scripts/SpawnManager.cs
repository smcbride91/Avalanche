using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SpawnManager : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public TextMeshProUGUI scoreText;

    private float spawnRangeXLeft = -22.2f;
    private float spawnRangeXRight = 18.83f;
    private int score;
    private float spawnPosZ = 26.33f;
    private float startDelay = 2;
    private float spawnInterval = 1.5f;
    private float spawnPosY = 9.77f;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnRandomObstacle", startDelay, spawnInterval);
        score = 0;
        scoreText.text = "Score: " + score;

    }

    // Update is called once per frame
    void Update()
    {


    }

    void SpawnRandomObstacle()
    {
        int obstacleIndex = Random.Range(0, obstaclePrefabs.Length);
        Vector3 spawnPos = new Vector3(Random.Range(spawnRangeXLeft, spawnRangeXRight), spawnPosY, spawnPosZ);

        Instantiate(obstaclePrefabs[obstacleIndex], spawnPos, obstaclePrefabs[obstacleIndex].transform.rotation);
        UpdateScore(5);

    }


    void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;

    }
}
