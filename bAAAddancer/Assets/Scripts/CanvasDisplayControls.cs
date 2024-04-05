using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDisplayControls : MonoBehaviour
{
    private ClockCounter clockCounter;
    private bool tutorialIsRunning = false; // To control whether the tutorial is running or not
    private float sequenceDuration;
    private PlayerControls playerControls;
    private SceneSwitcher sceneSwitcher;
    [SerializeField] private GameObject leftStick;
    [SerializeField] private GameObject rightStick;
    [SerializeField] private GameObject leftBumper;
    [SerializeField] private GameObject rightBumper;
    [SerializeField] private GameObject leftTrigger;
    [SerializeField] private GameObject rightTrigger;
    [SerializeField] private GameObject d_Pad;
    [SerializeField] private GameObject xButton;
    [SerializeField] private GameObject yButton;
    [SerializeField] private Image[] beatLights;

    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private TextMeshProUGUI rightButtonText;
    [SerializeField] private TextMeshProUGUI viewText;
    [SerializeField] private TextMeshProUGUI generalText;
    [SerializeField] private TextMeshProUGUI yesText;
    [SerializeField] private TextMeshProUGUI noText;

    private enum SceneState { SkipTutorial, UseJoysticks, UseObjectSwitch, UseViewSwitch, UseRecord, UsePlay, SaveOrContinueDance, KeepDancing }
    private SceneState CurrentSceneState;

    bool skipTutorial, isRecording, isPlaying, switchObject, yesOrNo, savedDance, keepDancing = false;

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
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();
        if (sceneSwitcher == null)
        {
            Debug.LogError("SceneSwitcher component not found in the scene.");
        }
        clockCounter = FindObjectOfType<ClockCounter>(); // Find the ClockCounter script in the scene
        if (clockCounter == null)
        {
            Debug.LogError("ClockCounter component not found in the scene.");
        }
        sequenceDuration = clockCounter.GetBeatInterval() * 16; //*** THIS IS RETURNING 0 *** (would be nice to use)
        Debug.Log(sequenceDuration);
        CurrentSceneState = SceneState.SkipTutorial;
        //StartTutorial();
    }

    void Update()
    {
        int current_Q_Beat = clockCounter.GetCurrent_Q_Beat(); //the clock counts in quarter beats
        int current_Beat = clockCounter.GetCurrentBeat();

        Color beatLightOff = new Color(1f, 1f, 1f, 0.13f);
        Color beatLightOn = new Color(1f, 1f, 1f, 0.4f);
        Color beatHighlight = new Color(1f, 1f, 1f, 0.6f);

        /*//isRecording = playerControls.DanceControls.Record.IsPressed();
        switchObject = (playerControls.DanceControls.SwitchObjectL.triggered) ? true : (playerControls.DanceControls.SwitchObjectR.triggered) ? true : false;
        yesOrNo = (playerControls.DanceControls.YesButton.triggered) ? true : (playerControls.DanceControls.NoButton.triggered) ? true : false;*/

        for (int i = 0; i < beatLights.Length; i++) 
        {

            beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);

            if (i== current_Q_Beat) // light display for each quarter beat
            {
                
                    beatLights[i].color = beatLightOff;
                    beatLights[i].color = (i == current_Q_Beat) ? beatLightOn : beatLightOff;
                    beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);
                
            }

            // Slightly different lights for each beat:
            if (i == current_Beat * 4 && current_Q_Beat % 4 == 0)
            {
                beatLights[i].rectTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                beatLights[i].color = beatHighlight;
            }
        }
        /*switch (CurrentSceneState) 
        {
            case SceneState.SkipTutorial:
                leftStick.SetActive(false);
                rightStick.SetActive(false);
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(false);
                d_Pad.SetActive(false);
                xButton.SetActive(true);
                yButton.SetActive(true);
                skipTutorial = (playerControls.DanceControls.YesButton.triggered) ? true : (playerControls.DanceControls.NoButton.triggered) ? false : false;
                if(skipTutorial) CurrentSceneState = SceneState.KeepDancing;
                break;
            case SceneState.UseJoysticks:
                leftStick.SetActive(true); // joysticks true
                rightStick.SetActive(true); // joysticks true
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(false);
                d_Pad.SetActive(false);
                break;
            case SceneState.UseObjectSwitch:
                leftStick.SetActive(false);
                rightStick.SetActive(false);
                leftBumper.SetActive(true); // leftBumper true
                rightBumper.SetActive(true); // rightBumper true
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(false);
                d_Pad.SetActive(false);
                break;
            case SceneState.UseViewSwitch:
                leftStick.SetActive(false);
                rightStick.SetActive(false);
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(false);
                d_Pad.SetActive(true); // d_Pad true
                break;
            case SceneState.UseRecord:
                leftStick.SetActive(true);
                rightStick.SetActive(true);
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(true); // leftTrigger true
                rightTrigger.SetActive(false);
                d_Pad.SetActive(false);
                break;
            case SceneState.UsePlay:
                leftStick.SetActive(false);
                rightStick.SetActive(false);
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(true); // rightTrigger true
                d_Pad.SetActive(false);
                break;
            case SceneState.SaveOrContinueDance:
                leftStick.SetActive(false); 
                rightStick.SetActive(false);
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(false); 
                d_Pad.SetActive(false);
                xButton.SetActive(true);
                yButton.SetActive(true);
                savedDance = (playerControls.DanceControls.YesButton.triggered) ? true : (playerControls.DanceControls.NoButton.triggered) ? false : false;
                keepDancing = (playerControls.DanceControls.NoButton.triggered) ? true : (playerControls.DanceControls.YesButton.triggered) ? false : false;
                break;
            case SceneState.KeepDancing:
                if (skipTutorial)
                {
                    leftStick.SetActive(false);
                    rightStick.SetActive(false);
                    leftBumper.SetActive(false);
                    rightBumper.SetActive(false);
                    leftTrigger.SetActive(false);
                    rightTrigger.SetActive(false);
                    d_Pad.SetActive(false);
                    xButton.SetActive(false);
                    yButton.SetActive(false);
                }
                else 
                {
                    leftStick.SetActive(true);
                    rightStick.SetActive(true);
                    leftBumper.SetActive(true);
                    rightBumper.SetActive(true);
                    leftTrigger.SetActive(true);
                    rightTrigger.SetActive(true);
                    d_Pad.SetActive(true);
                    xButton.SetActive(true);
                    yButton.SetActive(false);
                    yesText.text = "press x when done";
                }
                
                savedDance = false;
                savedDance = (playerControls.DanceControls.YesButton.triggered) ? true : (playerControls.DanceControls.NoButton.triggered) ? false : false;
                if (savedDance) EndScene();
                break;
            default:
                CurrentSceneState = SceneState.UseJoysticks;
                break;
        }*/
    }
    /*public void StartTutorial() 
    {
        tutorialIsRunning = true;
        StartCoroutine(LearningControls());
    }
    public void EndTutorial()
    {
        tutorialIsRunning = false;
        StopCoroutine(LearningControls());
        if (keepDancing) CurrentSceneState = SceneState.KeepDancing;
        if (savedDance) EndScene();
    }*/
    public void EndScene() 
    {
        //switch to the next scene
        sceneSwitcher.LoadNextScene();
        sceneSwitcher.SwitchToNextLevelKey();
    }
/*    private IEnumerator LearningControls()
    {
        while (tutorialIsRunning) 
        {
            // -- Check if Skip Tutorial
            CurrentSceneState = SceneState.SkipTutorial;
            generalText.text = "need to learn the controls?";
            noText.text = "show me the controls";
            yesText.text = "skip";
            yield return new WaitUntil(() => yesOrNo);
            generalText.text = "";
            noText.text = "";
            yesText.text = "";
            if (skipTutorial) 
            {
                EndTutorial();
                yield break;
            }
            
            // -- pelvis is first active object
            CurrentSceneState = SceneState.UseJoysticks;
            rightButtonText.text = "swivel that pelvis";
            yield return new WaitForSeconds(4f); // adjust time to wait here
            rightButtonText.text = "";

            CurrentSceneState = SceneState.UseRecord;
            leftButtonText.text = "hold left trigger to record your moves";
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);
            leftButtonText.text = "";

            CurrentSceneState = SceneState.UsePlay;
            rightButtonText.text = "tap right trigger to play back";
            yield return new WaitUntil(playerControls.DanceControls.Play.IsPressed);
            rightButtonText.text = "";
            isPlaying = true; // playback indicating (colour change)

            CurrentSceneState = SceneState.UseObjectSwitch;
            rightButtonText.text = "tap right bumper to move your feet";
            yield return new WaitUntil(() => switchObject);
            rightButtonText.text = "";
            isPlaying = false; // playback stops indicating for next object (colour change back)

            // --- Tutorial Loop for Each Object Control e.g. [legs]
            CurrentSceneState = SceneState.UseJoysticks;
            leftButtonText.text = "show us some fancy footwork";
            yield return new WaitForSeconds(4f);
            leftButtonText.text = "";

            CurrentSceneState = SceneState.UseRecord;
            leftButtonText.text = "hold to record";
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);
            leftButtonText.text = "";

            CurrentSceneState = SceneState.UsePlay;
            rightButtonText.text = "tap to play back";
            yield return new WaitUntil(playerControls.DanceControls.Play.IsPressed);
            isPlaying = true;

            CurrentSceneState = SceneState.UseObjectSwitch;
            rightButtonText.text = "next: hands";
            yield return new WaitUntil(() => switchObject);
            rightButtonText.text = "";
            isPlaying = false;

            // --- Another Loop --- e.g. [hands]
            CurrentSceneState = SceneState.UseJoysticks;
            leftButtonText.text = "dance hands!";
            rightButtonText.text = "dance hands!";
            yield return new WaitForSeconds(4f);
            rightButtonText.text = "";
            leftButtonText.text = "";

            CurrentSceneState = SceneState.UseRecord;
            leftButtonText.text = "hold to record";
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);
            leftButtonText.text = "";

            CurrentSceneState = SceneState.UsePlay;
            rightButtonText.text = "tap it!";
            yield return new WaitUntil(playerControls.DanceControls.Play.IsPressed);
            rightButtonText.text = "";
            isPlaying = true;

            CurrentSceneState = SceneState.UseObjectSwitch;
            leftButtonText.text = "";
            yield return new WaitUntil(() => switchObject);
            leftButtonText.text = "";
            isPlaying = false;

            // --- Another Loop --- e.g. [torso] *** Introducing view Switch
            CurrentSceneState = SceneState.UseViewSwitch;
            leftButtonText.text = "You can use different views";
            yield return new WaitForSeconds(3f);
            leftButtonText.text = "Try the top view (press up)";
            yield return new WaitUntil(() => playerControls.DanceControls.RotateViewX_Top.triggered);
            leftButtonText.text = "Try the left view (press left)";
            yield return new WaitUntil(() => playerControls.DanceControls.RotateViewY_Left.triggered);
            leftButtonText.text = "Try the right view (press right)";
            yield return new WaitUntil(() => playerControls.DanceControls.RotateViewY_Right.triggered);
            leftButtonText.text = "Now back to the front (press front)";
            yield return new WaitUntil(() => playerControls.DanceControls.RotateViewX_Front.triggered);
            leftButtonText.text = "";

            CurrentSceneState = SceneState.UseJoysticks;
            yield return new WaitForSeconds(4f);

            CurrentSceneState = SceneState.UseRecord;
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);

            CurrentSceneState = SceneState.UsePlay;
            yield return new WaitUntil(playerControls.DanceControls.Play.IsPressed);
            isPlaying = true;

            CurrentSceneState = SceneState.UseObjectSwitch;
            yield return new WaitUntil(() => switchObject);
            isPlaying = false;

            // --- Another Loop --- e.g. [head]
            CurrentSceneState = SceneState.UseJoysticks;
            yield return new WaitForSeconds(4f);

            CurrentSceneState = SceneState.UseRecord;
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);

            CurrentSceneState = SceneState.UsePlay;
            yield return new WaitUntil(playerControls.DanceControls.Play.IsPressed);
            isPlaying = true;

            CurrentSceneState = SceneState.SaveOrContinueDance;
            generalText.text = "Are you ready to save this dance?";
            yesText.text = "lock it in!";
            noText.text = "not yet...";
            yield return new WaitUntil(() => yesOrNo);
            generalText.text = "";
            yesText.text = "";
            noText.text = "";

            EndTutorial();
        }
    }
*/}
