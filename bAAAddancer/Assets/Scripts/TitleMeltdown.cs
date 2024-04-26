using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMeltdown : MonoBehaviour
{
    [SerializeField] Material titleMaterial; 
    [SerializeField] float meltSpeed = 10f;
    private float increasedNoise = 0f;
    public void HandleTitleMelt() 
    {
        StartCoroutine(MeltTitle());
    }
    private IEnumerator MeltTitle() 
    {
        float meltInterval = 1 / meltSpeed;

        while (increasedNoise < 1) {
            increasedNoise += 0.05f;
            titleMaterial.SetFloat("_NoiseX", increasedNoise);
            yield return new WaitForSeconds(meltInterval);
        }
    }
}
