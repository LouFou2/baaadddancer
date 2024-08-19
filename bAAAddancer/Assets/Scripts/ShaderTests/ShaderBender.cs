using UnityEngine;

public class ShaderBender : MonoBehaviour
{
    public Material material; // Assign your material in the inspector

    private float timer = 0f;
    [SerializeField] private float flickerSpeed = 1.0f; // Controls the speed of the flicker effect
    [SerializeField] private float flickerIntensity = 0.5f; // Controls the intensity of the flicker effect

    void Update()
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
