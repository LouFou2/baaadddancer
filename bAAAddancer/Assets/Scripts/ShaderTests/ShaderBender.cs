using UnityEngine;

public class ShaderBender : MonoBehaviour
{
    public Material material; // Assign your material in the inspector

    private float timer = 0f;

    // Shade smooth or flat
    [SerializeField] private bool shadeSmooth = true;

    // Flicker effect
    [SerializeField] private bool flickerShading = false;
    [SerializeField] private float flickerSpeed = 1.0f; // Controls the speed of the flicker effect
    [SerializeField] private float flickerIntensity = 0.5f; // Controls the intensity of the flicker effect

    // Displacement
    [SerializeField] [Range(0, 1)] private float displacementAmount = 0f;

    void Update()
    {
        // == Shade Smooth or Flat ==
        if(shadeSmooth)
            material.SetInt("_IsShadeFlat", 0);
        else
            material.SetInt("_IsShadeFlat", 1);

        // == Flicker Effect ==
        if (flickerShading)
            FlickerShadeSmoothFlat();

        // == Displacement ==
        material.SetFloat("_DisplacementDistance", displacementAmount);
    }

    void FlickerShadeSmoothFlat() 
    {
        timer += Time.deltaTime * flickerSpeed;

        // Generate a noise value based on the timer
        float noiseValue = Mathf.PerlinNoise(timer, 0.0f);

        // Map the noise value to a 0 or 1 value for shading mode
        float shadingMode = (noiseValue < flickerIntensity) ? 0.0f : 1.0f;

        // Set the shading mode in the material
        material.SetFloat("_ShadingMode", shadingMode);
    }

}
