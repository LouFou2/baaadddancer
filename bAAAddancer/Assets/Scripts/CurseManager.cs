using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseManager : MonoBehaviour
{
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private CharacterProfile characterProfile;
    private CharacterData charData;
    
    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        characterProfile = GetComponent<CharacterProfile>();
        charData = characterProfile.characterDataSO;
    }

    // Update is called once per frame
    void Update()
    {
        characterAnimator.SetFloat("CurseAmount", charData.infectionLevel);
    }
}
