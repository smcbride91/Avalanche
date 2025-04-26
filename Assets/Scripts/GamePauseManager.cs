using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign in the Inspector
    private bool isPaused = false;

    // ADD THIS:
    public static bool isGameOver = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // New check: don't allow pausing/unpausing if the game is over
            if (isGameOver)
                return;

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    }

    // Optional: Call this when you restart the game!
    public static void ResetGameOver()
    {
        isGameOver = false;
    }
}
