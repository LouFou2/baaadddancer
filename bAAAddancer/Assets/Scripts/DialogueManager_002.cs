using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager_002 : MonoBehaviour
{
    [SerializeField] private CameraManager camManager;
    [SerializeField] private CharacterManager charManager;
    [SerializeField] private CopyDanceSceneSetup sceneSetup;

    [SerializeField] private GameObject[] cameraAimTargets; // assign each character's aim target (example head bone) in inspector
    [SerializeField] private int lastCursedIndex;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera[] virtualCameras;

    private bool dialogueStarted;

    [SerializeField] private int roundIndex;
    private int dialogueLineCount;

    void Start()
    {
        dialogueStarted = false;

        // find the last cursed character
        if (charManager != null)
        {
            for(int i = 0; i < charManager.characterDataSOs.Length; i++)
            {
                if (charManager.characterDataSOs[i].lastCursedCharacter)
                {
                    lastCursedIndex = i;
                }
            }
        }
        else
        {
            Debug.LogWarning("assign char manager in inspector");
        }

        // Assign each virtual camera's LookAt target
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (i < cameraAimTargets.Length && virtualCameras[i] != null && cameraAimTargets[i] != null)
            {
                virtualCameras[i].LookAt = cameraAimTargets[i].transform;
            }
            else
            {
                Debug.LogWarning($"Missing camera or target at index {i}");
            }
        }

        roundIndex = GameManager.Instance.GetCurrentRound();
    }

    public void StartDancerDialogue()
    {
        dialogueStarted = true; // called from signal emmitter on Timeline
    }

    void Update()
    {
        if (!dialogueStarted)
        {
            return;
        }

        else
        {
            switch (roundIndex)
            {
                case -1: // no dialogue/ not started
                    break;

                case 0:
                    //Round 1 Dialogue
                    QueryDialogue(CharacterData.CharacterAlignment.Gud1, "um something strange over here");
                    break;

                case 1:
                    //Round 2 Dialogue

                    break;

                case 2:
                    //Round 3 Dialogue

                    break;

                case 3:
                    //Round 4 Dialogue

                    break;



                default:
                    roundIndex = -1;
                    break;
            }
        }
    }

    void QueryDialogue(CharacterData.CharacterAlignment charAlignment, string dialogueLine)
    {
        for(int i = 0; i < charManager.characterDataSOs.Length; i++)
        {
            if (charManager.characterDataSOs[i].charAlignment == charAlignment)
            {
                // flag the corresponding virtual camera related to current character's index
                camManager.SetCamera(i);
                Debug.Log(dialogueLine);
            }
        }
    }

}
