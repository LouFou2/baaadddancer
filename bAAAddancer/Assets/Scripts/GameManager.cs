using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Reference to the GameManagerData scriptable object
    public GameManagerData gameData;

    // Singleton instance of the GameManager
    public static GameManager Instance { get; private set; }

    private LevelKey currentLevelKey;
    private bool gameStarted = false;

    private void Awake()
    {
        // Ensure only one instance of the GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Preserve the GameManager across scene changes
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate instances
        }
    }
    private void Start()
    {
        currentLevelKey = gameData.currentLevelKey;
        if (!gameStarted && SceneManager.GetActiveScene().buildIndex == 0)
        {
            // If this is the first loading of the game, reset the currentLevelKey
            ResetCurrentLevelKey();
            gameData.currentRound = -1; // it will get set to 0 in the first dialogue scene
            gameStarted = true;
        }
    }
    private void ResetCurrentLevelKey()
    {
        currentLevelKey = (LevelKey)0; // Set default level key to first index (0)
        gameData.currentLevelKey = currentLevelKey;
    }

    // Method to set the current level key
    public void SetCurrentLevelKey(LevelKey levelKey)
    {
        currentLevelKey = levelKey;
        gameData.currentLevelKey = levelKey;
    }

    // Method to get the current level key
    public LevelKey GetCurrentLevelKey()
    {
        currentLevelKey = gameData.currentLevelKey;
        return currentLevelKey;
    }
    public void SwitchToNextLevelKey()
    {
        //how do we return the next index levelkey?
        int currentLevelIndex = (int)currentLevelKey;
        int nextLevelIndex = (currentLevelIndex + 1) % Enum.GetNames(typeof(LevelKey)).Length;
        LevelKey nextLevel = (LevelKey)nextLevelIndex;
        SetCurrentLevelKey(nextLevel);
    }
    public int GetCurrentRound() 
    {
        return gameData.currentRound;
    }
    public void NewRound() 
    {
        gameData.currentRound += 1;
    }
}
