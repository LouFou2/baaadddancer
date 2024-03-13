using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDisplayControls : MonoBehaviour
{
    private ClockCounter clockCounter;
    private PlayerControls playerControls;
    [SerializeField] private GameObject leftStick;
    [SerializeField] private GameObject rightStick;
    [SerializeField] private GameObject leftBumper;
    [SerializeField] private GameObject rightBumper;
    [SerializeField] private GameObject leftTrigger;
    [SerializeField] private GameObject rightTrigger;
    [SerializeField] private Image[] beatLights;


    void Start()
    {
        clockCounter = FindObjectOfType<ClockCounter>(); // Find the ClockCounter script in the scene
        playerControls = new PlayerControls();
    }

    void Update()
    {
        int current_Q_Beat = clockCounter.GetCurrent_Q_Beat(); //the clock counts in quarter beats
        int current_Beat = clockCounter.GetCurrentBeat();

        Color beatLightOff = new Color(1f, 1f, 1f, 0.13f);
        Color beatLightOn = new Color(1f, 1f, 1f, 0.4f);
        Color beatHighlight = new Color(1f, 1f, 1f, 0.6f);

        for (int i = 0; i < beatLights.Length; i++) 
        {
            beatLights[i].color = beatLightOff;
            beatLights[i].color = (i == current_Q_Beat) ? beatLightOn : beatLightOff;
            beatLights[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);

            //Stuff you can do on each beat:
            if (i == current_Beat * 4 && current_Q_Beat % 4 == 0)
            {
                beatLights[i].rectTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                beatLights[i].color = beatHighlight;
            }

        }
    }
}
