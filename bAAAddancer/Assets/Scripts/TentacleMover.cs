using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleMover : MonoBehaviour
{
    [SerializeField] private DemonFaceScreen demonFaceScreenScript; // assign in inspector
    [SerializeField] private GameObject[] tentaclesL = new GameObject[5]; // assign in inspector
    [SerializeField] private GameObject[] tentaclesR = new GameObject[5]; // assign in inspector

    private Vector3[] tentaclesPosL = new Vector3[5];
    private Vector3[] tentaclesPosR = new Vector3[5];

    private Vector3 mouthPosOffset;

    void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            tentaclesPosL[i] = tentaclesL[i].transform.position;
            tentaclesPosR[i] = tentaclesR[i].transform.position;
        }
        Vector2 mouthPosOffset2D = demonFaceScreenScript.GetMouthCenter();
        mouthPosOffset = new Vector3(mouthPosOffset2D.x, mouthPosOffset2D.y, 0);
    }

    void Update()
    {
        Vector2 mouthPos = demonFaceScreenScript.GetMouthCenter();
        Vector3 mouthPos3D = new Vector3(-mouthPos.x, mouthPos.y, 0); //***NOTE we invert the x here

        Vector3[] updatedTentaclePosL = new Vector3[5];
        Vector3[] updatedTentaclePosR = new Vector3[5];

        for (int i = 0; i < 5; i++)
        {
            updatedTentaclePosL[i] = tentaclesPosL[i] + mouthPos3D - mouthPosOffset;
            updatedTentaclePosR[i] = tentaclesPosR[i] + mouthPos3D - mouthPosOffset;

            tentaclesL[i].transform.position = updatedTentaclePosL[i];
            tentaclesR[i].transform.position = updatedTentaclePosR[i];
        }
    }
}
