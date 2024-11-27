using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Spawn : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public GameObject[] enemyPrefabs;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Button restartButton;
    private int score;
    private int highScore;
    private float spawnRange = 20.0f;
    private float spawnPosZ = 40.23f;
    private float spawnPosY = 0.32f;
    private float startDelay = 2;
    private float spawnInterval = 1.5f;
    public bool isGameActive;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore",0);
        setActive();
        updateScore(score);
        InvokeRepeating("SpawnRandomObstacle", startDelay, spawnInterval);
        //InvokeRepeating("SpawnRandomEnemy", startDelay, spawnInterval);
        SpawnRandomEnemy();

    }

    // Update is called once per frame
    void Update()
    {

    }


    void SpawnRandomObstacle()
    {
        if (isGameActive == true)
        {
            int obstacleIndex = Random.Range(0, obstaclePrefabs.Length);
            Instantiate(obstaclePrefabs[obstacleIndex], GenerateSpawnPosition(), obstaclePrefabs[obstacleIndex].transform.rotation);

        }
    }

    public void SpawnRandomEnemy()
    {
        if (isGameActive == true)
        {
            int enemyIndex = Random.Range(0, enemyPrefabs.Length);
            Vector3 enemyPos = new Vector3(0f, 0.6f, 20f);
            Instantiate(enemyPrefabs[enemyIndex], enemyPos, enemyPrefabs[enemyIndex].transform.rotation);

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
            score += scoreToAdd;
            checkHighScore();
            scoreText.text = "Score: " + score;
            highScoreText.text = "High Score: " + highScore;
        }
    }

    public void setActive()
    {
        isGameActive = true;
    }
    public void setFail()
    {
        isGameActive = false;
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void checkHighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore",0))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScore = score;
        }
    }
}
