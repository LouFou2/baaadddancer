using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDisplayControls : MonoBehaviour
{
    private ClockCounter clockCounter;

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
    }

    void Update()
    {
        int current_Q_Beat = clockCounter.GetCurrent_Q_Beat(); //the clock counts in quarter beats
        int current_Beat = clockCounter.GetCurrentBeat();

        Color beatLightOff = new Color(1f, 1f, 1f, 0.13f);
        Color beatLightOn = new Color(1f, 1f, 1f, 0.4f);
        Color beatHighlight = Color.green;

        for (int i = 0; i < beatLights.Length; i++) 
        {

            beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);

            beatLights[i].color = beatLightOff;
            beatLights[i].color = (i == current_Q_Beat) ? beatLightOn : beatLightOff;

            // Slightly different lights for each beat:
            if (i == current_Beat * 4 && current_Q_Beat % 4 == 0)
            {
                beatLights[i].rectTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                beatLights[i].color = beatHighlight;
            }
        }
    }
    
    public void EndScene() 
    {
        //switch to the next scene
        sceneSwitcher.LoadNextScene();
        sceneSwitcher.SwitchToNextLevelKey();
    }
}
