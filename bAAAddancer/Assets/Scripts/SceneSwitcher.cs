using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    private void Update() // only need this whil debugging
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            SwitchToNextLevelKey();
            LoadNextScene();
        }
    }
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
    public void LoadSceneByIndex(int sceneIndex)
    {
        // Kill all tweens associated with GameObjects
        DOTween.KillAll();

        SceneManager.LoadScene(sceneIndex);
    }
    public void SwitchToNextLevelKey()
    {
        LevelKey currentLevelKey = GameManager.Instance.GetCurrentLevelKey();
        int currentLevelIndex = (int)currentLevelKey;
        int nextLevelIndex = (currentLevelIndex + 1) % Enum.GetNames(typeof(LevelKey)).Length;
        LevelKey nextLevel = (LevelKey)nextLevelIndex;
        GameManager.Instance.SetCurrentLevelKey(nextLevel);
    }
    public void SwitchToNextRound() 
    {
        GameManager.Instance.NewRound();
    }
}
