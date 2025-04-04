using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

// THIS SCRIPT IS FOR DIALOGUE SCENE and MEETING SCENE
public class CameraManager : MonoBehaviour
{
    [SerializeField] private DialogueSwitcher dialogueSwitcher;
    [SerializeField] private Animator cameraAnimator; // script should be attached to GameObject with Camera Animator Controller
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private string[] cameraFlags; // the camera flags are the bool names in the Animator

    private CharacterData[] characterDataSOs;
    private List<int> availableIndexes = new List<int>();
    private int playerIndex;
    private int demonIndex;
    private int lastBuggedCharacterIndex;
    private int npc1_Index, npc2_Index, npc3_Index; //remaining npcs

    private Queue<DialogueData.DialogueUnit> dialogueUnitQueue = new Queue<DialogueData.DialogueUnit>();
    private DialogueData.DialogueUnit currentDialogueUnit;
    
    void Start()
    {
        dialogueSwitcher = FindObjectOfType<DialogueSwitcher>();
        cameraAnimator = GetComponent<Animator>();
        characterManager = FindObjectOfType<CharacterManager>();
        
        characterDataSOs = new CharacterData[6];
        cameraFlags = new string[6];

        // the camera flags are the bool names in the Animator
        cameraFlags[0] = "CharCam1"; // the caracters these cameras are aimed at are arranged in same order as in Character Manager
        cameraFlags[1] = "CharCam2"; // which is why we need logic below to figure out which indexes are the player, ravedemon, and last bugged character
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
            if (characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Demon)
            {
                demonIndex = i;
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
        QueueNewDialogueUnitsForCamera(currentDialogue);
    }

    public void QueueNewDialogueUnitsForCamera(DialogueData newDialogue)
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
            currentDialogueUnit = unit;
            SwitchCamera(unit.Camera);
        }
    }
    public void SelectPlayerCamera()
    {
        SwitchCamera(DialogueData.CameraToSwitch.playerCamM);
    }

    void SwitchCamera(DialogueData.CameraToSwitch cameraToSwitch)
    {
        // Reset all boolean parameters to false
        foreach (string flag in cameraFlags)
        {
            cameraAnimator.SetBool(flag, false);
        }
        // Perform camera switching logic based on the provided enum value
        switch (cameraToSwitch)
        {
            case DialogueData.CameraToSwitch.longCam:
                cameraAnimator.SetBool("LongCam", true);
                break;
            case DialogueData.CameraToSwitch.playerCamM:
                cameraAnimator.SetBool(cameraFlags[playerIndex], true); //see here we pass in the indexes that we get above
                cameraAnimator.SetFloat("MedToClose", 0);
                break;
            case DialogueData.CameraToSwitch.playerCamC:
                cameraAnimator.SetBool(cameraFlags[playerIndex], true);
                cameraAnimator.SetFloat("MedToClose", 1);
                break;
            case DialogueData.CameraToSwitch.ravedemonCamM:
                cameraAnimator.SetBool(cameraFlags[demonIndex], true); // *** bug is always npc1 cam, is there way to make this random?
                cameraAnimator.SetFloat("MedToClose", 0);
                break;
            case DialogueData.CameraToSwitch.ravedemonCamC:
                cameraAnimator.SetBool(cameraFlags[demonIndex], true);
                cameraAnimator.SetFloat("MedToClose", 1);
                break;
            case DialogueData.CameraToSwitch.npc01CamM:
                cameraAnimator.SetBool(cameraFlags[npc1_Index], true);
                cameraAnimator.SetFloat("MedToClose", 0);
                break;
            case DialogueData.CameraToSwitch.npc01CamC:
                cameraAnimator.SetBool(cameraFlags[npc1_Index], true);
                cameraAnimator.SetFloat("MedToClose", 1);
                break;
            case DialogueData.CameraToSwitch.npc02CamM:
                cameraAnimator.SetBool(cameraFlags[npc2_Index], true);
                cameraAnimator.SetFloat("MedToClose", 0);
                break;
            case DialogueData.CameraToSwitch.npc02CamC:
                cameraAnimator.SetBool(cameraFlags[npc2_Index], true);
                cameraAnimator.SetFloat("MedToClose", 1);
                break;
            case DialogueData.CameraToSwitch.npc03CamM:
                cameraAnimator.SetBool(cameraFlags[npc3_Index], true);
                cameraAnimator.SetFloat("MedToClose", 0);
                break;
            case DialogueData.CameraToSwitch.npc03CamC:
                cameraAnimator.SetBool(cameraFlags[npc3_Index], true);
                cameraAnimator.SetFloat("MedToClose", 1);
                break;
            case DialogueData.CameraToSwitch.lastBuggedCamM:
                cameraAnimator.SetBool(cameraFlags[lastBuggedCharacterIndex], true);
                cameraAnimator.SetFloat("MedToClose", 0);
                break;
            case DialogueData.CameraToSwitch.lastBuggedCamC:
                cameraAnimator.SetBool(cameraFlags[lastBuggedCharacterIndex], true);
                cameraAnimator.SetFloat("MedToClose", 1);
                break;
            default:
                cameraAnimator.SetBool("LongCam", true);
                break;
        }
    }

}
