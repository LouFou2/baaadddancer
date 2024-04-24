using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zapper : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private GameObject gameSurfacePlane; // Reference to the plane object
    [SerializeField] private GameObject zapEffectPrefab;
    [SerializeField] private GameObject zapMesh1;
    [SerializeField] private GameObject zapMesh2;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private int bugsZapped = 0;

    private HashSet<GameObject> zappedBugs = new HashSet<GameObject>(); // Set to track zapped bugs

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
        zapMesh1.SetActive(true);
        zapMesh2.SetActive(false);
    }

    void Update()
    {
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
        /*if (other.CompareTag("Bug"))
        {
            //wait for player trigger pewpew
            if (playerControls.GenericInput.RTrigger.IsPressed() || playerControls.GenericInput.AButton.IsPressed())
            {
                Instantiate(zapEffectPrefab, transform.position, Quaternion.identity);
                DestroyBug(other.gameObject);
            }
        }*/
        bool zapButtonPressed = (playerControls.GenericInput.RTrigger.IsPressed() || playerControls.GenericInput.AButton.IsPressed()) ? true : false;

        if (other.CompareTag("Bug") && !zappedBugs.Contains(other.gameObject) && zapButtonPressed)
        {
            GameObject rootParent = GetRootParent(other.gameObject);
            zappedBugs.Add(other.gameObject); // Add the bug to the set of zapped bugs
            Instantiate(zapEffectPrefab, transform.position, Quaternion.identity);
            DestroyBug(rootParent);
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
    private void DestroyBug(GameObject rootParent) 
    {
        /*// Get the topmost parent GameObject
        GameObject rootParent = colliderObject;
        while (rootParent.transform.parent != null)
        {
            rootParent = rootParent.transform.parent.gameObject;
        }

        // Destroy the parent GameObject
        Destroy(rootParent);

        bugsZapped++;

        zapMesh1.SetActive(true); // just ensuring the colours swap back
        zapMesh2.SetActive(false);*/
        Destroy(rootParent);
        bugsZapped++;
        zapMesh1.SetActive(true); // Ensuring the colors swap back
        zapMesh2.SetActive(false);
    }
    private GameObject GetRootParent(GameObject obj)
    {
        GameObject rootParent = obj;
        while (rootParent.transform.parent != null)
        {
            rootParent = rootParent.transform.parent.gameObject;
        }
        return rootParent;
    }
    public int GetBugZappedAmount() 
    {
        return bugsZapped;
    }
}
