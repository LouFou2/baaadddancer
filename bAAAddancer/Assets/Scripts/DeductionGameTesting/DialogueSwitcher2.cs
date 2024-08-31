using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSwitcher2 : MonoBehaviour //***RENAME TO DIEALOGUESWITCHER WHEN FULL REPLACEMENT HAS BEEN IMPLEMENT
{
    [Tooltip("The scriptable objects with the sequences of dynamic dialogue units, for all rounds of this scene")]
    public DynamicDialogueSequence[] dynamicDialogueSequences;
    [Tooltip("The query criteria scriptable objects, for all rounds of this scene")]
    public DialogueQueryCriteria[] queryCriteriaSOs;

    private DynamicDialogueSequence currentSceneDialogueSequence;
    public DynamicDialogueUnits currentDialogueUnits;

    public DialogueQueryCriteria currentSceneQueryCriteria;

    private LevelKey currentLevelKey;
    public int currentUnitsIndex = 0;

    private void Start()
    {
        currentLevelKey = GameManager.Instance.GetCurrentLevelKey();

        foreach (DynamicDialogueSequence sequence in dynamicDialogueSequences) 
        {
            if (sequence.levelKey == currentLevelKey)
            {
                currentSceneDialogueSequence = sequence;
            }
            else
            {
                Debug.LogWarning("No valid Levelkey found for Dialogue Sequence");
            }
        }
        foreach (DialogueQueryCriteria queryCritera in queryCriteriaSOs) 
        {
            if (queryCritera.levelKey == currentLevelKey)
            {
                currentSceneQueryCriteria = queryCritera;
            }
            else
            {
                Debug.LogWarning("No valid Levelkey found for Query Criteria");
            }
        }

        currentDialogueUnits = currentSceneDialogueSequence.dynamicDialogueUnits[0];

        currentUnitsIndex = 0; //***Not sure this is necesarry, as it is set to 0 in declaration?
    }

    // a simple "switcher" responsible for switching dialogue units
    public void SwitchDialogueUnits()
    {
        currentUnitsIndex += 1;
        currentDialogueUnits = currentSceneDialogueSequence.dynamicDialogueUnits[currentUnitsIndex];
    }
}
