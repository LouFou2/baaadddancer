using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private SceneSwitcher sceneSwitcher;

    [SerializeField] private GameObject endButtonObject;


    [SerializeField] private CharacterData[] charsData;
    [SerializeField] private GameObject[] charQuadObjects;

    [SerializeField] private GameObject[] panel1Objects;
    [SerializeField] private GameObject[] panel2Objects;

    private bool endOption = false;

    public int charDebuggedIndex = -1;

    private List<int> cursedCharsList;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        foreach (GameObject panel1Object in panel1Objects)
        {
            panel1Object.SetActive(true);
        }
        foreach (GameObject panel2Object in panel2Objects)
        {
            panel2Object.SetActive(false);
        }

        cursedCharsList = new List<int>();

        for (int i = 0; i < charQuadObjects.Length; i++)
        {
            if (charsData[i].infectionLevel > 0)
            {
                cursedCharsList.Add(i);

                charQuadObjects[i].GetComponent<MeshRenderer>().material.SetFloat("_GlitchLerp", charsData[i].infectionLevel);
            }
        }

        endButtonObject.SetActive(false);
        
    }
    private void Update()
    {
        if (endOption && playerControls.GenericInput.XButton.IsPressed())
        {
            EndDebug();
        }
    }

    public void StartDebug()
    {
        endOption = false;

        foreach (GameObject panel1Object in panel1Objects)
        {
            panel1Object.SetActive(false);
        }
        foreach (GameObject panel2Object in panel2Objects)
        {
            panel2Object.SetActive(true);
        }
    }
    public void EndAlignment(float finalDiscrepencyValue)
    {
        for (int i = 0; i < charQuadObjects.Length; i++)
        {
            if (i == charDebuggedIndex)
            {
                Material quadMat = charQuadObjects[i].GetComponent<MeshRenderer>().material;

                quadMat.SetFloat("_GlitchLerp", finalDiscrepencyValue);
                charsData[i].infectionLevel = finalDiscrepencyValue;

            }
        }

        foreach (GameObject panel1Object in panel1Objects)
        {
            panel1Object.SetActive(true);
        }
        foreach (GameObject panel2Object in panel2Objects)
        {
            panel2Object.SetActive(false);
        }

        endButtonObject.SetActive(true); 
        endOption = true;
    }
    public void Char0Debug()
    {
        charDebuggedIndex = 0;
        if (cursedCharsList.Contains(0))
        {
            StartDebug();
        }
    }
    public void Char1Debug()
    {
        charDebuggedIndex = 1;
        if (cursedCharsList.Contains(1))
        {
            StartDebug();
        }
    }
    public void Char2Debug()
    {
        charDebuggedIndex = 2;
        if (cursedCharsList.Contains(2))
        {
            StartDebug();
        }
    }
    public void Char3Debug()
    {
        charDebuggedIndex = 3;
        if (cursedCharsList.Contains(3))
        {
            StartDebug();
        }
    }
    public void Char4Debug()
    {
        charDebuggedIndex = 4;
        if (cursedCharsList.Contains(4))
        {
            StartDebug();
        }
    }
    public void Char5Debug()
    {
        charDebuggedIndex = 5;
        if (cursedCharsList.Contains(5))
        {
            StartDebug();
        }
    }
    private void EndDebug()
    {
        sceneSwitcher.SwitchToNextLevelKey();
        sceneSwitcher.LoadSceneByName("DialogueScene");
    }
}
