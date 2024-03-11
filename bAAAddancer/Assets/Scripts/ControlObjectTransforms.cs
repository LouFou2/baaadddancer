using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlObjectTransforms : MonoBehaviour
{
    [SerializeField] 
    private CNTRL_Transforms_SO cntrl_Data_SO;
    private Transform cntrlObjectTransforms;
    void Start()
    {
        //Assign the scriptable object data to the transforms of this object
        cntrlObjectTransforms = transform;
        cntrlObjectTransforms.transform.localPosition = new Vector3(cntrl_Data_SO.positionX, cntrl_Data_SO.positionY, cntrl_Data_SO.positionZ);
        cntrlObjectTransforms.transform.localEulerAngles = new Vector3(cntrl_Data_SO.rotationX, cntrl_Data_SO.rotationY, cntrl_Data_SO.rotationZ);
    }
    // This section is temporary, along with TransformUpdates method
    // It allows me to check rig controls at runtime
    private void OnEnable()
    {
        cntrl_Data_SO.updated.AddListener(TransformUpdates);
    }
    private void OnDisable()
    {
        cntrl_Data_SO.updated.RemoveListener(TransformUpdates);
    }

    void FixedUpdate()
    {
        // add code for controlling character movement
    }

    void TransformUpdates() 
    {
        cntrlObjectTransforms = transform;
        cntrlObjectTransforms.transform.localPosition = new Vector3(cntrl_Data_SO.positionX, cntrl_Data_SO.positionY, cntrl_Data_SO.positionZ);
        cntrlObjectTransforms.transform.localEulerAngles = new Vector3(cntrl_Data_SO.rotationX, cntrl_Data_SO.rotationY, cntrl_Data_SO.rotationZ);
    }
}
