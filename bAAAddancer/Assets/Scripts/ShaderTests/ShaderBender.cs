using UnityEngine;

public class ShaderBender : MonoBehaviour
{
    public Material material; // Assign your material in the inspector

    private float timer = 0f;

    // Shade smooth or flat
    [SerializeField] private bool shadeSmooth = true;
    
    // Displacement
    [SerializeField] [Range(0, 1)] private float displacementAmount = 0f;

    // Normal Alignment
    [SerializeField] private bool alignNormalsToView;
    [SerializeField] [Range(0, 2)] private float normalAlignmentThreshold;
    [SerializeField] [Range(0, 1)] private float normalAlignFactor;

    void Update()
    {
        // == Shade Smooth or Flat ==
        if(shadeSmooth)
            material.SetInt("_IsShadeSmooth", 1);
        else
            material.SetInt("_IsShadeSmooth", 0);

        // == Displacement ==
        material.SetFloat("_DisplacementDistance", displacementAmount);

        
    }

    

}
