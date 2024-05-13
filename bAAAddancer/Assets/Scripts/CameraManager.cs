using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private DialogueSwitcher dialogueSwitcher;
    [SerializeField] private Animator cameraAnimator; // script should be attached to GameObject with Camera Animator Controller
    [SerializeField] private CharacterManager characterManager;

    private CharacterData[] characterDataSOs;
    private List<int> availableIndexes = new List<int>();
    private int playerIndex;
    private int bugIndex;
    private int npc1_Index, npc2_Index, npc3_Index, npc4_Index;

    private Queue<DialogueData.DialogueUnit> dialogueUnitQueue = new Queue<DialogueData.DialogueUnit>();
    
    void Start()
    {
        dialogueSwitcher = FindObjectOfType<DialogueSwitcher>();
        cameraAnimator = GetComponent<Animator>();
        characterManager = FindObjectOfType<CharacterManager>();
        
        characterDataSOs = new CharacterData[characterManager.characterDataSOs.Length];

        // Populate available indexes
        for (int i = 0; i < characterManager.characterDataSOs.Length; i++)
        {
            availableIndexes.Add(i);
        }

        for (int i = 0; i < characterManager.characterDataSOs.Length; i++)
        {
            characterDataSOs[i] = characterManager.characterDataSOs[i];
            if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Player) 
            {
                playerIndex = i;
                availableIndexes.Remove(i); // Remove player index
            }
            if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Bug)
            {
                bugIndex = i;
                availableIndexes.Remove(i); // Remove bug index
            }
        }
        // Assign remaining indexes to NPC characters
        npc1_Index = availableIndexes[0];
        npc2_Index = availableIndexes[1];
        npc3_Index = availableIndexes[2];
        npc4_Index = availableIndexes[3];

        // Get the current dialogue data
        DialogueData currentDialogue = dialogueSwitcher.GetCurrentDialogue();
        //make a queue of the dialogue units
        QueueNewDialogueUnits(currentDialogue);
    }

    public void QueueNewDialogueUnits(DialogueData newDialogue)
    {
        dialogueUnitQueue.Clear();
        foreach (DialogueData.DialogueUnit unit in newDialogue.dialogueUnits)
        {
            dialogueUnitQueue.Enqueue(unit);
        }
        SelectNewCamera();
    }

    public void SelectNewCamera()
    {
        if (dialogueUnitQueue.Count > 0)
        {
            DialogueData.DialogueUnit unit = dialogueUnitQueue.Dequeue();
            SwitchCamera(unit.Camera);
        }
        else
        {
            Debug.LogWarning("No more dialogue units in queue.");
        }
    }

    void SwitchCamera(DialogueData.CameraToSwitch cameraToSwitch)
    {
        // Perform camera switching logic based on the provided enum value
        switch (cameraToSwitch)
        {
            case DialogueData.CameraToSwitch.longCam:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.playerCamM:
                // the issue with the player cam is, we need to use the indexes assigned in start
                // to determine which camera is for the player (e.g. if player index is 3, cameraAnimator will use "Char3Cam")
                break;
            case DialogueData.CameraToSwitch.playerCamC:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc01CamM:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc01CamC:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc02CamM:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc02CamC:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc03CamM:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc03CamC:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc04CamM:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc04CamC:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc05CamM:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.npc05CamC:
                cameraAnimator.SetBool("LongCam", true);
                break;
            default:
                cameraAnimator.SetBool("LongCam", true);
                break;
        }
    }

}
