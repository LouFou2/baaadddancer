using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField] private CharacterData[] charsData;
    [SerializeField] private GameObject[] charQuadObjects;

    [SerializeField] private GameObject[] panel1Objects;
    [SerializeField] private GameObject[] panel2Objects;

    public int charDebuggedIndex = -1;

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
        for (int i = 0; i < charQuadObjects.Length; i++)
        {
            if (charsData[i].infectionLevel > 0)
            {
                charQuadObjects[i].GetComponent<MeshRenderer>().material.SetFloat("_GlitchLerp", charsData[i].infectionLevel);
            }
        }

    }

    public void StartDebug()
    {
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
                Debug.Log("Debug Char is: " + charDebuggedIndex);
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
    }
    public void Char0Debug()
    {
        charDebuggedIndex = 0;
    }
    public void Char1Debug()
    {
        charDebuggedIndex = 1;
    }
    public void Char2Debug()
    {
        charDebuggedIndex = 2;
    }
    public void Char3Debug()
    {
        charDebuggedIndex = 3;
    }
    public void Char4Debug()
    {
        charDebuggedIndex = 4;
    }
    public void Char5Debug()
    {
        charDebuggedIndex = 5;
    }
}
