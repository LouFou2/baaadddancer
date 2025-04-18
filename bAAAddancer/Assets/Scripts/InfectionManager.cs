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

        /*// Generate a list of eligible characters (excluding player and bug characters)
        List<CharacterData> eligibleCharacters = new List<CharacterData>();*/

        // Temporary list to track characters that have had their infection level increased
        List<CharacterData> infectedCharacters = new List<CharacterData>();

        //store a float to calculate average infection level
        float averageInfection = 0;

        foreach (CharacterData characterData in characterManager.characterDataSOs)
        {
            characterData.lastCursedCharacter = false;

            /*if (characterData != null 
                && characterData.characterRoleSelect != CharacterData.CharacterRole.Player 
                *//*&& characterData.characterRoleSelect != CharacterData.CharacterRole.Demon*//*
                && !characterData.wasDebuggedLastRound)
            {
                eligibleCharacters.Add(characterData);
            }*/

            //increase current infection level
            if(characterData.infectionLevel > 0  && !infectedCharacters.Contains(characterData) )
            {
                characterData.infectionLevel += Random.Range(0.1f, 0.25f);
                averageInfection += characterData.infectionLevel;

                // Add character to the list of processed characters
                infectedCharacters.Add(characterData);
            }
        }

        /*// Check if there are eligible characters to infect
        if (eligibleCharacters.Count == 0)
        {
            Debug.LogWarning("No eligible characters to infect!");
            return;
        }*/

        /*// Select a random character from the eligible list
        int randomIndex = Random.Range(0, eligibleCharacters.Count);*/  // *** THE OLD SYSTEM DID A RANDOM PICK, I AM GOING TO DO MANUAL SELECTION:

        // *** Manual Selection ***
        int roundIndex = GameManager.Instance.GetCurrentRound();
        int selectCharIndex = -1;
        switch (roundIndex) 
        {
            case 0:
                selectCharIndex = GetCharByAlignment(CharacterData.CharacterAlignment.Neutral); // this is how we pick character to curse, by alignment.
                break;
            case 1:
                selectCharIndex = GetCharByAlignment(CharacterData.CharacterAlignment.Bent1);
                break;
            case 2:
                selectCharIndex = GetCharByAlignment(CharacterData.CharacterAlignment.Gud2);
                break;
            case 3:
                selectCharIndex = GetCharByAlignment(CharacterData.CharacterAlignment.Bent2);
                break;
            case 4:
                selectCharIndex = GetCharByAlignment(CharacterData.CharacterAlignment.Gud1);
                break;
            default:
                Debug.LogWarning("Round: " + roundIndex + "is not useable");
                Debug.LogWarning("Character to curse not selected");
                break;
        }


        //CharacterData selectedCharacter = eligibleCharacters[randomIndex]; // *** So we don't need this can do :
        CharacterData selectedCharacter = characterManager.characterDataSOs[selectCharIndex];

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

    private int GetCharByAlignment(CharacterData.CharacterAlignment charAlignment)
    {
        int charIndex = -1;

        for(int i = 0; i < characterManager.characterDataSOs.Length; i++)
        {
            if (characterManager.characterDataSOs[i].charAlignment == charAlignment)
            {
                charIndex = i;
            }
        }
        return charIndex;
    }

}
