using System.Collections.Generic;
using UnityEngine;

public class CurseManager : MonoBehaviour
{
    private CharacterManager characterManager;
    private DebugUI_Manager debugUI_Manager;

    [SerializeField] private float averageInfection; // this is used to pass to the player at the end of the scene

    private void OnEnable()
    {
        AlignerController2.On_AlignerComplete += AlignerCompleteHandler; // from the aligner controller
    }
    private void OnDisable()
    {
        AlignerController2.On_AlignerComplete -= AlignerCompleteHandler; // from the aligner controller
    }
    void AlignerCompleteHandler() //at the end of the alignment, we adjust curse of the debugged character
    {
        int debuggedChar = debugUI_Manager.GetSelectedDebugChar();
        float alignAmount = debugUI_Manager.GetDebugCharAlignment();
        float currentInfectionLevel = characterManager.characterDataSOs[debuggedChar].infectionLevel;
        float debuggedInfectionLevel = currentInfectionLevel * alignAmount; // the alignAmount should be between 0-1
        
        characterManager.characterDataSOs[debuggedChar].infectionLevel = debuggedInfectionLevel;
    }

    void Start()
    {
        // Get reference to CharacterManager
        characterManager = FindObjectOfType<CharacterManager>();
        debugUI_Manager = FindObjectOfType<DebugUI_Manager>();

        // Temporary list to track characters that have had their infection level increased
        List<CharacterData> infectedCharacters = new List<CharacterData>();

        //store a float to calculate average infection level
        averageInfection = 0;

        foreach (CharacterData characterData in characterManager.characterDataSOs)
        {
            characterData.lastCursedCharacter = false;

            //increase current infection level
            if(characterData.infectionLevel > 0  && !infectedCharacters.Contains(characterData) )
            {
                characterData.infectionLevel += Random.Range(0.1f, 0.25f);
                averageInfection += characterData.infectionLevel;

                // Add character to the list of processed characters
                infectedCharacters.Add(characterData);
            }
        }

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
                selectCharIndex = GetCharByAlignment(CharacterData.CharacterAlignment.Gud1);
                break;
            default:
                Debug.LogWarning("Round: " + roundIndex + "is not useable");
                Debug.LogWarning("Character to curse not selected");
                break;
        }

        if (roundIndex == 4) // meaning, this is the rave scene, we don't try the processes below this (we'll have null errors)
        {
            return; // * this could rather be somthing special that happens to curse levels
        }

        CharacterData selectedCharacter = characterManager.characterDataSOs[selectCharIndex];

        // Infect the selected character
        selectedCharacter.infectionLevel += 0.25f;
        averageInfection += 0.25f;
        infectedCharacters.Add(selectedCharacter);
        averageInfection /= infectedCharacters.Count;

        selectedCharacter.lastCursedCharacter = true;

        foreach (CharacterData characterData in characterManager.characterDataSOs)
        {
            if (characterData.infectionLevel > 1) characterData.infectionLevel = 1; //clamp at max 1

            // last round gives the remaining player the average curse level too
            if (roundIndex == 3)
            {
                if (characterData != null && characterData.charAlignment == CharacterData.CharacterAlignment.Bent2)
                {
                    //Player gets average of all chars' infections
                    characterData.infectionLevel = averageInfection;
                }
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
    public float GetAverageTeamInfection()
    {
        //reset averageInfection
        averageInfection = 0;

        List<CharacterData> infectedCharacters = new List<CharacterData>();

        foreach (CharacterData characterData in characterManager.characterDataSOs)
        {
            if (characterData.infectionLevel > 0)
            {
                averageInfection += characterData.infectionLevel;
                infectedCharacters.Add(characterData);
            }
        }
        averageInfection /= infectedCharacters.Count;

        return averageInfection;
    }

    public void SceneEndCurseUpdates() // this needs to be called at end of scene so player gets the average infection
    {
        GetAverageTeamInfection();

        foreach (CharacterData characterData in characterManager.characterDataSOs)
        {
            // INFECTING THE PLAYER
            if (characterData != null && characterData.characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                //Player gets average of all chars' infections
                characterData.infectionLevel = averageInfection;
            }
        }
    }

}
