using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LookManager : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private DialogueSwitcher dialogueSwitcher;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject[] lookTargets; // set in inspector. these are the look aim targets (rig constraints) of each character
    [SerializeField] private GameObject[] lookTargetPositioners; //set in inspector. these are actual positions of characters in scene.

    [SerializeField] private float changeLookDuration = 0.2f;

    [SerializeField] private GameObject randomLookPosition;
    private float gazePositionDuration;
    private bool gazePositionChange;

    // Indexes to use, same as CameraManager, according to dialogue data + character data
    private CharacterData[] characterDataSOs;
    private List<int> availableIndexes = new List<int>();
    private int playerIndex;
    private int bugIndex;
    private int lastBuggedCharacterIndex;
    private int npc1_Index, npc2_Index, npc3_Index; //remaining npcs

    private Queue<DialogueData.DialogueUnit> dialogueUnitQueue = new Queue<DialogueData.DialogueUnit>();
    private DialogueData.DialogueUnit currentDialogueUnit;

    void Start()
    {
        gazePositionChange = false;

        gazePositionDuration = Random.Range(1f, 3f);

        float xPosition = Random.Range(-0.5f, 0.5f);
        float yPosition = Random.Range(1.2f, 1.7f);
        float zPosition = Random.Range(0.5f, 2.5f);

        randomLookPosition.transform.position = new Vector3(xPosition, yPosition, zPosition);

        characterDataSOs = new CharacterData[6];

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

        // Get the current dialogue data
        DialogueData currentDialogue = dialogueSwitcher.GetCurrentDialogue();
        //make a queue of the dialogue units
        QueueNewDialogueUnitsForLookTargets(currentDialogue);
    }

    public void QueueNewDialogueUnitsForLookTargets(DialogueData newDialogue)
    {
        dialogueUnitQueue.Clear();
        foreach (DialogueData.DialogueUnit unit in newDialogue.dialogueUnits)
        {
            dialogueUnitQueue.Enqueue(unit);
        }
        SelectNewLookPosition();
    }

    public void SelectNewLookPosition()
    {
        if (dialogueUnitQueue.Count > 0)
        {
            DialogueData.DialogueUnit unit = dialogueUnitQueue.Dequeue();
            currentDialogueUnit = unit;
            SwitchLookPositions(unit.lookPosition);
        }
    }
    public void SetLooksToPlayer() 
    {
        SwitchLookPositions(DialogueData.LookPosition.LookatPlayer);
    }
    void SwitchLookPositions(DialogueData.LookPosition lookPositionToSwitch)
    {
        
        // Perform camera switching logic based on the provided enum value
        switch (lookPositionToSwitch)
        {
            case DialogueData.LookPosition.LookatPlayer:
                TweenLookTargets(playerIndex);
                break;
            case DialogueData.LookPosition.LookatDemon:
                TweenLookTargets(bugIndex);
                break;
            case DialogueData.LookPosition.LookatNPC2:
                TweenLookTargets(npc1_Index);
                break;
            case DialogueData.LookPosition.LookatNPC3:
                TweenLookTargets(npc2_Index);
                break;
            case DialogueData.LookPosition.LookatNPC4:
                TweenLookTargets(npc3_Index);
                break;
            case DialogueData.LookPosition.LookatLastBuggedNPC:
                TweenLookTargets(lastBuggedCharacterIndex);
                break;
            case DialogueData.LookPosition.LookatCamera:
                TweenLookatCamera();
                break;
            default:
                TweenLookTargets(playerIndex);
                break;
        }
    }
    void TweenLookTargets(int speakingCharacterIndex) 
    {
        for (int i = 0; i < lookTargets.Length; i++) 
        {
            if (i != speakingCharacterIndex)
            {
                lookTargets[i].transform.DOMove(lookTargetPositioners[speakingCharacterIndex].transform.position, changeLookDuration);
            }
            else // if i is the speaking character:
            {
                lookTargets[i].transform.position = randomLookPosition.transform.position;
            }
        }
    }
    void TweenLookatCamera() 
    {
        for (int i = 0; i < lookTargets.Length; i++)
        {
            lookTargets[i].transform.DOMove(mainCamera.transform.position, changeLookDuration);
        }
    }

    void Update()
    {
        if (!gazePositionChange) 
        {
            StartCoroutine(ChangeGazePosition());
        }
    }
    private IEnumerator ChangeGazePosition() 
    {
        gazePositionChange = true;

        float xPosition = Random.Range(-0.5f, 0.5f);
        float yPosition = Random.Range(1.2f, 1.7f);
        float zPosition = Random.Range(1.5f, 3.5f);

        Vector3 newRandomLookPosition = new Vector3(xPosition, yPosition, zPosition);

        float changeGazeDuration = Random.Range(0.4f, 1.5f);
        float timePassed = 0f;

        while (timePassed < changeGazeDuration) 
        {
            timePassed += Time.deltaTime;
            randomLookPosition.transform.DOMove(newRandomLookPosition, changeGazeDuration).SetEase(Ease.InOutBack);
        }
        timePassed = changeGazeDuration;

        yield return new WaitForSeconds(gazePositionDuration);

        gazePositionDuration = Random.Range(1f, 3f);

        gazePositionChange = false;
    }
}
