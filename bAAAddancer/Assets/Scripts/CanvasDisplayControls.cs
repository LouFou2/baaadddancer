using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDisplayControls : MonoBehaviour
{
    private ClockCounter clockCounter;

    private PlayerControls playerControls;
    private SceneSwitcher sceneSwitcher;

    // Count In Objects
    [SerializeField] private GameObject[] counts;

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

    private bool isCountingIn = false;
    private int countInIndex = -1;
    private int displayCounterIndex = -1;

    [SerializeField] private Color beatLightOff = new Color(1f, 1f, 1f, 0.13f);
    [SerializeField] private Color beatLightOn = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private Color beatHighlight = Color.green;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        ClockCounter.On_Beat_Trigger += On_BeatHandler;
        ClockCounter.On_Q_Beat_Trigger += On_Q_BeatHandler;

    }
    private void OnDisable()
    {
        playerControls.Disable();
        ClockCounter.On_Beat_Trigger -= On_BeatHandler;
        ClockCounter.On_Q_Beat_Trigger -= On_Q_BeatHandler;
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

        isCountingIn = true;

        foreach (GameObject count in counts)
        {
            count.SetActive(false);
        }
    }

    private void On_BeatHandler()
    {
        if (isCountingIn)
        {
            countInIndex++;
            // animate / swap the count-in numbers (5-6-7-8...) is handled by the Canvas Animation Manager
            if (countInIndex == 0)
            {
                counts[0].SetActive(true);
            }
            if (countInIndex == 1)
            {
                counts[0].SetActive(false);
                counts[1].SetActive(true);
            }
            if (countInIndex == 2)
            {
                counts[1].SetActive(false);
                counts[2].SetActive(true);
            }
            if (countInIndex == 3)
            {
                counts[2].SetActive(false);
                counts[3].SetActive(true);
            }
            if (countInIndex == 4)
            {
                counts[3].SetActive(false);
                isCountingIn = false;
            }
        }
        
    }
    void On_Q_BeatHandler()
    {
        if (!isCountingIn)
        {
            displayCounterIndex++;

            if (displayCounterIndex == 64)
            {
                displayCounterIndex = 0; //63 is the last q count
            }

            UpdateTimerLights();
        }
    }
    void UpdateTimerLights()
    {
        for (int i = 0; i < beatLights.Length; i++)
        {
            beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);

            beatLights[i].color = beatLightOff;
            beatLights[i].color = (i == displayCounterIndex) ? beatLightOn : beatLightOff;

            // To Scale and Highlight the "Beat" lights:
            if (i == displayCounterIndex && i % 4 == 0)
            {
                beatLights[i].rectTransform.localScale = new Vector3(1f, 3.5f, 1f);
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
