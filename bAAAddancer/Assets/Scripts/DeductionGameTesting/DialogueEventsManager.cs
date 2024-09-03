using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Custom UnityEvent class that takes an int argument
[System.Serializable]
public class UnityEventInt : UnityEvent<int> { }

public class DialogueEventsManager : MonoBehaviour
{
    public UnityEvent SwitchDialogueSequence;
    public UnityEvent SwitchNextLevelKey;
    public UnityEvent SwitchNextRound;
    public UnityEvent SwitchNextScene;
    public UnityEventInt LoadSceneByIndex;  // Using the custom UnityEvent that takes an int
    public UnityEvent StartDebugGame;
    public UnityEvent SpeakerJustLied;
    public UnityEvent ForgetLies;
    public UnityEvent IncrementTeamCurse;

    public void HandleEvents(DialogueEventData[] dialogueEvents)
    {
        foreach (var eventData in dialogueEvents)
        {
            HandleEvent(eventData);
        }
    }

    public void HandleEvent(DialogueEventData eventData)
    {
        switch (eventData.dialogueEvent)
        {
            case DialogueEvents.None:
                break;
            case DialogueEvents.SwitchDialogueSequence:
                SwitchDialogueSequence.Invoke();
                break;
            case DialogueEvents.SwitchNextLevelKey:
                SwitchNextLevelKey.Invoke();
                break;
            case DialogueEvents.SwitchNextRound:
                SwitchNextRound.Invoke();
                break;
            case DialogueEvents.SwitchNextScene:
                SwitchNextScene.Invoke();
                break;
            case DialogueEvents.LoadSceneByIndex:
                LoadSceneByIndex.Invoke(eventData.intArgument);  // Invoking with an int argument
                break;
            case DialogueEvents.StartDebugGame:
                StartDebugGame.Invoke();
                break;
            case DialogueEvents.SpeakerJustLied:
                SpeakerJustLied.Invoke();
                break;
            case DialogueEvents.ForgetLies:
                ForgetLies.Invoke();
                break;
            case DialogueEvents.IncrementTeamCurse:
                IncrementTeamCurse.Invoke();
                break;
            // Add more cases for other events
            default:
                eventData.dialogueEvent = DialogueEvents.None;
                break;
        }
    }
}
