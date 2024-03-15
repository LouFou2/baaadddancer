using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ControlObjectSwitcher;

public class ViewSwitcher : MonoBehaviour
{
    public enum ViewSwitch { front, top, left, right };
    public ViewSwitch CurrentView { get; set; } = ViewSwitch.front; // Property to access the current view

    private PlayerControls playerControls;

    //private Camera camera;
    [SerializeField] private Transform frontCameraTransform;
    [SerializeField] private Transform topCameraTransform;
    [SerializeField] private Transform leftCameraTransform;
    [SerializeField] private Transform rightCameraTransform;

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
        // Check for view switching controls
        if (playerControls == null)
        {
            Debug.LogError("Player controls are not assigned in ViewSwitcher.");
            return;
        }

        // Check for controls for each view and update the current view
        if (playerControls.DanceControls.RotateViewX_Front.triggered)
        {
            CurrentView = ViewSwitch.front;
            MoveCameraToView(frontCameraTransform);
            Debug.Log("frontview");
        }
        else if (playerControls.DanceControls.RotateViewX_Top.triggered)
        {
            CurrentView = ViewSwitch.top;
            MoveCameraToView(topCameraTransform);
            Debug.Log("topview");
        }
        else if (playerControls.DanceControls.RotateViewY_Left.triggered)
        {
            CurrentView = ViewSwitch.left;
            MoveCameraToView(leftCameraTransform);
            Debug.Log("leftview");
        }
        else if (playerControls.DanceControls.RotateViewY_Right.triggered)
        {
            CurrentView = ViewSwitch.right;
            MoveCameraToView(rightCameraTransform);
            Debug.Log("rightview");
        }
    }
    // Move the camera to the specified view
    private void MoveCameraToView(Transform targetTransform)
    {
        if (Camera.main != null && targetTransform != null)
        {
            Camera.main.transform.position = targetTransform.position;
            Camera.main.transform.rotation = targetTransform.rotation;
        }
        else
        {
            Debug.LogError("Camera or target transform is not assigned.");
        }
    }
}
