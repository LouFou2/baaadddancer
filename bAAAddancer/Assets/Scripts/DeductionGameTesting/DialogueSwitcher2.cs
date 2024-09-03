using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSwitcher2 : MonoBehaviour //***RENAME TO DIEALOGUE SWITCHER WHEN FULL REPLACEMENT HAS BEEN IMPLEMENT
{
    [Tooltip("The scriptable objects with the sequences of dialogue units, as well as sequences of queries, for all rounds of this scene")]
    public DialogueAndQuerySequence[] dialogueAndQuerySequences;

    private DialogueAndQuerySequence currentSceneDialogueSequence;

    public DynamicDialogueUnits currentDialogueUnits;
    public DialogueQueryCriteria currentSceneQueryCriteria;

    private LevelKey currentLevelKey;
    public int currentUnitsIndex = 0;

    private void Start()
    {
        currentLevelKey = GameManager.Instance.GetCurrentLevelKey();

        foreach (DialogueAndQuerySequence sequence in dialogueAndQuerySequences) 
        {
            if (sequence.levelKey == currentLevelKey)
            {
                currentSceneDialogueSequence = sequence;
            }
        }

        currentDialogueUnits = currentSceneDialogueSequence.dynamicDialogueUnits[0];
        currentSceneQueryCriteria = currentSceneDialogueSequence.dialogueQueries[0];

        currentUnitsIndex = 0; //***Not sure this is necesarry, as it is set to 0 in declaration?
    }

    // a simple "switcher" responsible for switching dialogue units
    public void SwitchDialogueUnits()
    {
        currentUnitsIndex += 1;
        currentDialogueUnits = currentSceneDialogueSequence.dynamicDialogueUnits[currentUnitsIndex];
        currentSceneQueryCriteria = currentSceneDialogueSequence.dialogueQueries[currentUnitsIndex];
    }
}
