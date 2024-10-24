using UnityEngine;

public class CopyProxyRig : MonoBehaviour
{
    public GameObject rootControlObject;
    public GameObject rootProxyObject;

    public GameObject[] controlObjects;
    public GameObject[] proxyObjects;

    private Vector3 rootInitialPosition;
    private Quaternion rootInitialRotation;
    private Vector3 rootPositionOffset;
    private Quaternion rootRotationOffset;

    private Vector3[] initialPositions;
    //private Quaternion[] initialRotations; //***DONT THINK THIS SCRIPT NEEDS TO HANDLE GIZMO ROTATIONS
    private Vector3[] positionOffsets;
    //private Quaternion[] rotationOffsets;

    //set up varibles to use as position offset and rotation offset

    private void Start()
    {
        rootInitialPosition = rootProxyObject.transform.position;
        rootInitialRotation = rootProxyObject.transform.rotation;

        rootPositionOffset = rootControlObject.transform.position - rootInitialPosition;
        rootRotationOffset = Quaternion.Inverse(rootInitialRotation) * rootControlObject.transform.rotation;

        initialPositions = new Vector3[proxyObjects.Length];
        //initialRotations = new Quaternion[proxyObjects.Length];
        positionOffsets = new Vector3[proxyObjects.Length];
        //rotationOffsets = new Quaternion[proxyObjects.Length];

        for (int i = 0; i < controlObjects.Length; i++)
        {
            initialPositions[i] = proxyObjects[i].transform.localPosition;
            //initialRotations[i] = proxyObjects[i].transform.localRotation;

            // calculate the offset position and rotation values
            positionOffsets[i] = controlObjects[i].transform.localPosition - initialPositions[i];
            //rotationOffsets[i] = Quaternion.Inverse(initialRotations[i]) * controlObjects[i].transform.rotation;
        }
    }
    void LateUpdate() // using late update else the "copy" doesn't get all the constraint (rotation) updates
    {
        rootControlObject.transform.position = rootProxyObject.transform.position + rootPositionOffset;
        rootControlObject.transform.rotation = rootProxyObject.transform.rotation * rootRotationOffset;

        for (int i = 0; i < controlObjects.Length; i++) 
        {
            controlObjects[i].transform.localPosition = proxyObjects[i].transform.localPosition + positionOffsets[i] + rootControlObject.transform.position; //do i really have to add this here?
            //controlObjects[i].transform.localRotation = proxyObjects[i].transform.localRotation * rotationOffsets[i] * rootControlObject.transform.rotation;
        }
    }
}
