using Unity.VisualScripting;
using UnityEngine;

public class Zapper : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private GameObject gameSurfacePlane; // Reference to the plane object
    private Vector3 intitialPosition;
    [SerializeField] private GameObject zapEffectPrefab;
    [SerializeField] private GameObject zapMesh1;
    [SerializeField] private GameObject zapMesh2;
    [SerializeField] private float moveSpeed = 1f;
    private int bugsZapped = 0;

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
    void Start()
    {
        intitialPosition = transform.position;
        zapMesh1.SetActive(true);
        zapMesh2.SetActive(false);
    }

    void Update()
    {
        /*MeshCollider planeCollider = gameSurfacePlane.GetComponent<MeshCollider>();
        Vector2 moveInput = playerControls.DanceControls.MoveL.ReadValue<Vector2>();

        //have to move it within bounds of plane surface (where bugs spawned)
        float x_RangeMin = -(intitialPosition.x - planeCollider.bounds.min.x);
        float x_RangeMax = planeCollider.bounds.max.x - intitialPosition.x;
        float y_RangeMin = -(intitialPosition.y - planeCollider.bounds.min.y);
        float y_RangeMax = planeCollider.bounds.max.y - intitialPosition.y;

        float rangedX = (moveInput.x <= 0) ? Mathf.Lerp(0, x_RangeMax, -moveInput.x) : Mathf.Lerp(0, x_RangeMin, moveInput.x);
        float rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);

        //move zapper object
        transform.position = new Vector3(intitialPosition.x + rangedX, intitialPosition.y + rangedY, planeCollider.bounds.center.z);*/
        MeshCollider planeCollider = gameSurfacePlane.GetComponent<MeshCollider>();
        Vector2 moveInput = playerControls.DanceControls.MoveR.ReadValue<Vector2>();

        // Calculate the desired movement direction based on input
        Vector3 moveDirection = new Vector3(-moveInput.x, moveInput.y, 0f).normalized;

        // Calculate the movement amount based on the move speed and time
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

        // Calculate the new position while constraining it within the bounds of the plane surface
        Vector3 newPosition = transform.position + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, planeCollider.bounds.min.x, planeCollider.bounds.max.x);
        newPosition.y = Mathf.Clamp(newPosition.y, planeCollider.bounds.min.y, planeCollider.bounds.max.y);

        // Update the zapper object's position
        transform.position = newPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bug"))
        {
            zapMesh1.SetActive(false);
            zapMesh2.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bug"))
        {
            //wait for player trigger pewpew
            if (playerControls.GenericInput.RTrigger.IsPressed())
            {
                Instantiate(zapEffectPrefab, transform.position, Quaternion.identity);
                DestroyBug(other.gameObject);
                bugsZapped++;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bug"))
        {
            zapMesh1.SetActive(true);
            zapMesh2.SetActive(false);
        }
    }
    private void DestroyBug(GameObject colliderObject) 
    {
        // Get the topmost parent GameObject
        GameObject rootParent = colliderObject;
        while (rootParent.transform.parent != null)
        {
            rootParent = rootParent.transform.parent.gameObject;
        }

        // Destroy the parent GameObject
        Destroy(rootParent);

        zapMesh1.SetActive(true); // just ensuring the colours swap back
        zapMesh2.SetActive(false);
    }
    public int GetBugZappedAmount() 
    {
        return bugsZapped;
    }
}
