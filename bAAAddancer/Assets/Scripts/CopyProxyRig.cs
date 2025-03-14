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
    private Vector3[] positionOffsets;

    private Vector3[] orbitPositionOfsets; // it should be moved like children of a rotating parent object (a.i. the root... but its not actually a parent)


    private void Start()
    {
        rootInitialPosition = rootProxyObject.transform.position;
        rootInitialRotation = rootProxyObject.transform.rotation;

        rootPositionOffset = rootControlObject.transform.position - rootInitialPosition;
        rootRotationOffset = Quaternion.Inverse(rootInitialRotation) * rootControlObject.transform.rotation;

        initialPositions = new Vector3[proxyObjects.Length];
        positionOffsets = new Vector3[proxyObjects.Length];

        orbitPositionOfsets = new Vector3[proxyObjects.Length];

        for (int i = 0; i < controlObjects.Length; i++)
        {
            initialPositions[i] = proxyObjects[i].transform.localPosition;

            // calculate the offset position and rotation values
            positionOffsets[i] = controlObjects[i].transform.localPosition - initialPositions[i];

            orbitPositionOfsets[i] = controlObjects[i].transform.localPosition - rootControlObject.transform.position;
        }
    }
    void LateUpdate() // using late update else the "copy" doesn't get all the constraint (rotation) updates
    {
        rootControlObject.transform.position = rootProxyObject.transform.position + rootPositionOffset;
        rootControlObject.transform.rotation = rootProxyObject.transform.rotation * rootRotationOffset;

        for (int i = 0; i < controlObjects.Length; i++) 
        {
            Vector3 x_z_OrbitOffset = new Vector3(orbitPositionOfsets[i].x, 0, orbitPositionOfsets[i].z);
            Vector3 rotatedOffset = rootControlObject.transform.rotation * x_z_OrbitOffset;
            controlObjects[i].transform.localPosition = proxyObjects[i].transform.localPosition 
                + positionOffsets[i] 
                + rootControlObject.transform.position
                + rotatedOffset; 
        }
    }
}
