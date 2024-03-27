using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Reference to the GameManagerData scriptable object
    public GameManagerData gameData;

    // Singleton instance of the GameManager
    public static GameManager Instance { get; private set; }

    /*[System.Serializable]
    public enum LevelKey
    {
        IntroDialogue,
        IntroMakeDance,
        IntroCopyDance,
        IntroDebug
    }*/
    private LevelKey currentLevelKey;

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

    // Method to set the current level key
    public void SetCurrentLevelKey(LevelKey levelKey)
    {
        currentLevelKey = levelKey;
        gameData.currentLevelKey = levelKey;
    }

    // Method to get the current level key
    public LevelKey GetCurrentLevelKey()
    {
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
}
