using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleMover : MonoBehaviour
{
    [SerializeField] private DemonFaceScreen demonFaceScreenScript; // assign in inspector
    [SerializeField] private Material screenRenderTextureMat;  // assign in inspector

    [SerializeField] private GameObject[] tentaclesL = new GameObject[5]; // assign in inspector
    [SerializeField] private GameObject[] tentaclesR = new GameObject[5]; // assign in inspector
    //layer2
    [SerializeField] private GameObject[] tentacles2L = new GameObject[5]; // assign in inspector
    [SerializeField] private GameObject[] tentacles2R = new GameObject[5]; // assign in inspector
    //layer3
    [SerializeField] private GameObject[] tentacles3L = new GameObject[5]; // assign in inspector
    [SerializeField] private GameObject[] tentacles3R = new GameObject[5]; // assign in inspector

    private Vector3[] tentaclesPosL = new Vector3[5];
    private Vector3[] tentaclesPosR = new Vector3[5];

    //second layer
    private Vector3[] tentacles2PosL = new Vector3[5];
    private Vector3[] tentacles2PosR = new Vector3[5];

    //second layer
    private Vector3[] tentacles3PosL = new Vector3[5];
    private Vector3[] tentacles3PosR = new Vector3[5];

    [SerializeField] private float scaleMovementX = 1.5f;
    [SerializeField] private float scaleMovementY = 0.5f;

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
        Vector3 mouthPos3D = new Vector3(-mouthPos.x * scaleMovementX, mouthPos.y * scaleMovementY, 0); //***NOTE we invert the x here

        Vector3[] updatedTentaclePosL = new Vector3[5];
        Vector3[] updatedTentaclePosR = new Vector3[5];

        for (int i = 0; i < 5; i++)
        {
            updatedTentaclePosL[i] = tentaclesPosL[i] + mouthPos3D - mouthPosOffset;
            updatedTentaclePosR[i] = tentaclesPosR[i] + mouthPos3D - mouthPosOffset;

            tentaclesL[i].transform.position = updatedTentaclePosL[i];
            tentaclesR[i].transform.position = updatedTentaclePosR[i];
        }

        SendTentaclePositionsToScreen();
    }
    void SendTentaclePositionsToScreen()
    {
        screenRenderTextureMat.SetVector("_TentaclePosL_0", new Vector2(tentaclesL[0].transform.position.x, tentaclesL[0].transform.position.y));
        screenRenderTextureMat.SetVector("_TentaclePosL_1", new Vector2(tentaclesL[1].transform.position.x, tentaclesL[1].transform.position.y));
        screenRenderTextureMat.SetVector("_TentaclePosL_2", new Vector2(tentaclesL[2].transform.position.x, tentaclesL[2].transform.position.y));
        screenRenderTextureMat.SetVector("_TentaclePosL_3", new Vector2(tentaclesL[3].transform.position.x, tentaclesL[3].transform.position.y));
        screenRenderTextureMat.SetVector("_TentaclePosL_4", new Vector2(tentaclesL[4].transform.position.x, tentaclesL[4].transform.position.y));

        screenRenderTextureMat.SetVector("_TentaclePosR_0", new Vector2(tentaclesR[0].transform.position.x, tentaclesR[0].transform.position.y));
        screenRenderTextureMat.SetVector("_TentaclePosR_1", new Vector2(tentaclesR[1].transform.position.x, tentaclesR[1].transform.position.y));
        screenRenderTextureMat.SetVector("_TentaclePosR_2", new Vector2(tentaclesR[2].transform.position.x, tentaclesR[2].transform.position.y));
        screenRenderTextureMat.SetVector("_TentaclePosR_3", new Vector2(tentaclesR[3].transform.position.x, tentaclesR[3].transform.position.y));
        screenRenderTextureMat.SetVector("_TentaclePosR_4", new Vector2(tentaclesR[4].transform.position.x, tentaclesR[4].transform.position.y));

        //Layer2
        screenRenderTextureMat.SetVector("_Tentacle2PosL_0", new Vector2(tentacles2L[0].transform.position.x, tentacles2L[0].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle2PosL_1", new Vector2(tentacles2L[1].transform.position.x, tentacles2L[1].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle2PosL_2", new Vector2(tentacles2L[2].transform.position.x, tentacles2L[2].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle2PosL_3", new Vector2(tentacles2L[3].transform.position.x, tentacles2L[3].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle2PosL_4", new Vector2(tentacles2L[4].transform.position.x, tentacles2L[4].transform.position.y));

        screenRenderTextureMat.SetVector("_Tentacle2PosR_0", new Vector2(tentacles2R[0].transform.position.x, tentacles2R[0].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle2PosR_1", new Vector2(tentacles2R[1].transform.position.x, tentacles2R[1].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle2PosR_2", new Vector2(tentacles2R[2].transform.position.x, tentacles2R[2].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle2PosR_3", new Vector2(tentacles2R[3].transform.position.x, tentacles2R[3].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle2PosR_4", new Vector2(tentacles2R[4].transform.position.x, tentacles2R[4].transform.position.y));

        //Layer3
        screenRenderTextureMat.SetVector("_Tentacle3PosL_0", new Vector2(tentacles3L[0].transform.position.x, tentacles3L[0].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle3PosL_1", new Vector2(tentacles3L[1].transform.position.x, tentacles3L[1].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle3PosL_2", new Vector2(tentacles3L[2].transform.position.x, tentacles3L[2].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle3PosL_3", new Vector2(tentacles3L[3].transform.position.x, tentacles3L[3].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle3PosL_4", new Vector2(tentacles3L[4].transform.position.x, tentacles3L[4].transform.position.y));

        screenRenderTextureMat.SetVector("_Tentacle3PosR_0", new Vector2(tentacles3R[0].transform.position.x, tentacles3R[0].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle3PosR_1", new Vector2(tentacles3R[1].transform.position.x, tentacles3R[1].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle3PosR_2", new Vector2(tentacles3R[2].transform.position.x, tentacles3R[2].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle3PosR_3", new Vector2(tentacles3R[3].transform.position.x, tentacles3R[3].transform.position.y));
        screenRenderTextureMat.SetVector("_Tentacle3PosR_4", new Vector2(tentacles3R[4].transform.position.x, tentacles3R[4].transform.position.y));
    }
}
