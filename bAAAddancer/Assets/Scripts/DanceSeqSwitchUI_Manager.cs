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

    Color imageDisabledColor = new(0.32f, 0f, 0.51f, 0.1f);
    Color imageEnabledColor = new(0.32f, 0f, 0.51f, 0.7f);
    Color imageSelectedColor = new(0.68f, 0.14f, 1f, 0.7f);

    Color textDisabledColor = new(0.5f, 1f, 0f, 0.1f);
    Color textEnabledColor = new(0.5f, 1f, 0f, 1f);

    bool switchingUI_Image = false;
    int imageSwitcherIndex = -1;

    [SerializeField] private GameObject skipPrevNextButtons;

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
        if (skipPrevNextButtons != null)
            skipPrevNextButtons.SetActive(false);
        int currentRoundIndex = GameManager.Instance.GetCurrentRound();
        imageSwitcherIndex = currentRoundIndex;

        if (skipPrevNextButtons != null) 
        {
            if (currentRoundIndex >= 1) skipPrevNextButtons.SetActive(true); // only need this visual if there are more than 1 option
        }
        

        for (int i = 0; i < seqSwitchImages.Length; i++) 
        {
            seqSwitchImages[i].color = imageDisabledColor;
            seqSwitchTexts[i].color = textDisabledColor;

            if (i <= currentRoundIndex) 
            {
                seqSwitchImages[i].color = imageEnabledColor;
                seqSwitchTexts[i].color = textEnabledColor;
            }
        }
        seqSwitchImages[imageSwitcherIndex].color = imageSelectedColor;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !switchingUI_Image) //replace this with correct input logic
        {
            SwitchUI_Image();
        }
    }

    void SwitchUI_Image() 
    {
        switchingUI_Image = true;
        int currentRoundIndex = GameManager.Instance.GetCurrentRound();

        seqSwitchImages[imageSwitcherIndex].color = imageEnabledColor; // current index "selected" goes back to "enabled"

        imageSwitcherIndex++;
        if (imageSwitcherIndex > currentRoundIndex) imageSwitcherIndex = 0; // loop, also limit switch index to amount of rounds

        seqSwitchImages[imageSwitcherIndex].color = imageSelectedColor;

        switchingUI_Image = false;

    }

}
