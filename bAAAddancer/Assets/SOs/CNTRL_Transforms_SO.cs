using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu (fileName = "CntrlObjectTransformData", menuName = "CntrlObjectTransformDataSO")]
public class CNTRL_Transforms_SO : ScriptableObject
{
    //store rig control GameObject Transforms
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationX;
    public float rotationY;
    public float rotationZ;

    [HideInInspector] public UnityEvent updated;


    // I am using this S.O to tweak Transforms during runtime in inspector
    // The OnEnable and OnValidate methods below can be removed when game is finished
    private void OnEnable()
    {
        // called when the instance is setup

        if (updated == null)
            updated = new UnityEvent();
    }

    private void OnValidate()
    {
        // called when any value is changed
        // in the inspector

        updated.Invoke();
    }
}
