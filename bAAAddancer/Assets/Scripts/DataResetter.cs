using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataResetter : MonoBehaviour
{
    [SerializeField] private CharacterData[] characterDataSOs;
    [SerializeField] private RoundsRecData[] roundsRecDataSOs;

    public void ResetAllData()
    {
        for (int i = 0; i < characterDataSOs.Length; i++)
        {
            characterDataSOs[i].characterRoleSelect = CharacterData.CharacterRole.NPC; //this just resets all characters to NPC's
            characterDataSOs[i].infectionLevel = 0;
            characterDataSOs[i].wasDebuggedLastRound = false;
            characterDataSOs[i].lastCursedCharacter = false;
            characterDataSOs[i].wasEliminated = false;
        }
        for (int j = 0; j < roundsRecDataSOs.Length; j++) 
        {
            foreach (RecordingData recordData in roundsRecDataSOs[j].recordingDataOfRounds) 
            {
                recordData.initialPositions = new Vector3[64];
                recordData.recordedPositions = new Vector3[64];
            }
        }
    }
}
