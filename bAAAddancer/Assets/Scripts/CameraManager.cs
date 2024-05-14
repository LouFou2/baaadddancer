using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private DialogueSwitcher dialogueSwitcher;
    [SerializeField] private Animator cameraAnimator; // script should be attached to GameObject with Camera Animator Controller
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private string[] cameraFlags; // the camera flags are the bool names in the Animator

    private CharacterData[] characterDataSOs;
    private List<int> availableIndexes = new List<int>();
    private int playerIndex;
    private int bugIndex;
    private int lastBuggedCharacterIndex;
    private int npc1_Index, npc2_Index, npc3_Index; //remaining npcs

    private Queue<DialogueData.DialogueUnit> dialogueUnitQueue = new Queue<DialogueData.DialogueUnit>();
    
    void Start()
    {
        dialogueSwitcher = FindObjectOfType<DialogueSwitcher>();
        cameraAnimator = GetComponent<Animator>();
        characterManager = FindObjectOfType<CharacterManager>();
        
        characterDataSOs = new CharacterData[6];
        cameraFlags = new string[6];

        // the camera flags are the bool names in the Animator
        cameraFlags[0] = "CharCam1"; // the caracters these cameras are aimed at are arranged in same order as in Character Manager
        cameraFlags[1] = "CharCam2"; // which is why we need logic below to figire out which indexes are the player, ravedemon, and last bugged character
        cameraFlags[2] = "CharCam3"; // (for the sake of the dialogue moments)
        cameraFlags[3] = "CharCam4";
        cameraFlags[4] = "CharCam5";
        cameraFlags[5] = "CharCam6";

        availableIndexes.Clear();
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
            if (characterDataSOs[i].lastBuggedCharacter == true)
            {
                lastBuggedCharacterIndex = i;
                availableIndexes.Remove(i); // Remove last bugged character index
            }
        }
        // Assign remaining indexes to NPC characters
        npc1_Index = availableIndexes[0];
        npc2_Index = availableIndexes[1];
        npc3_Index = availableIndexes[2];

        if(availableIndexes.Count == 4) //meaning there is no "lastBuggedCharacter" yet
            lastBuggedCharacterIndex = availableIndexes[3];

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
                cameraAnimator.SetBool(cameraFlags[playerIndex], true); //see here we pass in the indexes that we get above
                break;
            case DialogueData.CameraToSwitch.playerCamC:
                cameraAnimator.SetBool(cameraFlags[playerIndex], true);
                break;
            case DialogueData.CameraToSwitch.ravedemonCamM:
                cameraAnimator.SetBool(cameraFlags[bugIndex], true); // *** bug is always npc1 cam, is there way to make this random?
                break;
            case DialogueData.CameraToSwitch.ravedemonCamC:
                cameraAnimator.SetBool(cameraFlags[bugIndex], true);
                break;
            case DialogueData.CameraToSwitch.npc02CamM:
                cameraAnimator.SetBool(cameraFlags[npc1_Index], true);
                break;
            case DialogueData.CameraToSwitch.npc02CamC:
                cameraAnimator.SetBool(cameraFlags[npc1_Index], true);
                break;
            case DialogueData.CameraToSwitch.npc03CamM:
                cameraAnimator.SetBool(cameraFlags[npc2_Index], true);
                break;
            case DialogueData.CameraToSwitch.npc03CamC:
                cameraAnimator.SetBool(cameraFlags[npc2_Index], true);
                break;
            case DialogueData.CameraToSwitch.npc04CamM:
                cameraAnimator.SetBool(cameraFlags[npc3_Index], true);
                break;
            case DialogueData.CameraToSwitch.npc04CamC:
                cameraAnimator.SetBool(cameraFlags[npc3_Index], true);
                break;
            case DialogueData.CameraToSwitch.lastBuggedCamM:
                cameraAnimator.SetBool(cameraFlags[lastBuggedCharacterIndex], true);
                break;
            case DialogueData.CameraToSwitch.lastBuggedCamC:
                cameraAnimator.SetBool(cameraFlags[lastBuggedCharacterIndex], true);
                break;
            default:
                cameraAnimator.SetBool("LongCam", true);
                break;
        }
    }

}
