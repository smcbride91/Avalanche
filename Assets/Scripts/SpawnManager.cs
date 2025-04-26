using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI p1ScoreText;
    [SerializeField] private TextMeshProUGUI p2ScoreText;
    public GameObject singleScoreUI;
    public GameObject multiScoreUI;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRange = 20.0f;
    [SerializeField] private Vector3 obstacleSpawnPosition = new Vector3(0, 0.32f, 40.23f);
    [SerializeField] private Vector3 enemySpawnPosition = new Vector3(0, 0.6f, 20f);
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float spawnInterval = 1.5f;

    private int score;
    private int p1Score;
    private int p2Score;
    private int highScore;
    public bool isGameActive;
    public bool isMultiplayer = false;
    public bool isPlayer1Alive = true;
    public bool isPlayer2Alive = true;
    private int deadPlayers = 0;  // Track the number of dead players

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
 //       PlayerPrefs.DeleteAll();
 //       PlayerPrefs.Save();
        score = 0;
        p1Score = 0;
        p2Score = 0;

        if (!isMultiplayer)
        {
            highScore = PlayerPrefs.GetInt("HighScore", 0);
        }

        Time.timeScale = 1f;
        isGameActive = true;

        UpdateUI();

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

    public void UpdateScore(int points, int playerNumber)
    {
        if (!isGameActive) return;

        if (playerNumber == 1 && isPlayer1Alive) p1Score += points;
        if (playerNumber == 2 && isPlayer2Alive) p2Score += points;

        if (!isMultiplayer)
        {
            score = p1Score; // Assuming player 1 is the single player!
            CheckHighScore();
        }

        UpdateUI();
    }

    public void AwardSharedPoints(int points)
    {
        if (!isGameActive) return;

        if (isMultiplayer)
        {
            if (isPlayer1Alive) p1Score += points;
            if (isPlayer2Alive) p2Score += points;
        }
        else
        {
            score += points;
            CheckHighScore();
        }

        UpdateUI();
    }

    public void PlayerDied(int playerNumber)
    {
        deadPlayers++;

        if (isMultiplayer)
        {
            if (deadPlayers >= 2)
            {
                EndGame();  // End the game only when both players are dead in multiplayer
            }
        }
        else
        {
            EndGame();  // Immediately end game in single-player when the one player dies
        }
    }

    private void CheckHighScore()
    {
        if (score <= highScore) return;

        highScore = score;
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        if (isMultiplayer)
        {
            p1ScoreText.text = $"P1 Score: {p1Score}";
            p2ScoreText.text = $"P2 Score: {p2Score}";
            multiScoreUI.SetActive(true);
        }
        else
        {
            scoreText.text = $"Score: {score}";
            highScoreText.text = $"High Score: {highScore}";
            singleScoreUI.SetActive(true);
        }
    }

    public void EndGame()
    {
        isGameActive = false;
    }

    public void RestartGame()
    {
        StaticResetter.ResetStaticFlags();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // New generalized GetPlayerScore method:
    public int GetPlayerScore(int playerNumber)
    {
        if (playerNumber == 1)
        {
            return p1Score;
        }
        else if (playerNumber == 2)
        {
            return p2Score;
        }
        else
        {
            return 0; // If an invalid player number is passed
        }
    }
}
