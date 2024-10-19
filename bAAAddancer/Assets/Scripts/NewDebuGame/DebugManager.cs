using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DebugManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private SceneSwitcher sceneSwitcher;

    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private GameObject endButtonObject;
    [SerializeField] private GameObject dialogueObjectParent;

    [SerializeField] private CharacterData[] charsData;
    [SerializeField] private GameObject[] chars;
    [SerializeField] private GameObject[] charQuadObjects;
    [SerializeField] private GameObject charSelectObject;

    [SerializeField] private GameObject LThumbObject;
    [SerializeField] private GameObject RThumbObject;

    [SerializeField] private GameObject LLockObject; //the left trigger object
    [SerializeField] private GameObject RUnlockObject; //the right trigger object

    [SerializeField] private GameObject exitAlignObject;

    [SerializeField] private GameObject[] panel1Objects;
    [SerializeField] private GameObject[] panel2Objects;

    private bool isDialogue = true;
    private bool isChoosingDebugChar = false;
    private bool endOption = false;

    public int charDebuggedIndex = -1;

    private List<int> cursedCharsList;

    private bool isTweening = false;

    private bool isLocked = false;

    public bool xCurrentlyPressed = false; // this flag just stops the x being processed immediately when returning from alignment

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
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();

        for(int i = 0; i < chars.Length; i++)
        {
            chars[i].SetActive(false);

            if (charsData[i].lastCursedCharacter)
            {
                chars[i].SetActive(true);
                charsData[i].wasDebuggedLastRound = true;
            }
            if (charsData[i].characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                chars[i].SetActive(true);
                // Deactivate all child objects
                foreach (Transform child in chars[i].transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        foreach (GameObject panel1Object in panel1Objects)
        {
            panel1Object.SetActive(false);
        }
        foreach (GameObject panel2Object in panel2Objects)
        {
            panel2Object.SetActive(false);
        }

        endButtonObject.SetActive(false);
    }

    public void StartDebugGame()
    {
        isDialogue = false;
        dialogueObjectParent.SetActive(false);
        isChoosingDebugChar = true;

        foreach (GameObject character in chars)
        {
            character.SetActive(false);
        }
        foreach (GameObject panel1Object in panel1Objects)
        {
            panel1Object.SetActive(true);
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

        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    private void Update()
    {
        if (isChoosingDebugChar && !isDialogue)
        {
            charSelectObject.SetActive(true);

            if (!isTweening && !endOption)
            {
                charSelectObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                charSelectObject.transform.DOScale(0.8f, 0.4f).SetLoops(-1, LoopType.Yoyo);
                isTweening = true;
            }
        }

        if(!isChoosingDebugChar && !isDialogue)
        {
            // If not Choosing Char to Debug, we are in the Alignment Screen

            //in Update we only handle the player input
            if (playerControls.GenericInput.LTrigger.IsPressed() && !isLocked) //just adding the bool check so method isn't called extra times
            {
                isLocked = true;
                UnlockButtonTween();

            }
            if (playerControls.GenericInput.RTrigger.IsPressed() && isLocked)
            {
                isLocked = false;
                LockButtonTween();
            }
        }

        if (endOption && !isTweening)
        {
            endButtonObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            endButtonObject.transform.DOScale(0.8f, 0.4f).SetLoops(-1, LoopType.Yoyo);
            isTweening = true;
        }
        if (endOption && xCurrentlyPressed && !playerControls.GenericInput.XButton.IsPressed())
        {
            xCurrentlyPressed = false;
        }
        if (endOption && !xCurrentlyPressed && playerControls.GenericInput.XButton.IsPressed())
        {
            EndDebug();
        }
    }

    public void StartAlignment()
    {
        isTweening = false;

        DOTween.KillAll();

        isChoosingDebugChar = false;

        endOption = false;

        foreach (GameObject panel1Object in panel1Objects)
        {
            panel1Object.SetActive(false);
        }
        foreach (GameObject panel2Object in panel2Objects)
        {
            panel2Object.SetActive(true);
        }

        LLockObject.SetActive(false);
        RUnlockObject.SetActive(false);

        LThumbObject.transform.DORotate(new Vector3(0f, 45f, 0f), 0.4f, RotateMode.FastBeyond360).SetLoops(16, LoopType.Yoyo);
        RThumbObject.transform.DORotate(new Vector3(45f, 0f, 0f), 0.4f, RotateMode.FastBeyond360).SetLoops(16, LoopType.Yoyo).OnComplete(LockButtonTween);
    }

    void LockButtonTween()
    {
        if (!isLocked)
        {
            LLockObject.SetActive(true);
            RUnlockObject.SetActive(false);
            LLockObject.transform.DOScale(1f, 0.4f).SetLoops(8, LoopType.Yoyo);
        }
    }
    void UnlockButtonTween()
    {
        if (isLocked)
        {
            LLockObject.SetActive(false);
            RUnlockObject.SetActive(true);
            RUnlockObject.transform.DOScale(1f, 0.4f).SetLoops(8, LoopType.Yoyo).OnComplete(TweenReturnButton);
        }
    }
    void TweenReturnButton()
    {
        exitAlignObject.transform.DOScale(0.8f, 0.4f).SetLoops(8, LoopType.Yoyo);
    }

    public void EndAlignment(float finalDiscrepencyValue)
    {
        for (int i = 0; i < charQuadObjects.Length; i++)
        {
            if (i == charDebuggedIndex)
            {
                Material quadMat = charQuadObjects[i].GetComponent<MeshRenderer>().material;

                charsData[i].infectionLevel *= finalDiscrepencyValue;
                quadMat.SetFloat("_GlitchLerp", charsData[i].infectionLevel);
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

        isChoosingDebugChar = true;

        endButtonObject.SetActive(true); 
        endOption = true;
    }

    public void Char0Debug()
    {
        charDebuggedIndex = 0;
        if (cursedCharsList.Contains(0))
        {
            StartAlignment();
        }
    }
    public void Char1Debug()
    {
        charDebuggedIndex = 1;
        if (cursedCharsList.Contains(1))
        {
            StartAlignment();
        }
    }
    public void Char2Debug()
    {
        charDebuggedIndex = 2;
        if (cursedCharsList.Contains(2))
        {
            StartAlignment();
        }
    }
    public void Char3Debug()
    {
        charDebuggedIndex = 3;
        if (cursedCharsList.Contains(3))
        {
            StartAlignment();
        }
    }
    public void Char4Debug()
    {
        charDebuggedIndex = 4;
        if (cursedCharsList.Contains(4))
        {
            StartAlignment();
        }
    }
    public void Char5Debug()
    {
        charDebuggedIndex = 5;
        if (cursedCharsList.Contains(5))
        {
            StartAlignment();
        }
    }
    private void EndDebug()
    {
        sceneSwitcher.SwitchToNextLevelKey();
        sceneSwitcher.LoadSceneByName("DialogueScene");
    }
}
