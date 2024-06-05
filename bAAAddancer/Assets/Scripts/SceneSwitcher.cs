using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string GetCurrentSceneName()
    {
        // Get the name of the current active scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName;
    }

    public void LoadNextScene()
    {
        // Kill all tweens associated with GameObjects
        DOTween.KillAll();

        // Get the index of the current active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the next scene by incrementing the current scene index
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    public void LoadSceneByName(string sceneName) //*** dont like using strings for switches, but this should be fine
    {
        // Kill all tweens associated with GameObjects
        DOTween.KillAll();

        SceneManager.LoadScene(sceneName);
    }
    public void SwitchToNextLevelKey()
    {
        LevelKey currentLevelKey = GameManager.Instance.GetCurrentLevelKey();
        int currentLevelIndex = (int)currentLevelKey;
        int nextLevelIndex = (currentLevelIndex + 1) % Enum.GetNames(typeof(LevelKey)).Length;
        LevelKey nextLevel = (LevelKey)nextLevelIndex;
        GameManager.Instance.SetCurrentLevelKey(nextLevel);
    }
    public void SwitchToNextMakeDanceLevel() 
    {
        int currentRound = GameManager.Instance.GetCurrentRound();
        if (currentRound == -1)
        {
            GameManager.Instance.SetCurrentLevelKey(LevelKey.IntroMakeDance);
        }
        if (currentRound == 0) 
        {
            GameManager.Instance.SetCurrentLevelKey(LevelKey.IntroMakeDance);
        }
        if (currentRound == 1)
        {
            GameManager.Instance.SetCurrentLevelKey(LevelKey.Round2MakeDance);
        }
        if (currentRound == 2)
        {
            GameManager.Instance.SetCurrentLevelKey(LevelKey.Round3MakeDance);
        }
        if (currentRound == 3)
        {
            GameManager.Instance.SetCurrentLevelKey(LevelKey.Round4MakeDance);
        }
    }
    public void SwitchToNextCopyDanceLevel()
    {
        int currentRound = GameManager.Instance.GetCurrentRound();
        if (currentRound == 0)
        {
            GameManager.Instance.SetCurrentLevelKey(LevelKey.Round2CopyDance);
        }
        if (currentRound == 1)
        {
            GameManager.Instance.SetCurrentLevelKey(LevelKey.Round3CopyDance);
        }
        if (currentRound == 2)
        {
            GameManager.Instance.SetCurrentLevelKey(LevelKey.Round4CopyDance);
        }
    }
}
