using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadNextScene()
    {
        // Kill all tweens associated with GameObjects
        DOTween.KillAll();

        // Get the index of the current active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the next scene by incrementing the current scene index
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
