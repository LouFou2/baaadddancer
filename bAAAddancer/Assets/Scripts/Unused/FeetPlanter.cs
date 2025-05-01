using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetPlanter : MonoBehaviour
{
    public bool leftFootPlanted = true;
    public bool rightFootPlanted = true;

    [SerializeField] private ObjectControls leftFootObjCnrlScript;
    [SerializeField] private ObjectControls righttFootObjCnrlScript;




    /* 
    // if feet: check if they are "planted" (a.i. not able to shift, bearing weight)

        if ((isFoot && controlObject.transform.localPosition.y > (initialPosition.y + 0.002f)) || (isFoot && moveInput.magnitude > 0.002f))
        {
            if (leftObject)
                feetPlanter.leftFootPlanted = false;
            if (rightObject)
                feetPlanter.rightFootPlanted = false;
        }
        else if ((isFoot && controlObject.transform.localPosition.y <= (initialPosition.y + 0.002f)) || (isFoot && moveInput.magnitude <= 0.002f))
        {
            if (leftObject)
                feetPlanter.leftFootPlanted = true;
            if (rightObject)
                feetPlanter.rightFootPlanted = true;
        }

    // adjust feet positions inverse to root movement, if the foot is planted:
    Vector3 rootPos = rootTransforms.GetRootPosition();
    float rootX = rootPos.x;
    float rootZ = rootPos.z;
    Vector3 footOffset = controlObject.transform.localPosition + new Vector3(-rootX, 0, -rootZ);

    if (isFoot && leftObject && feetPlanter.leftFootPlanted)
    {
        //offset the foot position in inverse to the root movement
        if (isActive && isRecording)
        {
            finalUpdatePosition += new Vector3(-rootX, 0, -rootZ);
            controlObject.transform.localPosition = finalUpdatePosition;
        }
        else
        {
            controlObject.transform.localPosition = footOffset;
        }

    }
    if (isFoot && rightObject && feetPlanter.rightFootPlanted)
    {
        //offset the foot position in inverse to the root movement
        if (isActive && isRecording)
        {
            finalUpdatePosition += new Vector3(-rootX, 0, -rootZ);
            controlObject.transform.localPosition = finalUpdatePosition;
        }
        else
        {
            controlObject.transform.localPosition = footOffset;
        }
    }
    */
}
