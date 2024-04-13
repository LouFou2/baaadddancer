using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public GameObject[] characters;
    public CharacterData[] characterDataSOs;

    private enum CurrentScene { TitleScene, DialogueScene, MakeDanceScene, CopyDanceScene, DebugScene } //add more if needed
    private CurrentScene currentScene;
    
    void Start()
    {
        // Initialize characterDataSOs array with the same length as characters array
        characterDataSOs = new CharacterData[characters.Length];

        for (int i = 0; i < characters.Length; i++)
        {
            CharacterProfile characterProfile = characters[i].GetComponent<CharacterProfile>();

            if (characterProfile != null)
            {
                 characterDataSOs[i] = characterProfile.characterDataSO;
            }
            else
            {
                Debug.LogWarning("CharacterProfile component is missing on GameObject " + characters[i].name);
            }
        }

    }

    void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Make sure to add scene names accurately:
        if (currentSceneName == "TitleScene") { currentScene = CurrentScene.TitleScene; }
        else if (currentSceneName == "") { currentScene = CurrentScene.DialogueScene; }
        else if (currentSceneName == "") { currentScene = CurrentScene.MakeDanceScene; }
        else if (currentSceneName == "") { currentScene = CurrentScene.CopyDanceScene; }
        else if (currentSceneName == "") { currentScene = CurrentScene.DebugScene; }

    }
    public void SetPlayerCharacter() 
    {
    }
    //Add methods for accessing character Data
}
