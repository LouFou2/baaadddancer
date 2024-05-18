using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : MonoBehaviour
{
    private CharacterManager characterManager;

    [SerializeField] private Button[] buttons;

    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        foreach (CharacterData characterData in characterManager.characterDataSOs) 
        {
            characterData.wasEliminated = false;
        }
    }

    public void VoteChar1() 
    {
        characterManager.characterDataSOs[0].wasEliminated = true;
    }
    public void VoteChar2()
    {
        characterManager.characterDataSOs[1].wasEliminated = true;
    }
    public void VoteChar3()
    {
        characterManager.characterDataSOs[2].wasEliminated = true;
    }
    public void VoteChar4()
    {
        characterManager.characterDataSOs[3].wasEliminated = true;
    }
    public void VoteChar5()
    {
        characterManager.characterDataSOs[4].wasEliminated = true;
    }
    public void VoteChar6()
    {
        characterManager.characterDataSOs[5].wasEliminated = true;
    }

}
