using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float blinkInterval;
    [SerializeField] private float blinkDuration = 0.06f;
    private float counter;
    private bool isBlinking;
    void Start()
    {
        animator = GetComponent<Animator>();
        counter = 0f;
        isBlinking = false;
        blinkInterval = Random.Range(2f, 4f);
    }
    private void Update()
    {
        if(!isBlinking)
            counter += Time.deltaTime;

        if (counter >= blinkInterval && !isBlinking) 
        {
            animator.SetBool("Blink", true);
            StartCoroutine(BlinkHandler());
        }
    }
    private IEnumerator BlinkHandler() 
    {
        isBlinking = true;
        yield return new WaitForSeconds(blinkDuration);
        animator.SetBool("Blink", false);
        counter = 0f;
        isBlinking = false;
    }
}
