using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LookManager : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private GameObject[] lookTargets; // set in inspector. these are the look aim targets (rig constraints) of each character
    [SerializeField] private GameObject[] lookTargetPositioners; //set in inspector. these are actual positions of characters in scene.

    [SerializeField] private GameObject randomLookPosition;
    private float gazePositionDuration;
    private bool gazePositionChange;
    void Start()
    {
        gazePositionChange = false;

        gazePositionDuration = Random.Range(1f, 3f);

        float xPosition = Random.Range(-0.5f, 0.5f);
        float yPosition = Random.Range(1.2f, 1.7f);
        float zPosition = Random.Range(0.5f, 2.5f);

        randomLookPosition.transform.position = new Vector3(xPosition, yPosition, zPosition);

    }

    void Update()
    {
        if (!gazePositionChange) 
        {
            StartCoroutine(ChangeGazePosition());
        }


        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            for(int i = 0; i < lookTargets.Length; i++) 
            {
                if (i != 0)
                    lookTargets[i].transform.position = lookTargetPositioners[0].transform.position;
                else
                    lookTargets[i].transform.position = randomLookPosition.transform.position;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) 
        {
            for (int i = 0; i < lookTargets.Length; i++)
            {
                if (i != 1)
                    lookTargets[i].transform.position = lookTargetPositioners[1].transform.position;
                else
                    lookTargets[i].transform.position = randomLookPosition.transform.position;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            for (int i = 0; i < lookTargets.Length; i++)
            {
                if (i != 2)
                    lookTargets[i].transform.position = lookTargetPositioners[2].transform.position;
                else
                    lookTargets[i].transform.position = randomLookPosition.transform.position;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) 
        {
            for (int i = 0; i < lookTargets.Length; i++)
            {
                if (i != 3)
                    lookTargets[i].transform.position = lookTargetPositioners[3].transform.position;
                else
                    lookTargets[i].transform.position = randomLookPosition.transform.position;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) 
        {
            for (int i = 0; i < lookTargets.Length; i++)
            {
                if (i != 4)
                    lookTargets[i].transform.position = lookTargetPositioners[4].transform.position;
                else
                    lookTargets[i].transform.position = randomLookPosition.transform.position;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) 
        {
            for (int i = 0; i < lookTargets.Length; i++)
            {
                if (i != 5)
                    lookTargets[i].transform.position = lookTargetPositioners[5].transform.position;
                else
                    lookTargets[i].transform.position = randomLookPosition.transform.position;
            }
        }
    }
    private IEnumerator ChangeGazePosition() 
    {
        gazePositionChange = true;

        float xPosition = Random.Range(-0.5f, 0.5f);
        float yPosition = Random.Range(1.2f, 1.7f);
        float zPosition = Random.Range(0.5f, 2.5f);

        Vector3 newRandomLookPosition = new Vector3(xPosition, yPosition, zPosition);

        float changeGazeDuration = 0.2f;
        float timePassed = 0f;

        while (timePassed < changeGazeDuration) 
        {
            timePassed += Time.deltaTime;
            randomLookPosition.transform.DOMove(newRandomLookPosition, changeGazeDuration).SetEase(Ease.InOutBack);
        }
        timePassed = 0.2f;

        yield return new WaitForSeconds(gazePositionDuration);

        gazePositionDuration = Random.Range(1f, 3f);

        gazePositionChange = false;
    }
}
