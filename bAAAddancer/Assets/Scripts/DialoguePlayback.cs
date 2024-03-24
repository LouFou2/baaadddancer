using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialoguePlayback : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData currentDialogue; // Current dialogue being played
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float typeSpeedInterval = 0.03f;

    private Queue<string> sentences;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        sentences = new Queue<string>();
        dialoguePanel.transform.localScale = Vector3.zero;

        StartDialogue(currentDialogue);
    }
    
    public void StartDialogue(DialogueData newDialogue) 
    {
        sentences.Clear();
        currentDialogue = newDialogue;
        foreach (string sentence in currentDialogue.sentences)
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
    }
}
