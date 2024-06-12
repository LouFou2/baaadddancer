using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaveDemonRevealer : MonoBehaviour
{
    private CharacterManager characterManager;
    [SerializeField] private bool raveDemonEliminated;

    public UnityEvent revealRaveDemon;

    private void Start()
    {
        raveDemonEliminated = true;
        characterManager = FindObjectOfType<CharacterManager>();
        for (int i = 0; i < characterManager.characterDataSOs.Length; i++) 
        {
            CharacterData characterData = characterManager.characterDataSOs[i];
            if (characterData.characterRoleSelect == CharacterData.CharacterRole.Bug && !characterData.wasEliminated)
            {
                raveDemonEliminated = false;
            }
        }
    }
    public void RevealRaveDemon() 
    {
        if(!raveDemonEliminated)
            revealRaveDemon.Invoke();
    }
    public bool CheckIfPlayerWins() 
    {
        return raveDemonEliminated;
    }
}
