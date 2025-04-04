using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlObjectSwitcher : MonoBehaviour
{
    public enum ControlObjects { Pelvis, Legs, Hands, Shoulders, Head }

    private Color activeImageColor = new Color(0.7f, 0.4f, 1f, 0.15f); // Example active color (RGB and alpha)
    private Color inactiveImageColor = new Color(1f, 1f, 1f, 0.05f); // Example inactive color (RGB and alpha)
    private Color activeTextColor = new Color(0.1f, 1f, 0f, 1.0f); 
    private Color inactiveTextColor = new Color(0f, 0f, 0f, 0.7f);

    [Serializable]
    public class ControlObjectData
    {
        public ControlObjects controlObject;
        public ObjectControls[] controlScripts;
        public Image uiImageVisualiser;
        public TextMeshProUGUI uiTextVisualiser;
    }

    [SerializeField] private ControlObjectData[] controlObjectData;

    [SerializeField] private GameObject finishedButton;

    private int currentObjectIndex = 0;

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
    void Update()
    {
        bool switchToNext = playerControls.DanceControls.SwitchObjectR.triggered;
        bool switchToPrevious = playerControls.DanceControls.SwitchObjectL.triggered;

        if (switchToNext)
        {
            currentObjectIndex++;
            if (currentObjectIndex >= Enum.GetNames(typeof(ControlObjects)).Length) // *** COULD I USE controlObjectData.Length INSTEAD?
                currentObjectIndex = 0;
        }
        if (switchToPrevious)
        {
            currentObjectIndex--;
            if (currentObjectIndex < 0)
                currentObjectIndex = Enum.GetNames(typeof(ControlObjects)).Length - 1; // *** IT COULD ENSURE IT DOESNT HAVE BLANK ITEMS WHEN SWITCHING
        }

        ControlObjects currentObject = (ControlObjects)currentObjectIndex;

        switch (currentObject)
        {
            case ControlObjects.Pelvis:
            case ControlObjects.Legs:
            case ControlObjects.Hands:
            case ControlObjects.Shoulders:
            case ControlObjects.Head:
                for (int i = 0; i < controlObjectData.Length; i++)
                {
                    foreach (ObjectControls controlScript in controlObjectData[i].controlScripts)
                    {
                        controlScript.isActive = (i == currentObjectIndex);

                        // Set colors based on the current control object
                        bool isActiveControl = (i == currentObjectIndex);
                        if (controlObjectData[i].uiImageVisualiser != null)
                        {
                            // Change color of the Image component
                            controlObjectData[i].uiImageVisualiser.color = isActiveControl ? activeImageColor : inactiveImageColor;
                        }
                        if (controlObjectData[i].uiTextVisualiser != null)
                        {
                            // Change color of the TextMeshProUGUI text
                            controlObjectData[i].uiTextVisualiser.color = isActiveControl ? activeTextColor : inactiveTextColor;
                        }
                    }
                }
                // Activate button if the current control object is Head
                if (finishedButton != null)
                {
                    if (currentObject == ControlObjects.Head) finishedButton.SetActive(true);
                    else finishedButton.SetActive(false);
                }
                break;
            default:
                Debug.LogError("Unknown control object!");
                break;
        }
    }
}
