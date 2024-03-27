using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Sequence", menuName = "Dialogue System/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    public DialogueData[] dialogueData;
    public string sceneKey; // maybe enums used by SceneSwitcher
}

