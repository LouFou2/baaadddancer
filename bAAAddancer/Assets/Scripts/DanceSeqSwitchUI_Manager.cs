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
    int currentRoundIndex = -1;
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
        currentRoundIndex = GameManager.Instance.GetCurrentRound();
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
        
        if (playerControls.GenericInput.LBumper.triggered && !switchingUI_Image)
        {
            SwitchUI_Image(-1);
        }
        if (playerControls.GenericInput.RBumper.triggered && !switchingUI_Image)
        {
            SwitchUI_Image(1);
        }
    }

    void SwitchUI_Image(int posOrNegInt) 
    {
        switchingUI_Image = true;
        seqSwitchImages[imageSwitcherIndex].color = imageEnabledColor; // current index "selected" goes back to "enabled"

        imageSwitcherIndex += posOrNegInt;
        if (imageSwitcherIndex > currentRoundIndex) imageSwitcherIndex = 0; // loop, also limit switch index to amount of rounds
        if (imageSwitcherIndex < 0) imageSwitcherIndex = currentRoundIndex;

        seqSwitchImages[imageSwitcherIndex].color = imageSelectedColor;

        switchingUI_Image = false;
    }

}
