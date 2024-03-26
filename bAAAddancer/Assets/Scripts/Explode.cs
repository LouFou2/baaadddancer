using UnityEngine;

public class Explode : MonoBehaviour
{
    private Material material;
    private float scaleFactor = 0f;
    private float alphaFactor = 0f;
    [SerializeField] private float explodeSpeed = 1f;
    [SerializeField] private float maxScale = 3f;
    private void Awake()
    {
        transform.localScale = Vector3.zero;
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;
    }
    void Update()
    {
        scaleFactor += Time.deltaTime * explodeSpeed;
        
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        alphaFactor = Mathf.InverseLerp(0, maxScale, scaleFactor);

        // Modify the _Alpha property of the material
        material.SetFloat("_Alpha", alphaFactor);

        if (scaleFactor >= maxScale) Destroy(gameObject);
    }
}
