using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DebugUI_Manager : MonoBehaviour
{
    [SerializeField] private GameObject abstractsButtonsPanel;
    [SerializeField] private GameObject renderTexturePanel;
    [SerializeField] private GameObject alignerGroup;

    [SerializeField] private Button[] abstractButtons;

    [SerializeField] private CameraManager camManager;
    [SerializeField] private DebugGameAudioManager debugAudioManager;

    private int selectedButton = -1;

    private void Start()
    {
        abstractsButtonsPanel.SetActive(false);
        renderTexturePanel.SetActive(false);
        alignerGroup.SetActive(false);
    }
    public void StartDebugUI()
    {
        abstractsButtonsPanel.SetActive(true);
        renderTexturePanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(abstractButtons[0].gameObject);
        abstractButtons[0].Select();
    }
    private void Update()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current != null)
        {
            for (int i = 0; i < abstractButtons.Length; i++)
            {
                if (current == abstractButtons[i].gameObject)
                {
                    selectedButton = i;
                    camManager.SetCamera(i);
                }
            }
        }
    }
    public void ButtonClicked()
    {
        abstractsButtonsPanel.SetActive(false);
        renderTexturePanel.SetActive(false);
        alignerGroup.SetActive(true);

        debugAudioManager.alignerGameRunning = true; // this is a lazy way to code I know

        // we can use the selectedButton index
    }
    
}
