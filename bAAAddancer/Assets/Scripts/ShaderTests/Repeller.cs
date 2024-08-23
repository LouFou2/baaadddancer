using UnityEngine;

public class Repeller : MonoBehaviour
{
    public Material material; // Assign your material in the inspector

    // EffectPoint (a point that affects the mesh)
    [SerializeField] bool useRepell;
    [SerializeField] GameObject repellPointObject;
    private Vector3 repellPoint;
    private enum RepellOption { RepellDefault, RepellViewX, RepellViewY };
    [SerializeField] private RepellOption repellOption;
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

        switch (repellOption) 
        {
            case (RepellOption.RepellDefault):
                material.SetInt("_RepellViewX", 0);
                material.SetInt("_RepellViewY", 0);
                break;
            case (RepellOption.RepellViewX):
                material.SetInt("_RepellViewX", 1);
                material.SetInt("_RepellViewY", 0);
                break;
            case (RepellOption.RepellViewY):
                material.SetInt("_RepellViewY", 1);
                material.SetInt("_RepellViewX", 0);
                break;
            default:
                repellOption = RepellOption.RepellDefault;
                break;
        }

    }
}
