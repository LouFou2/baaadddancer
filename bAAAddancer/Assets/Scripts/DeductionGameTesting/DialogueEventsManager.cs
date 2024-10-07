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
    public UnityEvent SpeakerWantsElimination;
    public UnityEvent SpeakerBlocksElimination;
    public UnityEvent LoadVoteScene;
    public UnityEvent SpeakerChooseGud;
    public UnityEvent SpeakerChooseCurse;
    public UnityEvent PlayerChooseGud;
    public UnityEvent PlayerChooseCurse;

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
            //case DialogueEvents.LoadSceneByIndex:                 // I don't think I use this ever?
                //LoadSceneByIndex.Invoke(eventData.intArgument);   // as in, I don't load scene by index from scriptable object data
                //break;
            case DialogueEvents.StartDebugGame:
                StartDebugGame.Invoke();
                break;
            case DialogueEvents.SpeakerJustLied:
                SpeakerJustLied.Invoke();
                break;
            case DialogueEvents.ForgetLies:
                ForgetLies.Invoke();
                break;
            case DialogueEvents.CountTeamCurse:
                IncrementTeamCurse.Invoke();
                break;
            case DialogueEvents.GoElimination:
                SpeakerWantsElimination.Invoke();
                break;
            case DialogueEvents.NoGoElimination:
                SpeakerBlocksElimination.Invoke();
                break;
            case DialogueEvents.LoadVoteScene:
                LoadVoteScene.Invoke();
                break;
            case DialogueEvents.SpeakerChooseGud:
                SpeakerChooseGud.Invoke();
                break;
            case DialogueEvents.SpeakerChooseCurse:
                SpeakerChooseCurse.Invoke();
                break;
            case DialogueEvents.PlayerChooseGud:
                PlayerChooseGud.Invoke();
                break;
            case DialogueEvents.PlayerChooseCurse:
                PlayerChooseCurse.Invoke();
                break;
            // Add more cases for other events
            default:
                eventData.dialogueEvent = DialogueEvents.None;
                break;
        }
    }

}
