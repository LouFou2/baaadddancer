using Unity.VisualScripting;
using UnityEngine;

public class Zapper : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private GameObject gameSurfacePlane; // Reference to the plane object
    private Vector3 intitialPosition;
    [SerializeField] private GameObject zapEffectPrefab;
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
    }

    void Update()
    {
        MeshCollider planeCollider = gameSurfacePlane.GetComponent<MeshCollider>();
        
        Vector2 moveInput = playerControls.DanceControls.MoveL.ReadValue<Vector2>();

        //have to move it within bounds of plane surface (where bugs spawned)
        float x_RangeMin = -(intitialPosition.x - planeCollider.bounds.min.x);
        float x_RangeMax = planeCollider.bounds.max.x - intitialPosition.x;
        float y_RangeMin = -(intitialPosition.y - planeCollider.bounds.min.y);
        float y_RangeMax = planeCollider.bounds.max.y - intitialPosition.y;

        float rangedX = (moveInput.x <= 0) ? Mathf.Lerp(0, x_RangeMax, -moveInput.x) : Mathf.Lerp(0, x_RangeMin, moveInput.x);
        float rangedY = (moveInput.y <= 0) ? Mathf.Lerp(0, y_RangeMin, -moveInput.y) : Mathf.Lerp(0, y_RangeMax, moveInput.y);

        //move zapper object
        transform.position = new Vector3(intitialPosition.x + rangedX, intitialPosition.y + rangedY, planeCollider.bounds.center.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bug"))
        {
            other.gameObject.SetActive(false);
            Instantiate(zapEffectPrefab, transform.position, Quaternion.identity);
            bugsZapped++;
            
        }
    }
    public int GetBugZappedAmount() 
    {
        return bugsZapped;
    }
}
