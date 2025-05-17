using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFaceScreen : MonoBehaviour
{
    [SerializeField] private ScreenManipulator screenManipulator; // assign in inspector
    [SerializeField] private Material matDemonFaceScreen; // assign in inspector
    [SerializeField] private Material screenRenderTextureMat; // assign in inspector

    [SerializeField] private float offsetY = -0.5f;
    [SerializeField] private float scaleMovementY = 0.5f;

    void Update()
    {
        Vector2 thumbL = screenManipulator.GetThumbVectorL();
        Vector2 thumbR = screenManipulator.GetThumbVectorR();
        float thumbMagL = screenManipulator.GetThumbMagL();
        float thumbMagR = screenManipulator.GetThumbMagR();

        //placing the center of the mouth between the two thumb inputs:
        float thumbCenterX = (thumbR.x - thumbL.x) * 0.5f;
        float thumbCenterY = (thumbR.y - thumbL.y) * 0.5f;

        float positionX = thumbL.x + thumbCenterX;
        float positionY = thumbL.y + offsetY + (thumbCenterY * scaleMovementY);

        Vector2 calculatedMouthPos = new Vector2(positionX, positionY);

        matDemonFaceScreen.SetVector("_MouthCenter", calculatedMouthPos);
        screenRenderTextureMat.SetVector("_MouthCenter", calculatedMouthPos);

        matDemonFaceScreen.SetVector("_ThumbL", thumbL);
        matDemonFaceScreen.SetVector("_ThumbR", thumbR);
        matDemonFaceScreen.SetFloat("_ThumbMagL", thumbMagL);
        matDemonFaceScreen.SetFloat("_ThumbMagR", thumbMagR);
    }
}
