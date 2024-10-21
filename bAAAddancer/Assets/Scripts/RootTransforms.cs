using UnityEngine;

public class RootTransforms : MonoBehaviour
{
    private Rigidbody rB;
    [SerializeField] private LayerMask groundLayer; // Set this in the Inspector
    private PlayerControls playerControls;
    private Vector3 rootPosition = Vector3.zero;
    [SerializeField] private bool grounded = true;
    [SerializeField] private float jumpForce = 10f; // Adjust in Inspector
    [SerializeField] private float groundCheckDistance = 0.1f; // Adjust distance for ground check

    private void Awake()
    {
        playerControls = new PlayerControls();
        rB = GetComponent<Rigidbody>();
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
        // Grounded check using Raycast
        grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // Jumping logic
        if (playerControls.GenericInput.AButton.IsPressed() && grounded)
        {
            Jump();
        }

        rootPosition = transform.position;
    }

    private void Jump()
    {
        rB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply impulse for an instant jump
    }

    public Vector3 GetRootPosition()
    {
        return rootPosition;
    }
}
