using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlObjectSwitcher : MonoBehaviour
{
    [SerializeField] private ObjectControls[] objectCntrlScripts;

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
        bool switchToNext = playerControls.DanceControls.SwitchObjectL.triggered;
        bool switchToPrevious = playerControls.DanceControls.SwitchObjectR.triggered;

        if (switchToNext) currentObjectIndex++;
        if (switchToPrevious) currentObjectIndex--;
 
        if(currentObjectIndex == objectCntrlScripts.Length) currentObjectIndex = 0;
        if(currentObjectIndex < 0) currentObjectIndex = objectCntrlScripts.Length - 1;
        Debug.Log("current index: " + currentObjectIndex);

        for (int i = 0; i < objectCntrlScripts.Length; i++)
        {
            objectCntrlScripts[i].isActive = (i == currentObjectIndex); // set it active if it is the current index
        }
    }
}
