using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue And Queries Sequence", menuName = "NewDialogueSystem/Dialogue + Queries Sequence")]
public class DialogueAndQuerySequence : ScriptableObject
{
    public LevelKey levelKey;
    public DynamicDialogueUnits[] dynamicDialogueUnits;
    public DialogueQueryCriteria[] dialogueQueries;
}
