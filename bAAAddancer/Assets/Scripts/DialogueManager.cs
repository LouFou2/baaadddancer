using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static DialogueData;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueSwitcher dialogueSwitcher;
    [SerializeField] private DialoguePlayback dialoguePlayer;
    public enum DialogueState { NoDialogue, NPCSpeaks, PauseOrContinue, PlayerResponse }
    public DialogueState dialogueState;

    [SerializeField] private Button button0; // No button
    private TextMeshProUGUI button0Text;
    private bool button0clicked = false;
    [SerializeField] private Button button1; // yes button
    private TextMeshProUGUI button1Text;
    private bool button1clicked = false;

    // Unity Events for responses
    public UnityEvent triggerNextDialogueEvent; // set the event in the inspector (what method to call)
    public UnityEvent switchSceneEvent;
    public UnityEvent possibleEvent3; //etc

    void Start()
    {
        if (dialoguePlayer == null)
            Debug.LogError("Assign dialogue player in inspector");
        
        button0Text = button0.GetComponentInChildren<TextMeshProUGUI>();
        button1Text = button1.GetComponentInChildren<TextMeshProUGUI>();

        dialogueState = DialogueState.NoDialogue;
    }
    void Update()
    {
        switch (dialogueState) 
        {
            case DialogueState.NoDialogue:
                button0.gameObject.SetActive(false);
                button1.gameObject.SetActive(false);
                break;            
            case DialogueState.NPCSpeaks:
                button0.gameObject.SetActive(false);
                button1.gameObject.SetActive(false);
                break;
            case DialogueState.PauseOrContinue:
                button0.gameObject.SetActive(false);
                button1.gameObject.SetActive(true);

                if (button1clicked)
                {
                    dialoguePlayer.DisplayNextSentence(); // *** 
                    dialogueState = DialogueState.NPCSpeaks;
                    button1clicked = false;
                }
                break;

            case DialogueState.PlayerResponse:
                button0.gameObject.SetActive(true);
                button1.gameObject.SetActive(true);

                if (button0clicked) 
                {
                    HandleNoResponse(); // can i just modify this line?
                    dialogueState = DialogueState.NoDialogue;
                } 
                if (button1clicked) 
                {
                    HandleYesResponse(); // and this line??
                    dialogueState = DialogueState.NoDialogue;
                }
                
                break;

            default:
                dialogueState = DialogueState.NoDialogue;
                break;
        }
        button0clicked = false; // reset button click states
        button1clicked = false;
    }
    public void Button0Clicked() 
    {
        button0clicked = true;
    }
    public void Button1Clicked()
    {
        button1clicked = true;
    }

    public void NpcSpeaks()
    {
        dialogueState = DialogueState.NPCSpeaks;
    }

    public void DialoguePaused()
    {
        dialogueState = DialogueState.PauseOrContinue;
        button1Text.text = ">";
    }

    public void PlayerResponse(string noResponse, string yesResponse)
    {
        dialogueState = DialogueState.PlayerResponse;
        button0Text.text = noResponse;
        button1Text.text = yesResponse;
    }

    public void HandleYesResponse()
    {
        DialogueData currentDialogue = dialogueSwitcher.GetCurrentDialogue();

        //here we can use case-switching logic, using the enums from the SO
        //and making it call PossibleEvent1/2/3/etc
        switch (currentDialogue.YesEventToCall)
        {
            case EventsToCall.triggerNextDialogue:
                triggerNextDialogueEvent.Invoke();
                break;
            case EventsToCall.switchScene:
                switchSceneEvent.Invoke();
                break;
            // Add more cases for other events as needed
            default:
                Debug.LogError("Unknown event type in dialogue data: " + currentDialogue.YesEventToCall);
                break;
        }
    }
    public void HandleNoResponse()
    {
        DialogueData currentDialogue = dialogueSwitcher.GetCurrentDialogue();

        switch (currentDialogue.YesEventToCall)
        {
            case EventsToCall.triggerNextDialogue:
                triggerNextDialogueEvent.Invoke();
                break;
            case EventsToCall.switchScene:
                switchSceneEvent.Invoke();
                break;
            // Add more cases for other events as needed
            default:
                Debug.LogError("Unknown event type in dialogue data: " + currentDialogue.NoEventToCall);
                break;
        }
    }
}
