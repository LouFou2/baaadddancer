using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueResponseHandler : MonoBehaviour
{
    [SerializeField] private DialogueManager2 dialogueManager;
    public void CallQueryByName(string queryIdentifier) 
    {
        dialogueManager.RunQueryByIdentifier(queryIdentifier);
    }
}
