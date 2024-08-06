using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public GameObject[] characters;
    public CharacterData[] characterDataSOs;
    public int playerIndex;
    public int demonIndex;

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
                if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Demon)
                {
                    demonIndex = i;
                }
            }
            else
            {
                Debug.LogWarning("CharacterProfile component is missing on GameObject " + characters[i].name);
            }
        }
    }

}
