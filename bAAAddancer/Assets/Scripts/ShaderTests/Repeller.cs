using UnityEngine;

public class Repeller : MonoBehaviour
{
    public Material material; // Assign your material in the inspector

    // EffectPoint (a point that affects the mesh)
    [SerializeField] bool useRepell;
    [SerializeField] GameObject repellPointObject;
    private Vector3 repellPoint;
    [SerializeField] [Range(0,1)] float repellFalloff;
    [SerializeField] [Range(0, 1)] float repellDisplacement;

    void Update()
    {
        if (useRepell)
            RepellEffects();
    }
    void RepellEffects()
    {
        repellPoint = repellPointObject.transform.position;
        material.SetVector("_RepellPoint", repellPoint);
        material.SetFloat("_RepellPointFalloff", repellFalloff);
        material.SetFloat("_RepellPointDisplacement", repellDisplacement);
    }
}
