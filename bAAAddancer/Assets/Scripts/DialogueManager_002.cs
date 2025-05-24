using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueManager_002 : MonoBehaviour
{
    [SerializeField] private CameraManager camManager;
    [SerializeField] private CharacterManager charManager;
    [SerializeField] private CurseManager curseManager; // we need to check average team cursed level (to affect gud chars responses)
    [SerializeField] private CopyDanceSceneSetup sceneSetup;
    [SerializeField] private DebugUI_Manager debugUI_Manager;
    [SerializeField] private GameObject debuggerUIParent;
    [SerializeField] private GameObject renderTexture; // will deactivate this while it's only UI (doesn't need to be rendering in background)
    [SerializeField] StopDance stopDanceScript;

    private int lastCursedIndex;
    private int playerIndex;

    private bool dialogueStarted;
    private bool debugRunning = false;

    [SerializeField] private int roundIndex;
    [SerializeField] private int dialogueLineCount = 0;

    private PlayerControls playerControls;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button button0; // No button
    [SerializeField] private TextMeshProUGUI button0Text;
    [SerializeField] private Button button1; // yes button
    [SerializeField] private TextMeshProUGUI button1Text;
    private bool button0clicked = false;
    private bool button1clicked = false;

    [SerializeField] private float curseTolerance = 0.6f;

    public bool isDebugging = false;


    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        DebugUI_Manager.On_DebugComplete += EndDebugHandler;
    }
    private void OnDisable()
    {
        playerControls.Disable();
        DebugUI_Manager.On_DebugComplete -= EndDebugHandler;
    }

    void Start()
    {
        debuggerUIParent.SetActive(false);

        dialogueStarted = false;
        dialoguePanel.SetActive(false);
        button0.gameObject.SetActive(false);
        button1.gameObject.SetActive(false);

        // find the last cursed character
        for(int i = 0; i < charManager.characterDataSOs.Length; i++)
        {
            if (charManager.characterDataSOs[i].lastCursedCharacter)
            {
                lastCursedIndex = i;
            }
            if (charManager.characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                playerIndex = i;
            }
        }

        roundIndex = GameManager.Instance.GetCurrentRound();
    }

    public void StartDancerDialogue() // called from signal emmitter on Timeline
    {
        if (roundIndex < 4)
        {
            dialogueStarted = true;

            button0.gameObject.SetActive(true); // button 0 is always in use, for skipping + yes/no options
                                                // Select button0
            EventSystem.current.SetSelectedGameObject(button0.gameObject);
            button0.Select();
            button0clicked = false;
            button1clicked = false;
        }
        if (roundIndex == 4) // this means its the rave scene
        {
            Debug.Log("Rave Scene...");
            dialogueStarted = false; // Update loop will not happen
        }
    }

    void Update()
    {
        if (!dialogueStarted)
        {
            return;
        }
        else
        {
            float averageTeamCurse = curseManager.GetAverageTeamInfection();
            //Debug.Log("Average Team Curse: " + averageTeamCurse);
            switch (roundIndex)
            {
                case -1: // no dialogue/ not started
                    break;

                case 0:
                    //Round 1 Dialogue [Neutral Character is cursed]
                    if (dialogueLineCount == 0)
                    {
                        CursedCharResponse("what's happening?");
                        dialogueLineCount = 1;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 1)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Gud1, "oh no");
                        dialogueLineCount = 2;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 2)
                    {
                        PlayerResponse("what?", "");
                        dialogueLineCount = 3;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 3)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Gud2, "it's the curse");
                        dialogueLineCount = 4;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 4)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Bent2, "what curse?!");
                        dialogueLineCount = 5;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 5)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Gud1, "THE CURSE OF THE RAVE DEMON!");
                        dialogueLineCount = 6;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 6)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Bent1, "ooh that sounds wicked");
                        dialogueLineCount = 7;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 7)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Gud1, "it's messing with our moves!");
                        dialogueLineCount = 8;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 8)
                    {
                        PlayerResponse("what can we do?", "");
                        dialogueLineCount = 9;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 9)
                    {
                        NPCResponse(CharacterData.CharacterAlignment.Gud1, "debug! fix it!");
                        dialogueLineCount = 10;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 10)
                    {
                        DebugChoice("keep dancing", "fix");
                        dialogueLineCount = 11;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    //this is the debug choice
                    if (dialogueLineCount == 11 && button0clicked)
                    {
                        stopDanceScript.StopTheDance();
                        dialogueLineCount = 12;
                        button0clicked = false;
                    }
                    // debug choice 2
                    if (dialogueLineCount == 11 && button1clicked)
                    {
                        StartDebugGame();
                        dialogueLineCount = 12;
                        button1clicked = false;
                    }
                    // this is the end
                    if (dialogueLineCount == 12 && !debugRunning) // this gets cued after debug game, not by button click
                    {
                        // we need to check the cursed level of the last character
                        curseTolerance = 0.125f; // the first round the max average is 0.25, so this is half

                        if (averageTeamCurse >= curseTolerance) 
                        {
                            NPCResponse(CharacterData.CharacterAlignment.Gud1, "i don't know how we will win like this");
                            dialogueLineCount = 13;
                            button0clicked = false; // ensures it doesn't skip to next line in same frame
                        }
                        if (averageTeamCurse < 0.125f) // the first round the max average is 0.25, so this is half
                        {
                            NPCResponse(CharacterData.CharacterAlignment.Gud1, "ok i guess you're trying your best");
                            dialogueLineCount = 13;
                            button0clicked = false; // ensures it doesn't skip to next line in same frame
                        }
                    }
                    if (button0clicked && dialogueLineCount == 13)
                    {
                        EndSceneChoice("let's keep dancing", "");
                        dialogueLineCount = 14; 
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 14)
                    {
                        //END THE SCENE
                        dialogueLineCount = -1;
                        stopDanceScript.StopTheDance();
                    }
                    break;

                case 1:
                    //Round 2 Dialogue [Bent Char is cursed - positive reaction]
                    if (dialogueLineCount == 0)
                    {
                        CursedCharResponse("whoa check out these moves!");
                        dialogueLineCount = 1;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }

                    if (button0clicked && dialogueLineCount == 1)
                    {
                        DebugChoice("keep dancing", "fix");
                        dialogueLineCount = 11;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    //this is the debug choice
                    if (dialogueLineCount == 11 && button0clicked)
                    {
                        stopDanceScript.StopTheDance();
                        dialogueLineCount = 12;
                        button0clicked = false;
                    }
                    // debug choice 2
                    if (dialogueLineCount == 11 && button1clicked)
                    {
                        StartDebugGame();
                        dialogueLineCount = 12;
                        button1clicked = false;
                    }

                    if (dialogueLineCount == 12 && !debugRunning)
                    {
                        EndSceneChoice("gotta keep dancing", "");
                        dialogueLineCount = 14;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 14)
                    {
                        //END THE SCENE
                        dialogueLineCount = -1;
                        stopDanceScript.StopTheDance();
                    }
                    break;

                case 2:
                    //Round 3 Dialogue [Gud Char is Cursed - angsty response]
                    curseTolerance = 0.3f;

                    if (dialogueLineCount == 0)
                    {
                        // Here we need to track if player has been debuggin the team or letting things get bent...
                        // because if game is played straight, the character will respond straight
                        // or if game is played bent, character will respond more positively
                        if(averageTeamCurse >= curseTolerance) // *** adjust this if needed
                            CursedCharResponse("i'm not sure, but i think i like this");
                        else
                            CursedCharResponse("whyyyyyy?!"); 
                        dialogueLineCount = 1;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }

                    if (button0clicked && dialogueLineCount == 1)
                    {
                        DebugChoice("keep dancing", "fix");
                        dialogueLineCount = 11;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    //this is the debug choice
                    if (dialogueLineCount == 11 && button0clicked)
                    {
                        stopDanceScript.StopTheDance();
                        dialogueLineCount = 12;
                        button0clicked = false;
                    }
                    // debug choice 2
                    if (dialogueLineCount == 11 && button1clicked)
                    {
                        StartDebugGame();
                        dialogueLineCount = 12;
                        button1clicked = false;
                    }

                    if (dialogueLineCount == 12 && !debugRunning)
                    {
                        EndSceneChoice("one more move", "");
                        dialogueLineCount = 14;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 14)
                    {
                        //END THE SCENE
                        dialogueLineCount = -1;
                        stopDanceScript.StopTheDance();
                    }
                    break;

                case 3:
                    //Round 4 Dialogue [ Gud 1 is cursed, either turns proud cursed or stays extra gud ]
                    curseTolerance = 0.4f; // *** adjust this if needed

                    if (dialogueLineCount == 0)
                    {
                        // Again, we need to track if player has been debuggin the team or letting things get bent...
                        // because if game is played straight, the character will respond straight
                        // or if game is played bent, character will respond more positively
                        if (averageTeamCurse >= curseTolerance) 
                            CursedCharResponse("i've never been this messy");
                        else
                            CursedCharResponse("i feel so stupid");
                        dialogueLineCount = 1;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 1)
                    {
                        if (averageTeamCurse >= curseTolerance)
                            PlayerResponse("i think you look fabulous", "");
                        else
                            PlayerResponse("i'll straighten this out", "i think you look fabulous");
                        dialogueLineCount = 2;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 2)
                    {
                        if (averageTeamCurse >= curseTolerance)
                            CursedCharResponse("...i...think... this might be ...the new me");
                        else
                            CursedCharResponse("let's beat this game");
                        dialogueLineCount = 3;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button1clicked && dialogueLineCount == 2)
                    {
                        if (averageTeamCurse >= curseTolerance)
                            CursedCharResponse("...i...think... this might be ...the new me");
                        else
                            CursedCharResponse("i'm not used to getting messy");
                        dialogueLineCount = 3;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }

                    if (button0clicked && dialogueLineCount == 3)
                    {
                        DebugChoice("keep dancing", "straighten things out");
                        dialogueLineCount = 11;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    //this is the debug choice
                    if (dialogueLineCount == 11 && button0clicked)
                    {
                        stopDanceScript.StopTheDance();
                        dialogueLineCount = 12;
                        button0clicked = false;
                    }
                    // debug choice 2
                    if (dialogueLineCount == 11 && button1clicked)
                    {
                        StartDebugGame();
                        dialogueLineCount = 12;
                        button1clicked = false;
                    }

                    if (dialogueLineCount == 12 && !debugRunning)
                    {
                        EndSceneChoice("it's time...", "");
                        dialogueLineCount = 14;
                        button0clicked = false; // ensures it doesn't skip to next line in same frame
                    }
                    if (button0clicked && dialogueLineCount == 14)
                    {
                        //END THE SCENE
                        dialogueLineCount = -1;
                        stopDanceScript.StopTheDance();
                    }
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
        button0.Select(); // button 0 is already active
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
        button0.Select(); // button 0 is already active
        button1.gameObject.SetActive(false);

        // flag the corresponding virtual camera related to current character's index
        camManager.SetCamera(lastCursedIndex);
        dialogueText.text = dialogueLine;
        button0Text.text = ">";
    }
    void PlayerResponse(string dialogueLine0, string dialogueLine1)
    {
        dialoguePanel.SetActive(false);
        button0.Select(); // button 0 is already active
        if (dialogueLine1 != "") // only need this option if there are two responses
            button1.gameObject.SetActive(true);

        camManager.SetCamera(playerIndex);
        // dialogue
        button0Text.text = dialogueLine0;
        if (dialogueLine1 != "")
            button1Text.text = dialogueLine1;
    }
    void DebugChoice(string choiceText1, string choiceText2)
    {
        dialoguePanel.SetActive(false);
        button0.Select(); // button 0 is already active
        button1.gameObject.SetActive(true);

        camManager.SetCamera(playerIndex);
        // dialogue
        button0Text.text = choiceText1;
        button1Text.text = choiceText2;
    }
    public void Button0Clicked()
    {
        button0clicked = true;
    }
    public void Button1Clicked()
    {
        button1clicked = true;
    }

    void StartDebugGame()
    {
        debuggerUIParent.SetActive(true);
        renderTexture.SetActive(false);

        debugUI_Manager.StartDebugUI();

        debugRunning = true;
    }
    void EndDebugHandler()
    {
        debugRunning = false;

        debuggerUIParent.SetActive(false);
        renderTexture.SetActive(true);

        button0.gameObject.SetActive(true); // button 0 is always in use, for skipping + yes/no options
        // Select button0
        EventSystem.current.SetSelectedGameObject(button0.gameObject);
        button0.Select();
        button0clicked = false;
        button1clicked = false;
    }
    void EndSceneChoice(string dialogueLine0, string dialogueLine1)
    {
        dialoguePanel.SetActive(false);
        button0.Select(); // button 0 is already active
        if (dialogueLine1 != "") // only need this option if there are two responses
            button1.gameObject.SetActive(true);
        else
            button1.gameObject.SetActive(false); //just making sure it's off

        // flag the corresponding virtual camera related to current character's index
        camManager.SetCamera(playerIndex);
        // dialogue
        button0Text.text = dialogueLine0;
        if (dialogueLine1 != "")
            button1Text.text = dialogueLine1;
    }
}
