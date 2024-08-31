using UnityEngine;

[CreateAssetMenu(fileName = "NewNew Dialogue Sequence", menuName = "NewDialogueSystem/Dialogue Sequence")]
public class DynamicDialogueSequence : ScriptableObject
{
    public LevelKey levelKey;
    public DynamicDialogueUnits[] dynamicDialogueUnits;
}
