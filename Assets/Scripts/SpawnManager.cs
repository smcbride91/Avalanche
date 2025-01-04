using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRange = 20.0f;
    [SerializeField] private Vector3 obstacleSpawnPosition = new Vector3(0, 0.32f, 40.23f);
    [SerializeField] private Vector3 enemySpawnPosition = new Vector3(0, 0.6f, 20f);
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float spawnInterval = 1.5f;

    private int score;
    private int highScore;
    public bool isGameActive;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        score = 0;
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        isGameActive = true;

        UpdateUI();
        restartButton.gameObject.SetActive(false);

        InvokeRepeating(nameof(SpawnRandomObstacle), startDelay, spawnInterval);
        SpawnRandomEnemy();
    }

    private void SpawnRandomObstacle()
    {
        if (!isGameActive) return;

        int index = Random.Range(0, obstaclePrefabs.Length);
        Instantiate(obstaclePrefabs[index], GenerateSpawnPosition(), Quaternion.identity);
    }

    public void SpawnRandomEnemy()
    {
        if (!isGameActive) return;

        int index = Random.Range(0, enemyPrefabs.Length);
        Instantiate(enemyPrefabs[index], enemySpawnPosition, Quaternion.identity);
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, obstacleSpawnPosition.y, obstacleSpawnPosition.z);
    }

    public void UpdateScore(int points)
    {
        if (!isGameActive) return;

        score += points;
        CheckHighScore();
        UpdateUI();
    }

    private void CheckHighScore()
    {
        if (score <= highScore) return;

        highScore = score;
        PlayerPrefs.SetInt("HighScore", highScore);
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
        highScoreText.text = $"High Score: {highScore}";
    }

    public void EndGame()
    {
        isGameActive = false;
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
