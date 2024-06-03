using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDemonTransition : MonoBehaviour
{
    private List<Material> transitionsMaterials = new List<Material>();
    [SerializeField] private GameObject replacementCharacter; //assign in inspector

    private ClockCounter clock;
    void Start()
    {
        clock = FindObjectOfType<ClockCounter>();

        if (replacementCharacter != null)
            replacementCharacter.SetActive(false);

        // Find all Renderer components in the children of this GameObject
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Loop through each Renderer to get its materials
        foreach (Renderer renderer in renderers)
        {
            // Add each material of the Renderer to the list
            transitionsMaterials.AddRange(renderer.materials);
        }
    }
    public void CharacterTransitionToDemon()  //this is the method to be called from RevealDemon script (UnityEvent in Inspector)
    {
        transform.position = new Vector3(0,0,0); // snap character to world 0,0,0
        if(gameObject.activeSelf == true)
            StartCoroutine(TransitionMaterials());
    }
    private IEnumerator TransitionMaterials() 
    {
        float transitionDuration = clock.Get_Q_BeatInterval() * 8; //q beat interval is 1/16 measure
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float transitionValue = Mathf.Clamp01(elapsedTime / transitionDuration);  // Normalize to [0, 1]

            // Iterate through each material in transitionsMaterials and set the _Transition property
            foreach (Material material in transitionsMaterials)
            {
                material.SetFloat("_Transition", transitionValue);
            }

            yield return null;  // Wait for the next frame
        }

        // Ensure the final value is set
        foreach (Material material in transitionsMaterials)
        {
            material.SetFloat("_Transition", 1f);
        }

        DeActivateCharacter();
    }
    void DeActivateCharacter() 
    {
        gameObject.SetActive(false);

        if(replacementCharacter != null)
            replacementCharacter.SetActive(true);
    }

}
