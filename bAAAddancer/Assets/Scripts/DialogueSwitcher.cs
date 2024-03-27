using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueSwitcher : MonoBehaviour
{
    [SerializeField] private DialogueSequence[] dialogueSequences; // S.O. that holds a sequence of DialogueData[] (an array of arrays)

    [SerializeField] private DialogueData[] currentDialogueSequence; // a sequence of dialogue objects that plays in a specific scene

    private DialogueData currentDialogue; // the single unit of dialogue

    [SerializeField] private DialoguePlayback dialoguePlayer; // the dialogue "player" (just types out the sentences)

    [SerializeField] private Button button1; // Reference to your yes button (the game needs a default selected button)

    private int currentIndex = 0;

    private SceneSwitcher sceneSwitcher; // will need to get scene name to swap dialogue sequences

    private void Start()
    {
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();

        // Set the initial dialogue sequence based on the current scene
        SetCurrentDialogueSequence();

        currentDialogue = currentDialogueSequence[0];
    }
    private void SetCurrentDialogueSequence()
    {
        string currentSceneName = sceneSwitcher.GetCurrentSceneName();
        DialogueSequence sequence = GetDialogueSequence(currentSceneName);

        if (sequence != null)
        {
            currentDialogueSequence = sequence.dialogueData;
            currentIndex = 0;
        }
        else
        {
            Debug.LogError("Dialogue sequence for scene " + currentSceneName + " not found.");
        }
    }
    private DialogueSequence GetDialogueSequence(string sceneName)
    {
        foreach (DialogueSequence sequence in dialogueSequences)
        {
            if (sequence.sceneKey == sceneName) // Assuming sceneKey holds scene names
            {
                return sequence;
            }
        }
        return null;
    }

    public void TriggerNextDialogue()
    {
        currentIndex = (currentIndex + 1) % currentDialogueSequence.Length;
        currentDialogue = currentDialogueSequence[currentIndex];
        dialoguePlayer.StartDialogue(currentDialogue);

        // Select button1 after restarting the dialogue
        EventSystem.current.SetSelectedGameObject(button1.gameObject);
    }
}
