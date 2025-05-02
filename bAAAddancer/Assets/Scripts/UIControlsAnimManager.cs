using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControlsAnimManager : MonoBehaviour // This is the Debug / NormAligner game
{
    [SerializeField] private AlignerController2 alignController; // assign in inspector
    [SerializeField] private GameObject thumbStickL;
    [SerializeField] private GameObject thumbStickR;
    [SerializeField] private GameObject triggerR_Lock;
    [SerializeField] private GameObject exitButton;

    float time = 0;

    RectTransform rectTransformThumbL;
    RectTransform rectTransformThumbR;
    RectTransform rectTransformTriggerR;
    RectTransform rectTransformExitButton;

    private float thumbPivotL_X;
    private float thumbPivotL_Y;

    private float thumbPivotR_X;
    private float thumbPivotR_Y;

    Vector3 triggerRScale;
    Vector3 exitButtonScale;

    private bool canLock = false;

    void Start()
    {
        exitButton.SetActive(false);
        triggerR_Lock.SetActive(false);

        rectTransformThumbL = thumbStickL.GetComponent<RectTransform>();
        rectTransformThumbR = thumbStickR.GetComponent<RectTransform>();
        rectTransformTriggerR = triggerR_Lock.GetComponent<RectTransform>();
        rectTransformExitButton = exitButton.GetComponent<RectTransform>();

        thumbPivotL_X = rectTransformThumbL.pivot.x;
        thumbPivotL_Y = rectTransformThumbL.pivot.y;

        thumbPivotR_X = rectTransformThumbR.pivot.x;
        thumbPivotR_Y = rectTransformThumbR.pivot.y;

        triggerRScale = rectTransformTriggerR.localScale;
        exitButtonScale = rectTransformExitButton.localScale;

        //StartCoroutine(UIAnimsCoroutine());
    }
    private void Update()
    {
        if (canLock && alignController.isLocked)
        {
            exitButton.SetActive(true);
        }
        else
        {
            exitButton.SetActive(false);
        }
    }

    public void StartUIAnims()
    {
        StartCoroutine(UIAnimsCoroutine());
    }
    private IEnumerator UIAnimsCoroutine()
    {
        float elapsed = 0f;

        while (elapsed <= 2f) // Run for 5 seconds
        {
            elapsed += Time.deltaTime;
            time += Time.deltaTime * 5f;

            float cos = Mathf.Cos(time);
            float sin = Mathf.Sin(time);

            float remapX_L = Mathf.InverseLerp(-1, 1, cos); // get a 0-1 range
            float remapY_L = Mathf.InverseLerp(-1, 1, sin); 

            float remapX_R = Mathf.InverseLerp(-1, 1, sin); //flipped sin and cos, reverses clockwise / anti - clockwise
            float remapY_R = Mathf.InverseLerp(-1, 1, cos);

            float scaledX_L = Mathf.Lerp(-0.3f, 0.3f, remapX_L);
            float scaledY_L = Mathf.Lerp(-0.3f, 0.3f, remapY_L);

            float scaledX_R = Mathf.Lerp(-0.3f, 0.3f, remapX_R);
            float scaledY_R = Mathf.Lerp(-0.3f, 0.3f, remapY_R);

            float updatedPivL_X = thumbPivotL_X + scaledX_L;
            float updatedPivL_Y = thumbPivotL_Y + scaledY_L;

            float updatedPivR_X = thumbPivotR_X + scaledX_R;
            float updatedPivR_Y = thumbPivotR_Y + scaledY_R;

            rectTransformThumbL.pivot = new Vector2(updatedPivL_X, updatedPivL_Y);
            rectTransformThumbR.pivot = new Vector2(updatedPivR_X, updatedPivR_Y);

            yield return null; // Wait for next frame
        }
        

        while (elapsed > 2 && elapsed <= 4)
        {
            elapsed += Time.deltaTime;
            time += Time.deltaTime * 5f;

            float cos = Mathf.Cos(time);
            float sin = Mathf.Sin(time);

            float remapX_L = Mathf.InverseLerp(-1, 1, sin); // get a 0-1 range
            float remapY_L = Mathf.InverseLerp(-1, 1, cos); // flipped sin and cos (from above while loop), reverses clockwise/anti-clockwise

            float remapX_R = Mathf.InverseLerp(-1, 1, cos); // get a 0-1 range
            float remapY_R = Mathf.InverseLerp(-1, 1, sin);

            float scaledX_L = Mathf.Lerp(-0.3f, 0.3f, remapX_L);
            float scaledY_L = Mathf.Lerp(-0.3f, 0.3f, remapY_L);

            float scaledX_R = Mathf.Lerp(-0.3f, 0.3f, remapX_R);
            float scaledY_R = Mathf.Lerp(-0.3f, 0.3f, remapY_R);

            float updatedPivL_X = thumbPivotL_X + scaledX_L;
            float updatedPivL_Y = thumbPivotL_Y + scaledY_L;

            float updatedPivR_X = thumbPivotR_X + scaledX_R;
            float updatedPivR_Y = thumbPivotR_Y + scaledY_R;

            rectTransformThumbL.pivot = new Vector2(updatedPivL_X, updatedPivL_Y);
            rectTransformThumbR.pivot = new Vector2(updatedPivR_X, updatedPivR_Y);

            yield return null; // Wait for next frame
        }
        //reset thumb pivots
        rectTransformThumbL.pivot = new Vector2(thumbPivotL_X, thumbPivotL_Y);
        rectTransformThumbR.pivot = new Vector2(thumbPivotR_X, thumbPivotR_Y);

        yield return new WaitForSeconds(2f);

        while (elapsed > 4 && elapsed <= 6)
        {
            triggerR_Lock.SetActive(true);
            canLock = true;

            elapsed += Time.deltaTime;
            time += Time.deltaTime * 5f;

            rectTransformTriggerR.localScale = triggerRScale * (1 + (Mathf.Sin(time) * 0.1f));

            yield return null; // Wait for next frame
        }
        rectTransformTriggerR.localScale = triggerRScale;

        while (alignController.isLocked == false)
        {
            yield return null;
        }
        
        while (elapsed > 6 && elapsed <= 8)
        {
            elapsed += Time.deltaTime;
            time += Time.deltaTime * 5f;

            rectTransformExitButton.localScale = exitButtonScale * (1 + (Mathf.Sin(time) * 0.1f));

            yield return null; // Wait for next frame
        }
        rectTransformExitButton.localScale = exitButtonScale;
    }
}
