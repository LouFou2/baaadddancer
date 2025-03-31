using System.Collections.Generic;
using UnityEngine;

public class InfectionManager : MonoBehaviour
{
    private CharacterManager characterManager;
    
    void Start()
    {
        // Get reference to CharacterManager
        characterManager = FindObjectOfType<CharacterManager>();

        if (characterManager == null)
        {
            Debug.LogError("CharacterManager not found!");
            return;
        }

        // Generate a list of eligible characters (excluding player and bug characters)
        List<CharacterData> eligibleCharacters = new List<CharacterData>();

        // Temporary list to track characters that have had their infection level increased
        List<CharacterData> infectedCharacters = new List<CharacterData>();

        //store a float to calculate average infection level
        float averageInfection = 0;

        foreach (CharacterData characterData in characterManager.characterDataSOs)
        {
            characterData.lastCursedCharacter = false;

            if (characterData != null 
                && characterData.characterRoleSelect != CharacterData.CharacterRole.Player 
                /*&& characterData.characterRoleSelect != CharacterData.CharacterRole.Demon*/
                && !characterData.wasDebuggedLastRound)
            {
                eligibleCharacters.Add(characterData);
            }
            //increase current infection level
            if(characterData.infectionLevel > 0  && !infectedCharacters.Contains(characterData) )
            {
                characterData.infectionLevel += Random.Range(0.1f, 0.25f);
                averageInfection += characterData.infectionLevel;

                // Add character to the list of processed characters
                infectedCharacters.Add(characterData);
            }
        }

        // Check if there are eligible characters to infect
        if (eligibleCharacters.Count == 0)
        {
            Debug.LogWarning("No eligible characters to infect!");
            return;
        }

        // Select a random character from the eligible list
        int randomIndex = Random.Range(0, eligibleCharacters.Count);
        CharacterData selectedCharacter = eligibleCharacters[randomIndex];

        // Infect the selected character
        selectedCharacter.infectionLevel += 0.25f;
        averageInfection += 0.25f;
        infectedCharacters.Add(selectedCharacter);

        selectedCharacter.lastCursedCharacter = true;

        foreach (CharacterData characterData in characterManager.characterDataSOs)
        {
            if (characterData.infectionLevel > 1) characterData.infectionLevel = 1; //clamp at max 1

            // INFECTING THE PLAYER
            if (characterData != null && characterData.characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                //Player gets average of all chars' infections
                averageInfection /= infectedCharacters.Count; // average "infection level" of all infected characters 
                characterData.infectionLevel = averageInfection;
            }
        }
    }

}
