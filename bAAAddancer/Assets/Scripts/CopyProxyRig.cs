using UnityEngine;

public class CopyProxyRig : MonoBehaviour
{
    public GameObject[] controlObjects;
    public GameObject[] proxyObjects;

    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;
    private Vector3[] positionOffsets;
    private Quaternion[] rotationOffsets;

    //set up varibles to use as position offset and rotation offset

    private void Start()
    {
        initialPositions = new Vector3[proxyObjects.Length];
        initialRotations = new Quaternion[proxyObjects.Length];
        positionOffsets = new Vector3[proxyObjects.Length];
        rotationOffsets = new Quaternion[proxyObjects.Length];

        for (int i = 0; i < controlObjects.Length; i++)
        {
            initialPositions[i] = proxyObjects[i].transform.position;
            initialRotations[i] = proxyObjects[i].transform.rotation;

            // calculate the offset position and rotation values
            positionOffsets[i] = controlObjects[i].transform.position - initialPositions[i];
            rotationOffsets[i] = Quaternion.Inverse(initialRotations[i]) * controlObjects[i].transform.rotation;
        }
    }
    void LateUpdate() // using late update else the "copy" doesn't get all the constraint (rotation) updates
    {
        
        for (int i = 0; i < controlObjects.Length; i++) 
        {
            controlObjects[i].transform.position = proxyObjects[i].transform.position + positionOffsets[i];
            controlObjects[i].transform.rotation = proxyObjects[i].transform.rotation * rotationOffsets[i];
        }
    }
}
