using UnityEngine;

public class HairShapeAnimator : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer; // assign in inspector

    [SerializeField] private GameObject hairAnchor;
    [SerializeField] private GameObject hairSpring;

    [SerializeField] private int hairU_Index;
    [SerializeField] private int hairD_Index;
    [SerializeField] private int hairL_Index;
    [SerializeField] private int hairR_Index;
    [SerializeField] private int hairF_Index;
    [SerializeField] private int hairB_Index;

    [SerializeField] private float shapeValueMultiplier;

    void Update()
    {
        // Measure direction and distance between hairAnchor and hairSpring
        Vector3 springVector = -(hairAnchor.transform.position - hairSpring.transform.position); // Negate the vector

        // The Y shapekeys: U / D
        if (springVector.y > 0)
        {
            // Add to the Up ShapeKey, multiplied by shapeValueMultiplier
            skinnedMeshRenderer.SetBlendShapeWeight(hairU_Index, springVector.y * shapeValueMultiplier);
        }
        else if (springVector.y < 0)
        {
            // Add absolute value to the Down ShapeKey, multiplied by shapeValueMultiplier
            skinnedMeshRenderer.SetBlendShapeWeight(hairD_Index, Mathf.Abs(springVector.y) * shapeValueMultiplier);
        }

        // The X shapekeys: L / R
        if (springVector.x > 0)
        {
            // Add to the Left ShapeKey, multiplied by shapeValueMultiplier
            skinnedMeshRenderer.SetBlendShapeWeight(hairL_Index, springVector.x * shapeValueMultiplier);
        }
        else if (springVector.x < 0)
        {
            // Add absolute value to the Right ShapeKey, multiplied by shapeValueMultiplier
            skinnedMeshRenderer.SetBlendShapeWeight(hairR_Index, Mathf.Abs(springVector.x) * shapeValueMultiplier);
        }

        // The Z shapekeys: F / B
        if (springVector.z > 0)
        {
            // Add to the Forward ShapeKey, multiplied by shapeValueMultiplier
            skinnedMeshRenderer.SetBlendShapeWeight(hairF_Index, springVector.z * shapeValueMultiplier);
        }
        else if (springVector.z < 0)
        {
            // Add absolute value to the Back ShapeKey, multiplied by shapeValueMultiplier
            skinnedMeshRenderer.SetBlendShapeWeight(hairB_Index, Mathf.Abs(springVector.z) * shapeValueMultiplier);
        }
    }
}
