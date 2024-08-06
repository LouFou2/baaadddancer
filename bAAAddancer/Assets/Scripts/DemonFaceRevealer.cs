using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFaceRevealer : MonoBehaviour
{
    private CharacterManager characterManager;
    private ClockCounter clock;
    [SerializeField] private Animator raveDemonAnimator; //assign in inspector

    [SerializeField] GameObject[] demonFaces; //assign these in inspector, in same order as characters in Character Manager
    [SerializeField] private GameObject raveDemonFace;
    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        clock = FindObjectOfType<ClockCounter>();

        foreach (GameObject demonFace in demonFaces) 
        {
            demonFace.SetActive(false);
        }

        for (int i = 0; i < characterManager.characterDataSOs.Length; i++) 
        {
            if (characterManager.characterDataSOs[i].characterRoleSelect == CharacterData.CharacterRole.Demon) 
            {
                raveDemonFace = demonFaces[i];
            }
        }
    }

    public void RevealDemonFace() 
    {
        StartCoroutine(DemonFaceReveal());
    }
    private IEnumerator DemonFaceReveal() 
    {
        float transitionDuration = clock.Get_Q_BeatInterval() * 8; //q beat interval is 1/16 measure

        yield return new WaitForSeconds(transitionDuration);

        raveDemonFace.SetActive(true);

        yield return new WaitForSeconds(transitionDuration);

        raveDemonAnimator.SetBool("DemonSmile", true);
    }
}
