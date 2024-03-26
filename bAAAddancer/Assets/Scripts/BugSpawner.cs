using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BugSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bugPrefab;
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
    }
}
