using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Netcode; // For NetworkObject spawning

public class SpawnManager : NetworkBehaviour
{
    [Header("Game Objects - Local")]
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Game Objects - Networked")]
    [SerializeField] private GameObject[] networkObstaclePrefabs;
    [SerializeField] private GameObject[] networkEnemyPrefabs;

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI p1ScoreText;
    [SerializeField] private TextMeshProUGUI p2ScoreText;
    public GameObject singleScoreUI;
    public GameObject multiScoreUI;
    public NetworkVariable<int> networkScore1 = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> networkScore2 = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> netPlayer1Dead = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> netPlayer2Dead = new NetworkVariable<bool>(false);

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
    private int deadPlayers;
    private bool listenersAdded = false;
    private bool gameInitialized = false;

    public bool isGameActive;
    public bool isLocalMultiplayer = false;
    public bool isNetworkMultiplayer = false;
    public bool isPlayer1Alive = true;
    public bool isPlayer2Alive = true;
    public Menu menu;

    private Dictionary<ulong, int> clientToPlayerNumber = new Dictionary<ulong, int>();


    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Persist through scenes
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        CancelInvoke("SpawnRandomObstacle");
        if (isNetworkMultiplayer && !NetworkManager.Singleton.IsServer)
        {
            if (networkScore1 != null) networkScore1.OnValueChanged -= OnNetworkScore1Changed;
            if (networkScore2 != null) networkScore2.OnValueChanged -= OnNetworkScore2Changed;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void Start()
    {
        InitializeGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeGame();
    }

    public void ResetManager()
    {
        score = 0;
        p1Score = 0;
        p2Score = 0;
        deadPlayers = 0;
        isPlayer1Alive = true;
        isPlayer2Alive = true;
        isGameActive = true;

        // Re-assign UI references (only needed if they get lost when changing scene)
        singleScoreUI = GameObject.Find("singleScore");
        multiScoreUI = GameObject.Find("multiScore");

        if (singleScoreUI != null)
            singleScoreUI.SetActive(false);
        if (multiScoreUI != null)
            multiScoreUI.SetActive(false);

        UpdateUI();
    }



    private void InitializeGame()
    {
        if (gameInitialized) return;
        gameInitialized = true;

        // Re-assign UI references in case of scene change
        singleScoreUI = GameObject.Find("singleScore");
        multiScoreUI = GameObject.Find("multiScore");

        p1ScoreText = GameObject.Find("p1Score")?.GetComponent<TextMeshProUGUI>();
        p2ScoreText = GameObject.Find("p2Score")?.GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("scoreText")?.GetComponent<TextMeshProUGUI>();
        highScoreText = GameObject.Find("highScoreText")?.GetComponent<TextMeshProUGUI>();

        // Standard init
        score = 0;
        p1Score = 0;
        p2Score = 0;
        deadPlayers = 0;

        if (!isLocalMultiplayer && !isNetworkMultiplayer)
        {
            highScore = PlayerPrefs.GetInt("HighScore", 0);
        }

        Time.timeScale = 1f;
        isGameActive = true;

        if (isNetworkMultiplayer && !NetworkManager.Singleton.IsServer && !listenersAdded)
        {
            networkScore1.OnValueChanged += OnNetworkScore1Changed;
            networkScore2.OnValueChanged += OnNetworkScore2Changed;
            listenersAdded = true;
        }


        UpdateUI();

//        CancelInvoke();
        if (!isNetworkMultiplayer)
        {
            InvokeRepeating(nameof(SpawnRandomObstacle), startDelay, spawnInterval);
            SpawnRandomEnemy();
        }
    }


    public void SpawnRandomObstacle()
    {
        if (!isGameActive) return;
        if (obstaclePrefabs.Length == 0)
        {
            Debug.LogError("Enemy prefabs array is empty!");
            return;
        }

        if (isNetworkMultiplayer)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (networkObstaclePrefabs.Length == 0)
                {
                    Debug.LogError("Network obstacle prefabs array is empty!");
                    return;
                }

                int index = Random.Range(0, networkObstaclePrefabs.Length);
                GameObject obstacle = Instantiate(networkObstaclePrefabs[index], GenerateSpawnPosition(), Quaternion.identity);
                obstacle.GetComponent<NetworkObject>()?.Spawn();
            }
        }
        else
        {
            int index = Random.Range(0, obstaclePrefabs.Length);
            Instantiate(obstaclePrefabs[index], GenerateSpawnPosition(), Quaternion.identity);
        }
    }

    public void SpawnRandomEnemy()
    {
        if (!isGameActive) return;
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("Enemy prefabs array is empty!");
            return;
        }
        if (isNetworkMultiplayer)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (networkEnemyPrefabs.Length == 0)
                {
                    Debug.LogError("Network enemy prefabs array is empty!");
                    return;
                }

                int index = Random.Range(0, networkEnemyPrefabs.Length);
                GameObject enemy = Instantiate(networkEnemyPrefabs[index], enemySpawnPosition, Quaternion.identity);
                enemy.GetComponent<NetworkObject>()?.Spawn();
            }
        }
        else
        {
            int index = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[index], enemySpawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, obstacleSpawnPosition.y, obstacleSpawnPosition.z);
    }

    public void UpdateScore(int points, int playerNumber)
    {
        if (!isGameActive) return;

        if (isNetworkMultiplayer && NetworkManager.Singleton.IsServer)
        {
            if (playerNumber == 1 && isPlayer1Alive) networkScore1.Value += points;
            if (playerNumber == 2 && isPlayer2Alive) networkScore2.Value += points;
        }
        else
        {
            if (playerNumber == 1 && isPlayer1Alive) p1Score += points;
            if (playerNumber == 2 && isPlayer2Alive) p2Score += points;

            if (!isLocalMultiplayer)
            {
                p1Score += points;
                score += points;
                CheckHighScore();
            }
        }

        UpdateUI();
    }

public void AwardSharedPoints(int points)
{
        if (!isGameActive) return;

        if (isNetworkMultiplayer && NetworkManager.Singleton.IsServer)
        { 
            if (isPlayer1Alive) networkScore1.Value += points;
            if (isPlayer2Alive) networkScore2.Value += points;
        }
    else if (isLocalMultiplayer)
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

        if ((isLocalMultiplayer || isNetworkMultiplayer) && deadPlayers >= 2)
        {
            EndGame();
        }
        else if (!isLocalMultiplayer && !isNetworkMultiplayer)
        {
            EndGame();
        }
    }

    private void CheckHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    private void UpdateUI()
    {
        if (isLocalMultiplayer || isNetworkMultiplayer)
        {

            int displayedP1Score = 0;
            int displayedP2Score = 0;

            if (isLocalMultiplayer)
            {
                displayedP1Score = p1Score;
                displayedP2Score = p2Score;
            }
            else if (isNetworkMultiplayer)
            {
                displayedP1Score = networkScore1.Value;
                displayedP2Score = networkScore2.Value;
            }

            if (p1ScoreText != null) p1ScoreText.text = $"P1 Score: {displayedP1Score}";
            if (p2ScoreText != null) p2ScoreText.text = $"P2 Score: {displayedP2Score}";
            if (multiScoreUI != null) multiScoreUI.SetActive(true);
            if (singleScoreUI != null) singleScoreUI.SetActive(false);
        }
        else
        {
            if (scoreText != null) scoreText.text = $"Score: {score}";
            if (highScoreText != null) highScoreText.text = $"High Score: {highScore}";
            if (singleScoreUI != null) singleScoreUI.SetActive(true);
            if (multiScoreUI != null) multiScoreUI.SetActive(false);
        }
    }

    public void EndGame()
    {
        isGameActive = false;
        CancelInvoke();
    }

    public void RestartGame()
    {
        StaticResetter.ResetStaticFlags(); // Your custom static reset helper
        CancelInvoke("SpawnRandomObstacle");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGameSingleLocal()
    {
        EndGame();
        SceneManager.LoadScene(0);
    }

    public void QuitGameOnline()
    {
        EndGame();
        menu.QuitMultiplayer();
    }

    public int GetPlayerScore(int playerNumber)
    {
        return playerNumber == 1 ? p1Score : playerNumber == 2 ? p2Score : 0;
    }

    private void OnNetworkScore1Changed(int oldValue, int newValue)
    {
        UpdateUI();

    }

    private void OnNetworkScore2Changed(int oldValue, int newValue)
    {
        UpdateUI();
    }

    public int RegisterPlayer(ulong clientId)
    {
        if (!clientToPlayerNumber.ContainsKey(clientId))
        {
            int assignedPlayerNumber = clientToPlayerNumber.Count + 1; // 1 for first, 2 for second
            clientToPlayerNumber.Add(clientId, assignedPlayerNumber);
            return assignedPlayerNumber;
        }

        return clientToPlayerNumber[clientId]; // Already registered
    }

    public int GetPlayerNumberByClientId(ulong clientId)
    {
        return clientToPlayerNumber.TryGetValue(clientId, out var playerNum) ? playerNum : 0;
    }

    public void SetNetPlayerDead(int playerNum)
    {
        if (playerNum == 1)
            netPlayer1Dead.Value = true;
        else if (playerNum == 2)
            netPlayer2Dead.Value = true;
    }

}
