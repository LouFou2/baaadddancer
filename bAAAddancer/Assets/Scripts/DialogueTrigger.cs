using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private DialoguePlayback dialoguePlayer;

    public void TriggerDialogue() 
    {
        dialoguePlayer.StartDialogue(dialogue);
    }
}
