using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoDebug : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;

    void Start()
    {
        
        for (int i = 0; i < characterManager.characterDataSOs.Length; i++) 
        {
            if(characterManager.characterDataSOs[i].infectionLevel > 0)
                characterManager.characterDataSOs[i].wasDebuggedLastRound = true;
        }
        
    }
}
