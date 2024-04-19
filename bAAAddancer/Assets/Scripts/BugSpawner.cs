using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BugSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bugPrefab;
    private Queue<GameObject> bugPool = new Queue<GameObject>(); // Object pool for bugs
    [SerializeField] private GameObject gameSurfacePlane; // Reference to the plane object
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 2f;
    [SerializeField] private float bugMoveSpeed = 2f;
    [SerializeField] private float bugRotationSpeed = 2f;
    [SerializeField] private float rotationAngleRange = 15f;
    
    private bool allBugsReleased = false;
    private bool debugHasEnded = false;

    private void Start()
    {
        gameSurfacePlane.SetActive(true);
        InitializeBugPool();
    }

    private void InitializeBugPool()
    {
        MeshCollider planeCollider = gameSurfacePlane.GetComponent<MeshCollider>();

        for (int i = 0; i < poolSize; i++)
        {
            // Generate a random point along the top edge of the plane
            Vector3 spawnPosition = new Vector3(
                Random.Range(planeCollider.bounds.min.x, planeCollider.bounds.max.x),
                planeCollider.bounds.max.y,
                Random.Range(planeCollider.bounds.min.z, planeCollider.bounds.max.z)
            );

            GameObject bug = Instantiate(bugPrefab, spawnPosition, Quaternion.identity);
            bug.SetActive(false);
            bugPool.Enqueue(bug);
        }
    }

    public IEnumerator SpawnBugsSequentially()
    {
        while (!allBugsReleased && bugPool.Count > 0)
        {
            // Dequeue a bug from the pool
            GameObject bug = bugPool.Dequeue();
            bug.SetActive(true);

            // Move the bug
            MoveBug(bug);

            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
        }
    }

    private void MoveBug(GameObject bug)
    {
        StartCoroutine(MoveBugCoroutine(bug));
    }

    private IEnumerator MoveBugCoroutine(GameObject bug)
    {
        MeshCollider planeCollider = gameSurfacePlane.GetComponent<MeshCollider>();
        float bottomY = planeCollider.bounds.min.y;
        float targetAngle = Random.Range(-rotationAngleRange, rotationAngleRange);

        while (bug != null && bug.activeSelf)
        {
            bool outBoundsX = ((bug != null && bug.transform.position.x < planeCollider.bounds.min.x)
            || (bug != null && bug.transform.position.x > planeCollider.bounds.max.x)) ? true : false;

            // Rotate the bug towards the target angle
            bug.transform.rotation = Quaternion.RotateTowards(bug.transform.rotation, Quaternion.Euler(0, 0, targetAngle), Time.deltaTime * bugRotationSpeed);

            // Check if the bug has reached the target angle
            if (Quaternion.Angle(bug.transform.rotation, Quaternion.Euler(0, 0, targetAngle)) < 0.01f)
            {
                // If the bug has reached the target angle, pick a new random angle
                targetAngle = Random.Range(-rotationAngleRange, rotationAngleRange);
            }
            
            // Move the bug downward along the y-axis
            bug.transform.Translate(Vector3.down * bugMoveSpeed * Time.deltaTime);
            
            // Check if the bug has reached the bottom of the screen
            if (bug.transform.position.y < bottomY)
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
