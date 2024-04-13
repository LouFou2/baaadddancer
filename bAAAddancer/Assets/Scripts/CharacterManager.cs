using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characters;
    private CharacterData[] characterDataSOs;
    
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
        
    }

    //Add methods for accessing character Data
}
