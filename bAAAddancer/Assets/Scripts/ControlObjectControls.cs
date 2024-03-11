using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlObjectControls : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private GameObject[] controlObjects;
    [SerializeField] private bool isActive = false;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float x_Range;
    [SerializeField] private float y_Range;

    private Vector3 initialPosition;


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
        initialPosition = transform.position;
    }
    void Update()
    {
        Vector2 moveInput = playerControls.DanceControls.Move.ReadValue<Vector2>();
        Debug.Log("input x: " + moveInput.x);
        Debug.Log("input y: " + moveInput.y);

        float rangedX = Mathf.Lerp(-x_Range, x_Range, (moveInput.x + 1f) / 2f);
        float rangedY = Mathf.Lerp(-y_Range, y_Range, (moveInput.y + 1f) / 2f);

        //controlObjects[0].transform.position += new Vector3(-rangedX, rangedY, 0f) * Time.deltaTime * moveSpeed;
        controlObjects[0].transform.position = new Vector3(-rangedX + initialPosition.x, rangedY + initialPosition.y, 0f);

        Vector3 currentPosition = controlObjects[0].transform.position;
        float newX = Mathf.Clamp(currentPosition.x, initialPosition.x - x_Range, initialPosition.x + x_Range);
        float newY = Mathf.Clamp(currentPosition.y, initialPosition.y - y_Range, initialPosition.y + y_Range);
        controlObjects[0].transform.position = new Vector3(newX, newY, currentPosition.z);

    }
}
