using UnityEngine;

public class DebugSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        for (int i = 0; i < characterManager.characters.Length; i++)
        {
            GameObject character = characterManager.characters[i];
            CharacterData characterData = characterManager.characterDataSOs[i];
            character.SetActive(false);

            if (characterData.infectionLevel > 0) 
            { 
                character.SetActive(true);
            }
        }
    }
}
