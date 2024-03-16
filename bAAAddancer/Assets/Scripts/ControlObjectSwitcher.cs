using System;
using UnityEngine;

public class ControlObjectSwitcher : MonoBehaviour
{
    public enum ControlObjects { Pelvis, Legs, Hands, Shoulders, Head }


    [Serializable]
    public class ControlObjectData
    {
        public ControlObjects controlObject;
        public ObjectControls[] controlScripts;
    }

    [SerializeField] private ControlObjectData[] controlObjectData;

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
                    }
                }
                break;
            default:
                Debug.LogError("Unknown control object!");
                break;
        }
    }
}
