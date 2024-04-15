using UnityEngine;

public class MakeDanceSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        foreach (GameObject character in characterManager.characters) 
        {
            CharacterProfile characterProfile = character.GetComponent<CharacterProfile>();
            CopyProxyRig copyProxyRig = character.GetComponent<CopyProxyRig>();

            if (characterProfile.characterDataSO.characterRoleSelect != CharacterData.CharacterRole.Player)
            {
                character.SetActive(false);
                copyProxyRig.enabled = false;
            }
            else 
            {
                character.SetActive(true);
                copyProxyRig.enabled = true;
                Debug.Log(character.name);
            }
        }
    }
    void Update()
    {
        
    }
}
