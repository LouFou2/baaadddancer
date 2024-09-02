using UnityEngine;

public enum DialogueEvents
{
    None,
    SwitchDialogueSequence,
    SwitchNextLevelKey,
    SwitchNextRound,
    SwitchNextScene,
    LoadSceneByIndex,
    StartDebugGame,
}

[System.Serializable]
public class DialogueEventData
{
    public DialogueEvents dialogueEvent;
    public int intArgument; // For cases like scene index
}
