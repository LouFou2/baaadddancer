using UnityEngine;

public class RootTransforms : MonoBehaviour
{
    private Rigidbody rB;
    [SerializeField] private LayerMask groundLayer; // Set this in the Inspector
    private PlayerControls playerControls;
    private Vector3 rootPosition = Vector3.zero;
    private Vector3 rootOffset;
    [SerializeField] private bool grounded = true;
    [SerializeField] private float jumpHeight = 0.3f;
    private float jumpForce; // Adjust in Inspector
    [SerializeField] private float gravityScale = 5f;
    [SerializeField] private float fallingGravityScale = 10f;
    [SerializeField] private float groundCheckDistance = 0.1f; // Adjust distance for ground check

    private bool jumpCalled;

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
    private void Start()
    {
        jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics.gravity.y * gravityScale));
        rootOffset = rootPosition + transform.position;
    }

    void Update()
    {
        // Grounded check using Raycast
        grounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        //check input:
        if (playerControls.GenericInput.AButton.IsPressed() && grounded)
        {
            jumpCalled = true;
        }
        
        rootPosition = transform.position - rootOffset + new Vector3(0f, 0.01f, 0f); // the 0.01 offset if because collider has a small buffer *see physics settings 
        Debug.Log(rootPosition);
    }
    private void FixedUpdate()
    {
        //=== Scaled gravity
        //-- if its going up:
        if (rB.velocity.y >= 0)
        {
            rB.AddForce(Physics.gravity * (gravityScale - 1) * rB.mass);
        }
        //going down, we make it fall faster
        else if (rB.velocity.y < 0)
        {
            rB.AddForce(Physics.gravity * (fallingGravityScale - 1) * rB.mass);
        }
        
        // Jumping logic
        if (jumpCalled)
        {
            Jump();
        }
    }
    private void Jump()
    {
        jumpCalled = false;
        rB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply impulse for an instant jump
    }

    public Vector3 GetRootPosition()
    {
        return rootPosition;
    }
}
