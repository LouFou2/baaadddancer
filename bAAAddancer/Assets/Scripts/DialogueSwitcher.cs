using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueSwitcher : MonoBehaviour
{
    [SerializeField] private DialogueData[] dialogue;
    private DialogueData currentDialogue;
    [SerializeField] private DialoguePlayback dialoguePlayer;

    [SerializeField] private Button button1; // Reference to your yes button

    private int currentIndex = 0;

    private void Start()
    {
        currentDialogue = dialogue[0];
    }

    public void TriggerNextDialogue()
    {
        currentIndex = (currentIndex + 1) % dialogue.Length;
        currentDialogue = dialogue[currentIndex];
        dialoguePlayer.StartDialogue(currentDialogue);

        // Select button1 after starting the dialogue
        EventSystem.current.SetSelectedGameObject(button1.gameObject);
    }
}
