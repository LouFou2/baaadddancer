using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaveSceneSetup : MonoBehaviour
{
    private CharacterManager characterManager;
    [SerializeField] private GameObject[] charSetupTransforms;
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        int charIndex = 0;

        for (int i = 0; i < characterManager.characters.Length; i++) // we subtract 1 here because 1 of the characters are eliminated
        {
            GameObject character = characterManager.characters[i];
            CharacterData charData = characterManager.characterDataSOs[i];

            CopyProxyRig copyProxyRig = character.GetComponent<CopyProxyRig>();
            CopyDance copyDanceScript = character.GetComponent<CopyDance>();

            if (charData.wasEliminated)
            {
                character.SetActive(false);
            }
            else
            {
                character.SetActive(true);
                copyProxyRig.enabled = false;
                copyDanceScript.enabled = true;

                if (charIndex < charSetupTransforms.Length)
                {
                    character.transform.position = charSetupTransforms[charIndex].transform.position;
                    character.transform.rotation = charSetupTransforms[charIndex].transform.rotation;
                    charIndex++;
                }
                else
                {
                    Debug.LogError("Not enough setup transforms for the number of characters.");
                }
            }


            //set character as left screen, center screen, or right screen:
            if (charIndex < 3) // 3 is the center character
                copyDanceScript.charLeftScreen = true;
            else if (charIndex == 3)
                copyDanceScript.charCenterScreen = true;
            else
                copyDanceScript.charRightScreen = true;
            
        }
    }
}
