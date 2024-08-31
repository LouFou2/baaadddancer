using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventsManager : MonoBehaviour
{
    private SceneSwitcher sceneSwitcher;

    private void Start()
    {
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
    }
    public void HandleEvents(DialogueEventData[] dialogueEvents)
    {
        foreach (var eventData in dialogueEvents)
        {
            HandleEvent(eventData);
        }
    }

    // Method to process the DialogueEvents enum and invoke the corresponding method
    public void HandleEvent(DialogueEventData eventData)
    {
        switch (eventData.dialogueEvent)
        {
            case DialogueEvents.None:
                break;
            case DialogueEvents.SwitchDialogueSequence:
                SwitchDialogueSequence();
                break;
            case DialogueEvents.SwitchNextLevelKey:
                SwitchNextLevelKey();
                break;
            case DialogueEvents.SwitchNextRound:
                SwitchNextRound();
                break;
            case DialogueEvents.SwitchNextScene:
                SwitchNextScene();
                break;
            case DialogueEvents.LoadSceneByIndex:
                LoadSceneByIndex(eventData.intArgument);
                break;
            // Add more cases for other events
            default:
                eventData.dialogueEvent = DialogueEvents.None;
                break;
        }
    }
    
    // Set up events methods as needed
    void SwitchDialogueSequence()
    {
        DialogueSwitcher2 dialogueSwitcher = GetComponent<DialogueSwitcher2>();
        dialogueSwitcher.SwitchDialogueUnits();
    }
    void SwitchNextLevelKey() 
    {
        sceneSwitcher.SwitchToNextLevelKey();
    }
    void SwitchNextRound() 
    {
        sceneSwitcher.SwitchToNextRound();
    }
    void SwitchNextScene() 
    {
        sceneSwitcher.LoadNextScene();
    }
    void LoadSceneByIndex(int sceneIndex)
    {
        sceneSwitcher.LoadSceneByIndex(sceneIndex);
    }
}
