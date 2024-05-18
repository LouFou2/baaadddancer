using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    private CharacterData[] characterDataSOs;
    [SerializeField] private Button[] buttons;
    
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        
        if (characterManager == null)
        {
            Debug.LogError("CharacterManager is null");
            return; // Exit early if characterManager is not found
        }

        // Log the number of characterDataSOs
        Debug.Log($"Number of characterDataSOs: {characterManager.characterDataSOs.Length}");

        characterDataSOs = new CharacterData[characterManager.characterDataSOs.Length];

        if (buttons == null || buttons.Length != characterManager.characterDataSOs.Length)
        {
            Debug.LogError("Buttons array is not properly assigned or length mismatch.");
            return; // Exit early if buttons array is not properly assigned
        }

        for (int i = 0; i < characterManager.characterDataSOs.Length; i++)
        {
            characterDataSOs[i] = characterManager.characterDataSOs[i];
            Debug.Log($"Character {i} Role: {characterDataSOs[i].characterRoleSelect}");

            if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                // Set the button to non-interactable
                buttons[i].interactable = false;

                // Option 1: Change the color of the button to grey it out
                ColorBlock colors = buttons[i].colors;
                colors.normalColor = Color.gray;
                colors.highlightedColor = Color.gray;
                colors.pressedColor = Color.gray;
                colors.selectedColor = Color.gray;
                colors.disabledColor = Color.gray;
                buttons[i].colors = colors;
            }
        }
    }
}
