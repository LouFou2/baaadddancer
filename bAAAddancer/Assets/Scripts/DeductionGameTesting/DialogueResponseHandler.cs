using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueResponseHandler : MonoBehaviour
{
    [SerializeField] private DialogueQueryHandler dialogueManager;
    public void CallQueryByName(string queryIdentifier) 
    {
        dialogueManager.RunQueryByIdentifier(queryIdentifier);
    }
}
