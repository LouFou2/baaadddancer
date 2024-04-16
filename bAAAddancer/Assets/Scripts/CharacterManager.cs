using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public GameObject[] characters;
    public CharacterData[] characterDataSOs;
    public int playerIndex;
    public int bugIndex;

    // We can use the current scene to activate/de-activate components on character as needed **MIGHT REMOVE SCENE INFO RELATED LOGIC
    /*private enum CurrentScene { TitleScene, DialogueScene, MakeDanceScene, CopyDanceScene, DebugScene } //add more if needed
    private CurrentScene currentScene;*/
    
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
                if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Player) 
                {
                    playerIndex = i;
                }
                if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Bug)
                {
                    bugIndex = i;
                }
            }
            else
            {
                Debug.LogWarning("CharacterProfile component is missing on GameObject " + characters[i].name);
            }
        }

    }

    void Update()
    {
        /*string currentSceneName = SceneManager.GetActiveScene().name;

        // Make sure to add scene names accurately:
        if (currentSceneName == "TitleScene") { currentScene = CurrentScene.TitleScene; }
        else if (currentSceneName == "") { currentScene = CurrentScene.DialogueScene; }
        else if (currentSceneName == "") { currentScene = CurrentScene.MakeDanceScene; }
        else if (currentSceneName == "") { currentScene = CurrentScene.CopyDanceScene; }
        else if (currentSceneName == "") { currentScene = CurrentScene.DebugScene; }*/

    }
    
    //Add methods for accessing character Data
}
