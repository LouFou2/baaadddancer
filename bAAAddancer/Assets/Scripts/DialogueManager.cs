using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialoguePlayback dialoguePlayer;
    public enum DialogueState { NoDialogue, NPCSpeaks, PauseOrContinue, PlayerResponse, EndDialogue }
    public DialogueState dialogueState;

    [SerializeField] private Button button0; // No button
    private TextMeshProUGUI button0Text;
    [SerializeField] private Button button1; // yes button
    private TextMeshProUGUI button1Text;

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
                // yes button acts as "continue"
                break;
            case DialogueState.PlayerResponse:
                // here we queue new dialogue... HOW?
                button0.gameObject.SetActive(true);
                button1.gameObject.SetActive(true);
                break;
            case DialogueState.EndDialogue:
                break;
            default:
                dialogueState = DialogueState.NoDialogue;
                break;
        }
    }
    
    public void NpcSpeaks()
    {
        dialogueState = DialogueState.NPCSpeaks;
    }

    public void DialoguePaused()
    {
        dialogueState = DialogueState.PauseOrContinue;
    }

    public void PlayerSpeaks(string noResponse, string yesResponse)
    {
        dialogueState = DialogueState.PlayerResponse;
        button0Text.text = noResponse;
        button1Text.text = yesResponse;

    }
}
