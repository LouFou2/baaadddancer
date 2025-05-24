using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DebugUI_Manager : MonoBehaviour // this manager also supervises the debug audio
{
    [SerializeField] private GameObject abstractsButtonsPanel;
    [SerializeField] private GameObject renderTexturePanel;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject alignerGroup;

    [SerializeField] private Button[] abstractButtons;

    [SerializeField] private CameraManager camManager;
    [SerializeField] private DebugGameAudioManager debugAudioManager;
    [SerializeField] private AlignerController2 alignerController;
    [SerializeField] private DancerShaderHandler abstractsShadersHandler;

    private int selectedCharacter = -1;
    private float debuggedCharFinalAlignment = 0;

    private bool debugRunning = false;
    private bool alignerRunning = false;

    private PlayerControls playerControls;

    public static event System.Action On_DebugComplete; //subscribed to by the dialogue manager_002

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    private void OnEnable()
    {
        playerControls.Enable();
        AlignerController2.On_AlignerComplete += AlignerCompleteHandler; // from the aligner controller
    }
    private void OnDisable()
    {
        playerControls.Disable();
        AlignerController2.On_AlignerComplete -= AlignerCompleteHandler; // from the aligner controller
    }
    private void Start()
    {
        abstractsButtonsPanel.SetActive(false);
        renderTexturePanel.SetActive(false);
        exitButton.SetActive(false);
        alignerGroup.SetActive(false);
    }
    public void StartDebugUI()
    {
        debugRunning = true;

        debugAudioManager.StartDebugUIAudio(); // this is a lazy way to code I know

        abstractsButtonsPanel.SetActive(true);
        renderTexturePanel.SetActive(true);
        exitButton.SetActive(true);

        EventSystem.current.SetSelectedGameObject(abstractButtons[0].gameObject);
        abstractButtons[0].Select();
    }
    public void EndDebugUI()
    {
        debugRunning = false;

        debugAudioManager.EndDebugUIAudio(); // this is a lazy way to code I know

        abstractsButtonsPanel.SetActive(false);
        renderTexturePanel.SetActive(false);
        exitButton.SetActive(false);
        alignerGroup.SetActive(false);

        abstractsShadersHandler.UpdateDancerAbstractShaders();

        On_DebugComplete?.Invoke(); //subscribed to by the dialogue manager
    }
    private void Update()
    {
        if (!debugRunning || alignerRunning) // we don't need this button+camera switching logic to run all the time!
        {
            return;
        }
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current != null)
        {
            for (int i = 0; i < abstractButtons.Length; i++)
            {
                if (current == abstractButtons[i].gameObject)
                {
                    selectedCharacter = i;
                    camManager.SetCamera(i);
                }
            }
        }

        // exit
        if (playerControls.GenericInput.YButton.triggered)
        {
            EndDebugUI();
        }
    }
    public void ButtonClicked()
    {
        alignerRunning = true;

        abstractsButtonsPanel.SetActive(false);
        renderTexturePanel.SetActive(false);
        exitButton.SetActive(false);
        alignerGroup.SetActive(true);

        // this is a lazy way to code I know
        alignerController.StartAligner();
        debugAudioManager.StartAlignerAudio();

        // we can use the selectedButton index + "finalAlignedAmount" from alignerController
        // to adjust character[at index] curse level
    }
    void AlignerCompleteHandler()
    {
        debugAudioManager.EndAlignerAudio(); // ***another lazy lazy move

        alignerRunning = false;

        abstractsButtonsPanel.SetActive(true);
        renderTexturePanel.SetActive(true);
        exitButton.SetActive(true);
        alignerGroup.SetActive(false);

        EventSystem.current.SetSelectedGameObject(abstractButtons[0].gameObject);
        abstractButtons[0].Select();

        debuggedCharFinalAlignment = alignerController.GetFinalAlignedAmount();
    }
    public int GetSelectedDebugChar()
    {
        return selectedCharacter;
    }
    public float GetDebugCharAlignment()
    {
        return debuggedCharFinalAlignment;
    }
}
