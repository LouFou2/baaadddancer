using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDisplayControls : MonoBehaviour
{
    private ClockCounter clockCounter;
    private bool isRunning = false; // To control whether the tutorial is running or not
    private float sequenceDuration;
    private PlayerControls playerControls;
    [SerializeField] private GameObject leftStick;
    [SerializeField] private GameObject rightStick;
    [SerializeField] private GameObject leftBumper;
    [SerializeField] private GameObject rightBumper;
    [SerializeField] private GameObject leftTrigger;
    [SerializeField] private GameObject rightTrigger;
    [SerializeField] private GameObject d_Pad;
    [SerializeField] private Image[] beatLights;

    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private TextMeshProUGUI rightButtonText;
    [SerializeField] private TextMeshProUGUI viewText;

    private enum TutorialState { useJoysticks, useObjectSwitch, useViewSwitch, useRecord, usePlay }
    private TutorialState CurrentTutorialState = TutorialState.useJoysticks;

    bool isRecording, isPlaying, switchObject = false;

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
        clockCounter = FindObjectOfType<ClockCounter>(); // Find the ClockCounter script in the scene
        sequenceDuration = clockCounter.GetBeatInterval() * 16; //*** THIS IS RETURNING 0 *** (would be nice to use)
        Debug.Log(sequenceDuration);
        CurrentTutorialState = TutorialState.useJoysticks;
        StartTutorial();
    }

    void Update()
    {
        int current_Q_Beat = clockCounter.GetCurrent_Q_Beat(); //the clock counts in quarter beats
        int current_Beat = clockCounter.GetCurrentBeat();

        Color beatLightOff = new Color(1f, 1f, 1f, 0.13f);
        Color beatLightOff_Recording = new Color(1f, 0.627f, 0.637f, 0.2f);
        Color beatLightOff_Playback = new Color(0f, 1f, 1f, 0.2f);
        Color beatLightOn = new Color(1f, 1f, 1f, 0.4f);
        Color beatLightOn_Recording = new Color(1f, 0.627f, 0.637f, 0.4f);
        Color beatLightOn_Playback = new Color(0f, 1f, 1f, 0.4f);
        Color beatHighlight = new Color(1f, 1f, 1f, 0.6f);
        Color beatHighlight_Recording = new Color(1f, 0.627f, 0.637f, 0.6f);
        Color beatHighlight_Playback = new Color(0f, 1f, 1f, 0.6f);

        isRecording = playerControls.DanceControls.Record.IsPressed();
        isPlaying = playerControls.DanceControls.Play.triggered;
        switchObject = (playerControls.DanceControls.SwitchObjectL.triggered) ? true : (playerControls.DanceControls.SwitchObjectR.triggered) ? true : false;

        for (int i = 0; i < beatLights.Length; i++) 
        {

            beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);

            if (i== current_Q_Beat) // light display for each quarter beat
            {
                if (isRecording)
                {
                    beatLights[i].color = beatLightOff_Recording;
                    beatLights[i].color = (i == current_Q_Beat) ? beatLightOn_Recording : beatLightOff_Recording;
                    beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);
                }
                else if (isPlaying)
                {
                    beatLights[i].color = beatLightOff_Playback;
                    beatLights[i].color = (i == current_Q_Beat) ? beatLightOn_Playback : beatLightOff_Playback;
                    beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);
                }
                else
                {
                    beatLights[i].color = beatLightOff;
                    //beatLights[i].color = (i == current_Q_Beat) ? beatLightOn : beatLightOff;
                    beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);
                }
            }

            // Slightly different lights for each beat:
            if (i == current_Beat * 4 && current_Q_Beat % 4 == 0)
            {
                beatLights[i].rectTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                beatLights[i].color = (isRecording)? beatHighlight_Recording : (isPlaying)? beatHighlight_Playback : beatHighlight;
            }
        }
        switch (CurrentTutorialState) 
        {
            case TutorialState.useJoysticks:
                leftStick.SetActive(true); // joysticks true
                rightStick.SetActive(true); // joysticks true
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(false);
                d_Pad.SetActive(false);
                break;
            case TutorialState.useObjectSwitch:
                leftStick.SetActive(false);
                rightStick.SetActive(false);
                leftBumper.SetActive(true); // leftBumper true
                rightBumper.SetActive(true); // rightBumper true
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(false);
                d_Pad.SetActive(false);
                break;
            case TutorialState.useViewSwitch:
                leftStick.SetActive(false);
                rightStick.SetActive(false);
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(false);
                d_Pad.SetActive(true); // d_Pad true
                break;
            case TutorialState.useRecord:
                leftStick.SetActive(true);
                rightStick.SetActive(true);
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(true); // leftTrigger true
                rightTrigger.SetActive(false);
                d_Pad.SetActive(false);
                break;
            case TutorialState.usePlay:
                leftStick.SetActive(false);
                rightStick.SetActive(false);
                leftBumper.SetActive(false);
                rightBumper.SetActive(false);
                leftTrigger.SetActive(false);
                rightTrigger.SetActive(true); // rightTrigger true
                d_Pad.SetActive(false);
                break;
            default:
                CurrentTutorialState = TutorialState.useJoysticks;
                break;
        }
    }
    public void StartTutorial() 
    {
        isRunning = true;
        StartCoroutine(LearningControls());
    }
    public void EndTutorial()
    {
        isRunning = false;
        StopCoroutine(LearningControls());
    }
    private IEnumerator LearningControls()
    {
        while (isRunning) 
        {
            CurrentTutorialState = TutorialState.useJoysticks;
            rightButtonText.text = "swivel that pelvis";
            yield return new WaitForSeconds(4f); // adjust time to wait here

            CurrentTutorialState = TutorialState.useRecord;
            leftButtonText.text = "hold left trigger to record your moves";
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);

            CurrentTutorialState = TutorialState.usePlay;
            rightButtonText.text = "tap right trigger to play back";
            yield return new WaitUntil(playerControls.DanceControls.Play.IsPressed);

            CurrentTutorialState = TutorialState.useObjectSwitch;
            rightButtonText.text = "tap right bumper to move your feet";
            yield return new WaitUntil(() => switchObject);
            isPlaying = false;

            // --- Tutorial Loop for Each Object Control e.g. [legs]
            CurrentTutorialState = TutorialState.useJoysticks;
            leftButtonText.text = "show us some fancy footwork";
            yield return new WaitForSeconds(4f);

            CurrentTutorialState = TutorialState.useRecord;
            leftButtonText.text = "hold to record";
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);

            CurrentTutorialState = TutorialState.usePlay;
            rightButtonText.text = "tap to play back";
            yield return new WaitUntil(playerControls.DanceControls.Play.IsPressed);

            CurrentTutorialState = TutorialState.useObjectSwitch;
            rightButtonText.text = "next: hands";
            yield return new WaitUntil(() => switchObject);
            isPlaying = false;

            // --- Another Loop --- e.g. [hands]
            CurrentTutorialState = TutorialState.useJoysticks;
            leftButtonText.text = "dance hands!";
            rightButtonText.text = "dance hands!";
            yield return new WaitForSeconds(4f);

            CurrentTutorialState = TutorialState.useRecord;
            leftButtonText.text = "hold to record";
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);

            CurrentTutorialState = TutorialState.usePlay;
            rightButtonText.text = "tap it!";
            yield return new WaitForSeconds(10f);

            CurrentTutorialState = TutorialState.useObjectSwitch;
            leftButtonText.text = "";
            yield return new WaitUntil(() => switchObject);
            isPlaying = false;

            // --- Another Loop --- e.g. [torso] *** Introducing view Switch
            CurrentTutorialState = TutorialState.useViewSwitch;
            yield return new WaitUntil(() => playerControls.DanceControls.RotateViewX_Top.triggered);
            // add logic for signalling (e.g. animation, text)  which button to press
            yield return new WaitUntil(() => playerControls.DanceControls.RotateViewY_Left.triggered);
            // add logic for signalling (e.g. animation, text)  which button to press
            yield return new WaitUntil(() => playerControls.DanceControls.RotateViewY_Right.triggered);
            // add logic for signalling (e.g. animation, text)  which button to press
            yield return new WaitUntil(() => playerControls.DanceControls.RotateViewX_Front.triggered);

            CurrentTutorialState = TutorialState.useJoysticks;
            yield return new WaitForSeconds(4f);

            CurrentTutorialState = TutorialState.useRecord;
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);

            CurrentTutorialState = TutorialState.usePlay;
            yield return new WaitForSeconds(10f);

            CurrentTutorialState = TutorialState.useObjectSwitch;
            yield return new WaitUntil(() => switchObject);
            isPlaying = false;

            // --- Another Loop --- e.g. [head]
            CurrentTutorialState = TutorialState.useJoysticks;
            yield return new WaitForSeconds(4f);

            CurrentTutorialState = TutorialState.useRecord;
            yield return new WaitUntil(playerControls.DanceControls.Record.IsPressed);
            yield return new WaitWhile(playerControls.DanceControls.Record.IsPressed);

            CurrentTutorialState = TutorialState.usePlay;
            yield return new WaitForSeconds(10f);

            CurrentTutorialState = TutorialState.useObjectSwitch;
            yield return new WaitUntil(() => switchObject);
            isPlaying = false;

            EndTutorial();
        }
    }
}
