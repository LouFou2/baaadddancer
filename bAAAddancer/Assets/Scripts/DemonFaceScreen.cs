using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFaceScreen : MonoBehaviour
{
    [SerializeField] private ScreenManipulator screenManipulator; // assign in inspector
    [SerializeField] private Material matDemonFaceScreen; // assign in inspector

    void Update()
    {
        Vector2 thumbL = screenManipulator.GetThumbVectorL();
        Vector2 thumbR = screenManipulator.GetThumbVectorR();
        float thumbMagL = screenManipulator.GetThumbMagL();
        float thumbMagR = screenManipulator.GetThumbMagR();

        //a little fancy calculations for placing the center of the mouth:
        Vector2 vectorBetweenThumbs = thumbR - thumbL;
        vectorBetweenThumbs *= 0.5f;
        Vector2 centerBetweenThumbs = thumbL + vectorBetweenThumbs;
        matDemonFaceScreen.SetVector("_MouthCenter", centerBetweenThumbs);

        matDemonFaceScreen.SetVector("_ThumbL", thumbL);
        matDemonFaceScreen.SetVector("_ThumbR", thumbR);
        matDemonFaceScreen.SetFloat("_ThumbMagL", thumbMagL);
        matDemonFaceScreen.SetFloat("_ThumbMagR", thumbMagR);
    }
}
