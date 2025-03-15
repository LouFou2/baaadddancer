using UnityEngine;

public class CopyProxyRig : MonoBehaviour
{
    public GameObject rootControlObject;
    public GameObject rootProxyObject;

    public GameObject[] controlObjects;
    public GameObject[] proxyObjects;

    private Vector3[] initialPositions;
    private Vector3[] positionOffsets;

    private float[] orbitPositionOffsets; // using a float because this is just a distance
    private Vector3[] orbitObjectDirections; // this is a normalised vector. We will multiply the "orbit object position" * "direction"


    private void Start()
    {
        // Just a note about "Root Offsets" : 
        // the root "proxy" and every character's "root control" is always at 0,0,0 (at least for the scene this script is used in)
        // so offests are also 0,0,0.
        // meaning we can just use the proxy object's position as is, no "offset" calculation necesarry
        
        initialPositions = new Vector3[proxyObjects.Length];
        positionOffsets = new Vector3[proxyObjects.Length];

        orbitPositionOffsets = new float[proxyObjects.Length];
        orbitObjectDirections = new Vector3[proxyObjects.Length];

        for (int i = 0; i < controlObjects.Length; i++)
        {
            initialPositions[i] = proxyObjects[i].transform.position;
            positionOffsets[i] = controlObjects[i].transform.position - initialPositions[i];

            // *** !! I changed the position calculations for proxy objects as well as control objects to use world space and not localPosition.
            // This might mess things up, so if there are problems, change it back !! (also below) ***
        }
    }
    void LateUpdate() // using late update else the "copy" doesn't get all the constraint (rotation) updates
    {
        rootControlObject.transform.position = rootProxyObject.transform.position;
        rootControlObject.transform.rotation = rootProxyObject.transform.rotation;

        for (int i = 0; i < controlObjects.Length; i++) 
        {
            controlObjects[i].transform.position =
                proxyObjects[i].transform.position
                + positionOffsets[i]
                + rootControlObject.transform.position;
        }
    }
}
