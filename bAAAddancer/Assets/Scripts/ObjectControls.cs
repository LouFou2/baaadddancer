using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectControls : MonoBehaviour
{
    private PlayerControls playerControls;
    private GameObject controlObject;
    [SerializeField] private CNTRL_Transforms_SO cntrl_Data_SO; // ***think I'm gonna forego the SO's
    [SerializeField] private bool leftObject = true;
    [SerializeField] private bool rightObject = false;
    public bool isActive = false;
    public bool useRecordedPositions = false;
    [SerializeField] private float x_Range;
    [SerializeField] private float y_Range;
    private Vector2 moveInput;

    private Vector3 initialPosition;
    private Vector3 initialRotation;

    // Flag to control whether to use input from the controller or values from the Scriptable Object
    private bool useControllerInput = true;


    private void Awake()
    {
        playerControls = new PlayerControls();
        controlObject = gameObject;
    }
    private void OnEnable()
    {
        playerControls.Enable();
        if (cntrl_Data_SO != null)
            cntrl_Data_SO.updated.AddListener(TransformUpdates);
    }
    private void OnDisable()
    {
        playerControls.Disable();
        if (cntrl_Data_SO != null)
            cntrl_Data_SO.updated.RemoveListener(TransformUpdates);
    }

    private void Start()
    {
        if (cntrl_Data_SO != null)
        {
            //Setting the default transforms
            initialPosition = new Vector3(cntrl_Data_SO.positionX, cntrl_Data_SO.positionY, cntrl_Data_SO.positionZ);
            initialRotation = new Vector3(cntrl_Data_SO.rotationX, cntrl_Data_SO.rotationY, cntrl_Data_SO.rotationZ);
            controlObject.transform.position = initialPosition;
            controlObject.transform.eulerAngles = initialRotation;
        }
        else
        {
            initialPosition = controlObject.transform.position;
            initialRotation = controlObject.transform.eulerAngles;
        }
    }
    void Update()
    {
        if (useControllerInput && isActive) 
        {
            if (leftObject) 
            {
                rightObject = false;
                moveInput = playerControls.DanceControls.MoveL.ReadValue<Vector2>();
            }

            if (rightObject) 
            {
                leftObject = false;
                moveInput = playerControls.DanceControls.MoveR.ReadValue<Vector2>();
            }

            float rangedX = Mathf.Lerp(-x_Range, x_Range, (moveInput.x + 1f) / 2f);
            float rangedY = Mathf.Lerp(-y_Range, y_Range, (moveInput.y + 1f) / 2f);

            controlObject.transform.position = new Vector3(-rangedX + initialPosition.x, rangedY + initialPosition.y, 0f);

            Vector3 currentPosition = controlObject.transform.position;
            float newX = Mathf.Clamp(currentPosition.x, initialPosition.x - x_Range, initialPosition.x + x_Range);
            float newY = Mathf.Clamp(currentPosition.y, initialPosition.y - y_Range, initialPosition.y + y_Range);
            controlObject.transform.position = new Vector3(newX, newY, currentPosition.z);
        }
    }

    void TransformUpdates()
    {
        if (cntrl_Data_SO != null)
        {
            // Deactivate controller input when updating from Scriptable Object
            useControllerInput = false;

            // We re-tweak the default transforms 
            initialPosition = new Vector3(cntrl_Data_SO.positionX, cntrl_Data_SO.positionY, cntrl_Data_SO.positionZ);
            initialRotation = new Vector3(cntrl_Data_SO.rotationX, cntrl_Data_SO.rotationY, cntrl_Data_SO.rotationZ);

            controlObject.transform.position = initialPosition;
            controlObject.transform.eulerAngles = initialRotation;

            // Re-activate controller input after updating from Scriptable Object
            useControllerInput = true;
        }
    }
}
