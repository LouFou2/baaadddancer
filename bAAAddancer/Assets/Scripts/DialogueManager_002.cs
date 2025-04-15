using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager_002 : MonoBehaviour
{
    [SerializeField] private CameraManager camManager;
    [SerializeField] private CharacterManager charManager;
    [SerializeField] private CopyDanceSceneSetup sceneSetup;

    //[SerializeField] private GameObject[] cameraAimTargets; // assign each character's aim target (example head bone) in inspector
    [SerializeField] private int lastCursedIndex;

    //[SerializeField] private Cinemachine.CinemachineVirtualCamera[] virtualCameras;

    private bool dialogueStarted;

    [SerializeField] private int roundIndex;
    private int dialogueLineCount = 0;

    private PlayerControls playerControls;
    private bool skipDialogueTriggered;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button button0; // No button
    [SerializeField] private TextMeshProUGUI button0Text;
    private bool button0clicked = false;
    [SerializeField] private Button button1; // yes button
    [SerializeField] private TextMeshProUGUI button1Text;
    private bool button1clicked = false;


    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        dialogueStarted = false;
        dialoguePanel.SetActive(false);
        button0.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);

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

        roundIndex = GameManager.Instance.GetCurrentRound();
    }

    public void StartDancerDialogue() // called from signal emmitter on Timeline
    {
        dialogueStarted = true;
    }

    void Update()
    {
        // Check Input
        skipDialogueTriggered = false;
        if (playerControls.GenericInput.AButton.triggered)
        {
            skipDialogueTriggered = true;
        }

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
                    if (dialogueLineCount == 0)
                    {
                        CursedCharResponse("what's happening?");
                        dialogueLineCount = 1;
                        skipDialogueTriggered = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (skipDialogueTriggered && dialogueLineCount == 1)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Gud1, "oh no");
                        dialogueLineCount = 2;
                        skipDialogueTriggered = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (skipDialogueTriggered && dialogueLineCount == 2)
                    {
                        PlayerResponse("what?", "");
                        dialogueLineCount = 3;
                        skipDialogueTriggered = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (skipDialogueTriggered && dialogueLineCount == 3)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Gud1, "it's the curse");
                        dialogueLineCount = 4;
                        skipDialogueTriggered = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (skipDialogueTriggered && dialogueLineCount == 4)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Neutral, "what curse?!");
                        dialogueLineCount = 5;
                        skipDialogueTriggered = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (skipDialogueTriggered && dialogueLineCount == 5)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Gud2, "THE CURSE OF THE RAVE DEMON!");
                        dialogueLineCount = 6;
                        skipDialogueTriggered = false; // ensures it doesn't skip to next line in same frame
                    }

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

    void NPCResponse(CharacterData.CharacterAlignment charAlignment, string dialogueLine)
    {
        dialoguePanel.SetActive(true);
        button0.gameObject.SetActive(true);
        button1.gameObject.SetActive(false);

        for (int i = 0; i < charManager.characterDataSOs.Length; i++)
        {
            if (charManager.characterDataSOs[i].charAlignment == charAlignment)
            {
                // flag the corresponding virtual camera related to current character's index
                camManager.SetCamera(i);
                dialogueText.text = dialogueLine;
                button0Text.text = ">";
            }
            
        }
    }
    void CursedCharResponse(string dialogueLine)
    {
        dialoguePanel.SetActive(true);
        button0.gameObject.SetActive(true);
        button1.gameObject.SetActive(false);

        for (int i = 0; i < charManager.characterDataSOs.Length; i++)
        {
            if (charManager.characterDataSOs[i].lastCursedCharacter)
            {
                // flag the corresponding virtual camera related to current character's index
                camManager.SetCamera(i);
                CharacterData.CharacterAlignment charAlignment = charManager.characterDataSOs[i].charAlignment;
                dialogueText.text = dialogueLine;
                button0Text.text = ">";
            }
        }
    }
    void PlayerResponse(string dialogueLine0, string dialogueLine1)
    {
        dialoguePanel.SetActive(false);
        button0.gameObject.SetActive(true);
        if(dialogueLine1 != "") // only need this option if there are two responses
            button1.gameObject.SetActive(true);

        for (int i = 0; i < charManager.characterDataSOs.Length; i++)
        {
            if (charManager.characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                // flag the corresponding virtual camera related to current character's index
                camManager.SetCamera(i);
                
                // dialogue
                button0Text.text = dialogueLine0;
                if (dialogueLine1 != "")
                    button1Text.text = dialogueLine1;
            }
        }
    }
    

}
