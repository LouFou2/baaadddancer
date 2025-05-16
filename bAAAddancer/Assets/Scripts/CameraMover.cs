using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour // Script for taking control of the Camera in the RaveScene2
{
    [SerializeField] private GameObject defaultCam; // this is the LongCam at the moment
    private PlayerControls playerControls;

    [SerializeField] private float camMoveSpeed = 10f;
    [SerializeField] private float camVerticalSpeed = 0.5f;
    [SerializeField] private float camRotateSpeed = 500f;  // adjust these as needed

    [SerializeField] private float xBoundary = 5f;
    [SerializeField] private float yMax = 5f;
    [SerializeField] private float yMin = 0f;
    [SerializeField] private float zMax = 0f;
    [SerializeField] private float zMin = 5f;

    private float yawAngle;
    private float pitchAngle;
    [SerializeField] private float yYawLimit = 90; //in degrees
    [SerializeField] private float yPitchLimit = 90;
    private float yawMin;
    private float yawMax;
    private float pitchMin;
    private float pitchMax;

    bool canControlCamera = false;

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

    public void StartCamControl()
    {
        canControlCamera = true;
    }
    public void Start()
    {
        Vector3 angles = defaultCam.transform.rotation.eulerAngles;
        yawAngle = angles.y;    // Rotation around Y axis
        pitchAngle = angles.x;  // Rotation around X axis
        yawMin = yawAngle - yYawLimit;
        yawMax = yawAngle + yYawLimit;
        pitchMin = pitchAngle - yPitchLimit;
        pitchMax = pitchAngle + yPitchLimit;
    }

    void Update()
    {
        if (!canControlCamera)
        {
            return;
        }
        else // here we go camera controls...
        {
            Vector2 horizontalInput = playerControls.GenericInput.LThumb.ReadValue<Vector2>();
            Vector2 cameraRotation = playerControls.GenericInput.RThumb.ReadValue<Vector2>();

            float verticalMove = 0f;

            // A Button = fly up
            if (playerControls.GenericInput.RTrigger.ReadValue<float>() > 0)
                verticalMove += 1f * camVerticalSpeed;

            // RS (Right Stick Click) or B Button = fly down
            if (playerControls.GenericInput.LTrigger.ReadValue<float>() > 0)
                verticalMove -= 1f * camVerticalSpeed;

            // --- Movement ---
            // Move the camera in the local forward and right directions
            Vector3 moveDirection = new Vector3(horizontalInput.x, verticalMove, -horizontalInput.y); // just negating x/y values here because it's audience left-right
            Vector3 newPosition = defaultCam.transform.position + (moveDirection * camMoveSpeed * Time.deltaTime);
            
            //Clamp movement to boundaries
            float xClamped = Mathf.Clamp(newPosition.x, -xBoundary, xBoundary);
            float yClamped = Mathf.Clamp(newPosition.y, yMin, yMax);
            float zClamped = Mathf.Clamp(newPosition.z, zMin, zMax);

            defaultCam.transform.position = new Vector3(xClamped, yClamped, zClamped);

            // --- Rotation ---
            float yaw = -cameraRotation.x * camRotateSpeed * Time.deltaTime;
            float pitch = -cameraRotation.y * camRotateSpeed * Time.deltaTime;

            // Update angles
            yawAngle += yaw;
            pitchAngle += pitch;

            // Clamp pitch and yaw
            yawAngle = Mathf.Clamp(yawAngle, yawMin, yawMax);
            pitchAngle = Mathf.Clamp(pitchAngle, pitchMin, pitchMax);

            // Apply rotation
            defaultCam.transform.rotation = Quaternion.Euler(pitchAngle, yawAngle, 0f);
        }
    }
}
