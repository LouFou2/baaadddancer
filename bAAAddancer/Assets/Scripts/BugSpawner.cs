using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BugSpawner : MonoBehaviour
{
    /*[SerializeField] private GameObject bugPrefab;
    [SerializeField] private int poolSize;
    [SerializeField] private GameObject gameSurfacePlane; // Reference to the plane object

    private Queue<GameObject> bugPool = new Queue<GameObject>(); // Object pool for bugs
    private List<GameObject> activeBugs = new List<GameObject>(); // List of active bugs

    private bool allBugsReleased = false;
    private bool debugHasEnded = false;
    [SerializeField] private float bugMoveDuration = 2f;
    private int bugMovements = 0;

    void Start()
    {
        // Pre-instantiate bugs and populate the object pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bug = Instantiate(bugPrefab, Vector3.zero, Quaternion.identity);
            bug.SetActive(false);
            bugPool.Enqueue(bug);
        }
    }
    private void Update()
    {
        if (allBugsReleased && DOTween.TotalPlayingTweens() == 0)
        {
            if (bugMovements < 3) MoveBugs();
            else EndDebug();
        }
    }

    public void SpawnAllBugsInPool()
    {
        MeshCollider planeCollider = gameSurfacePlane.GetComponent<MeshCollider>();

        for (int i = 0; i < poolSize; i++) 
        {
            // Generate a random point within the bounds of the plane's collider
            Vector3 randomPointOnPlane = GetRandomPointOnPlane(planeCollider.bounds);

            if (bugPool.Count > 0)
            {
                GameObject bugToSpawn = bugPool.Dequeue();
                bugToSpawn.transform.position = randomPointOnPlane;
                bugToSpawn.SetActive(true);
                activeBugs.Add(bugToSpawn); // Add the bug to the list of active bugs
            }
        }
        // once all bugs are spawned:
        allBugsReleased = true;
        MoveBugs();
    }
    private Vector3 GetRandomPointOnPlane(Bounds planeBounds)
    {
        return new Vector3
        (
            Random.Range(planeBounds.min.x, planeBounds.max.x),
            Random.Range(planeBounds.min.y, planeBounds.max.y),
            planeBounds.center.z
        );
    }
    private void MoveBugs()
    {
        bugMovements++;
        MeshCollider planeCollider = gameSurfacePlane.GetComponent<MeshCollider>();

        foreach (GameObject bug in activeBugs)
        {
            if (bug.activeSelf)
            {
                Vector3 moveTarget = GetRandomPointOnPlane(planeCollider.bounds);
                bug.transform.DOMove(moveTarget, bugMoveDuration).SetEase(Ease.InOutBounce);
            }
        }
    }
    private void EndDebug() 
    {
        foreach (GameObject bug in activeBugs) 
        {
            bug.SetActive(false);
        }
        debugHasEnded = true;
    }
    public int GetBugCount() 
    {
        return poolSize;
    }
    public bool DebugHasEnded() 
    {
        return debugHasEnded;
    }*/
    [SerializeField] private GameObject bugPrefab;
    [SerializeField] private GameObject gameSurfacePlane; // Reference to the plane object
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 2f;
    [SerializeField] private float bugMoveSpeed = 2f;
    [SerializeField] private float rotationAngleRange = 15f;

    private GameObject[] bugPool;
    private bool allBugsReleased = false;
    private bool debugHasEnded = false;

    private void Start()
    {
        InitializeBugPool();
        StartCoroutine(SpawnBugsSequentially());
    }

    private void InitializeBugPool()
    {
        bugPool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            bugPool[i] = Instantiate(bugPrefab, Vector3.zero, Quaternion.identity);
            bugPool[i].SetActive(false);
        }
    }

    public IEnumerator SpawnBugsSequentially()
    {
        MeshCollider planeCollider = gameSurfacePlane.GetComponent<MeshCollider>();
        while (!allBugsReleased)
        {
            // Get a bug from the pool
            GameObject bug = GetPooledBug();

            // Generate a random point along the top edge of the plane
            Vector3 spawnPosition = new Vector3(
                Random.Range(planeCollider.bounds.min.x, planeCollider.bounds.max.x),
                planeCollider.bounds.max.y,
                Random.Range(planeCollider.bounds.min.z, planeCollider.bounds.max.z)
            );

            bug.transform.position = spawnPosition;
            bug.SetActive(true);

            MoveBug(bug);

            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
        }
    }

    private GameObject GetPooledBug()
    {
        foreach (GameObject bug in bugPool)
        {
            if (!bug.activeSelf)
                return bug;
        }
        return null; // If all bugs are active, return null (not handling this for simplicity)
    }

    private void MoveBug(GameObject bug)
    {
        StartCoroutine(MoveBugCoroutine(bug));
    }

    private IEnumerator MoveBugCoroutine(GameObject bug)
    {
        while (bug != null && bug.activeSelf)
        {
            // Rotate the bug around the z-axis
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(-rotationAngleRange, rotationAngleRange));
            bug.transform.rotation *= randomRotation;

            // Move the bug downward along the y-axis
            bug.transform.Translate(Vector3.down * bugMoveSpeed * Time.deltaTime);

            // Check if the bug has reached the bottom of the screen
            if (bug.transform.position.y < 0)
            {
                // Handle what happens when the bug reaches the bottom (e.g., return to pool)
                bug.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }
    public int GetBugCount()
    {
        return poolSize;
    }

    public bool DebugHasEnded()
    {
        return debugHasEnded;
    }
}
