using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    private CharacterData[] characterDataSOs;
    [SerializeField] private Button[] buttons;

    // Define the shader property name as a constant
    private const string burnShaderEffect = "_NoiseFade"; 

    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        
        if (characterManager == null)
        {
            Debug.LogError("CharacterManager is null");
            return; // Exit early if characterManager is not found
        }

        characterDataSOs = new CharacterData[characterManager.characterDataSOs.Length];

        if (buttons == null || buttons.Length != characterManager.characterDataSOs.Length)
        {
            Debug.LogError("Buttons array is not properly assigned or length mismatch.");
            return; // Exit early if buttons array is not properly assigned
        }

        for (int i = 0; i < characterManager.characterDataSOs.Length; i++)
        {
            characterDataSOs[i] = characterManager.characterDataSOs[i];

            Material buttonMaterial = buttons[i].image.material;
            if (buttonMaterial != null)
            {
                buttonMaterial.SetFloat(burnShaderEffect, 0f);
            }

            if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                // Set the button to non-interactable
                buttons[i].interactable = false;
                if (i == 0) // meaning this is button for Char01: first selected in EventSystem
                {
                    buttons[1].Select(); // we select the second button 
                }
            }
        }
    }
}
