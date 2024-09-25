using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerShaderHandler : MonoBehaviour
{
    [SerializeField] private Material dancerAbstractMat;
    private SimpleCounter counter;

    private void Start()
    {
        counter = FindObjectOfType<SimpleCounter>();
        dancerAbstractMat = GetComponent<MeshRenderer>().material;
    }
    void Update()
    {
        float beat = Mathf.InverseLerp(0, 4, counter.beat);
        float bar = Mathf.InverseLerp(0, 4, counter.bar);
        dancerAbstractMat.SetFloat("_Beat", beat);
        dancerAbstractMat.SetFloat("_Bar", bar);
    }
}
