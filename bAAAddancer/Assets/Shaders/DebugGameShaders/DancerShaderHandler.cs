using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerShaderHandler : MonoBehaviour
{
    [SerializeField] private Material[] dancerAbstractMat; //assign in inspector
    [SerializeField] private ClockCounter counter;
    [SerializeField] private CharacterManager charManager;

    [SerializeField] float time = 0;
    float beatDuration; // to be used to lerp glitch effect
    [SerializeField] float lerpValue;

    private void OnEnable()
    {
        ClockCounter.On_Q_Beat_Trigger += On_Q_BeatHandler; // Subscribe to the beat trigger event
    }
    private void OnDisable()
    {
        ClockCounter.On_Q_Beat_Trigger -= On_Q_BeatHandler; // Subscribe to the beat trigger event
    }

    private void Start()
    {
        counter = FindObjectOfType<ClockCounter>();
        charManager = FindObjectOfType<CharacterManager>();

        CharacterData[] characterData = new CharacterData[6];

        for (int i = 0; i < characterData.Length; i++)
        {
            characterData[i] = charManager.characterDataSOs[i];
            dancerAbstractMat[i].SetFloat("_CursedAmount", characterData[i].infectionLevel);
        }
    }
    void Update()
    {
        time += Time.deltaTime;
        float elapsedTime = beatDuration - time;
        if (elapsedTime <= 0) time = 0;

        lerpValue = Mathf.InverseLerp(0, beatDuration, elapsedTime);

        for (int i = 0; i < dancerAbstractMat.Length; i++)
        {
            dancerAbstractMat[i].SetFloat("_beatDuration", beatDuration);
            dancerAbstractMat[i].SetFloat("_beatElapsed", lerpValue);
        }
    }
    public void UpdateDancerAbstractShaders() // will be called by the DebugUI_Manager after debug is finished: EndDebugUI()
    {
        CharacterData[] characterData = new CharacterData[6];

        for (int i = 0; i < characterData.Length; i++)
        {
            characterData[i] = charManager.characterDataSOs[i];
            dancerAbstractMat[i].SetFloat("_CursedAmount", characterData[i].infectionLevel);
        }
    }

    void On_Q_BeatHandler()
    {
        beatDuration = counter.Get_Q_BeatInterval() * 4;
    }
}
