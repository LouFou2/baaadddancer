using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DanceSeqSwitchUI_Manager : MonoBehaviour
{
    [SerializeField] private Image[] seqSwitchImages; //assign UI buttons in inspector
    [SerializeField] private TextMeshProUGUI[] seqSwitchTexts; //assign UI buttons in inspector
    private PlayerControls playerControls;

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

    private void Start()
    {
        int currentRoundIndex = GameManager.Instance.GetCurrentRound();

        for (int i = 0; i < seqSwitchImages.Length; i++) 
        {
            // Set the button to non-interactable
            Color imageDisabledColor = new(0.32f, 0f, 0.51f, 0.1f);
            Color imageEnabledColor = new(0.32f, 0f, 0.51f, 0.7f);
            Color textDisabledColor = new(0.5f, 1f, 0f, 0.1f);
            Color textEnabledColor = new(0.5f, 1f, 0f, 1f);

            seqSwitchImages[i].color = imageDisabledColor;
            seqSwitchTexts[i].color = textDisabledColor;

            if (i <= currentRoundIndex) 
            {
                seqSwitchImages[i].color = imageEnabledColor;
                seqSwitchTexts[i].color = textEnabledColor;
            }
        }
    }

}
