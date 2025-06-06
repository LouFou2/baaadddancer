using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitScipt : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private GameObject exitPanel; // assign all these in inspector
    [SerializeField] private Image yesImage;
    [SerializeField] private Image noImage;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unSelectedColor;

    private bool exitOptionActive;
    private bool yesSelected;
    private bool noSelected;

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
        exitOptionActive = false;
        yesSelected = true;
        noSelected = false;

        exitPanel.SetActive(false);

        yesImage.color = selectedColor;
        noImage.color = unSelectedColor;
    }

    private void Update()
    {
        bool upPressed = false;
        bool downPressed = false;

        //only listening for "return" button
        if (playerControls.GenericInput.HomeButton.triggered)
        {
            exitOptionActive = !exitOptionActive; // toggle true false, (e.g. if home button gets pressed again, its false)
        }

        if (exitOptionActive == true)
        {
            exitPanel.SetActive(true);

            //now we can switch between "buttons" (they are actually just images)
            upPressed = playerControls.GenericInput.ButtonN.triggered ? true : false;
            downPressed = playerControls.GenericInput.ButtonS.triggered ? true : false;

            if (upPressed && !yesSelected)
            {
                yesSelected = true;
                noSelected = false;

                yesImage.color = selectedColor;
                noImage.color = unSelectedColor;
            }
            if (downPressed && !noSelected)
            {
                noSelected = true;
                yesSelected = false;

                yesImage.color = unSelectedColor;
                noImage.color = selectedColor;
            }

            if (yesSelected && playerControls.GenericInput.AButton.triggered)
            {
                GameManager.Instance.RestartGame();
            }
            if (noSelected && playerControls.GenericInput.AButton.triggered)
            {
                exitOptionActive = false;
                exitPanel.SetActive(false);
            }
        }
        else
        {
            exitPanel.SetActive(false);
        }

    }
    
}
