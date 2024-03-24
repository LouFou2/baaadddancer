using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialoguePlayback : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    //public DialogueData dialogue; // The dialogue S.O. associated with this script
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typeSpeedInterval = 0.03f;

    private Queue<string> sentences;

    private bool isDialoguePlaying = false; // Flag to indicate if dialogue is currently playing
    private DialogueData currentDialogue; // Current dialogue being played

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        sentences = new Queue<string>();
        dialoguePanel.transform.localScale = Vector3.zero;

        QueueDialogue(currentDialogue);
    }
    public void QueueDialogue(DialogueData newDialogue)
    {
        if (!isDialoguePlaying)
        {
            StartDialogue(newDialogue);
        }
        else
        {
            sentences.Clear();
            currentDialogue = newDialogue;
            foreach (string sentence in currentDialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }
        }
    }

    public void StartDialogue(DialogueData dialogue) 
    {
        sentences.Clear();
        
        foreach (string sentence in dialogue.sentences) 
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence() 
    {
        if (sentences.Count == 0) 
        {
            EndDialogue();
            return;
        }
        dialoguePanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBounce);
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence) 
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) //converts string into an array of characters
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeedInterval);
        }
        dialogueManager.DialoguePaused();
    }

    void EndDialogue() 
    {
        dialogueText.text = "";
        dialoguePanel.transform.DOScale(0, 0.3f);
        dialogueManager.PlayerSpeaks( currentDialogue.responseNo, currentDialogue.responseYes );
        
        Debug.Log("end of conversation");
    }
}
