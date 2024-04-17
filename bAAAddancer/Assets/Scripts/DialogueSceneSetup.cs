using UnityEngine;

public class DialogueSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        foreach (GameObject character in characterManager.characters) 
        {
            character.SetActive(true);
        }
    }
}
