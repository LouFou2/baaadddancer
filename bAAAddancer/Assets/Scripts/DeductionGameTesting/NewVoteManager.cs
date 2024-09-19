using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class NewVoteManager : MonoBehaviour
{
    private CharacterManager characterManager;
    private CharacterStatsManager statsManager;
    private SceneSwitcher sceneSwitcher;

    [SerializeField] private Button[] buttons;
    [SerializeField] private Material[] buttonMaterials;
    [SerializeField] private TextMeshProUGUI[] voteTexts;

    private GameObject lastSelectedGameObject;

    [SerializeField] private AudioSource switchSelectAudio;
    [SerializeField] private AudioSource selectAudio;
    [SerializeField] private AudioSource x_Audio;
    [SerializeField] private AudioSource eliminateAudio;

    private int voteIndex = -1;

    [SerializeField] private Canvas sceneCanvas;
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private Button restartGameButton;

    void Start()
    {
        sceneCanvas.gameObject.SetActive(true);
        menuCanvas.gameObject.SetActive(false);

        characterManager = FindObjectOfType<CharacterManager>();
        statsManager = FindObjectOfType<CharacterStatsManager>();
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();

        foreach (TextMeshProUGUI voteText in voteTexts)
        {
            voteText.text = string.Empty;
        }
        foreach (Button button in buttons)
        {
            button.image.material = null;
        }

        // Initialize lastSelectedGameObject with the currently selected GameObject
        lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
    }
    void Update() //literally just handling the change selection sound
    {
        GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

        if (currentSelectedGameObject != lastSelectedGameObject)
        {
            switchSelectAudio.Play();
            lastSelectedGameObject = currentSelectedGameObject;
        }
    }

    public void VoteChar1()
    {
        statsManager.characterStats[0].EliminationVoteCountsInt += 1;
        selectAudio.Play();
        x_Audio.Play();
        voteTexts[0].text = "X";
        StartVoteRoutine();
    }
    public void VoteChar2()
    {
        statsManager.characterStats[1].EliminationVoteCountsInt += 1;
        selectAudio.Play();
        x_Audio.Play();
        voteTexts[1].text = "X";
        StartVoteRoutine();
    }
    public void VoteChar3()
    {
        statsManager.characterStats[2].EliminationVoteCountsInt += 1;
        selectAudio.Play();
        x_Audio.Play();
        voteTexts[2].text = "X";
        StartVoteRoutine();
    }
    public void VoteChar4()
    {
        statsManager.characterStats[3].EliminationVoteCountsInt += 1;
        selectAudio.Play();
        x_Audio.Play();
        voteTexts[3].text = "X";
        StartVoteRoutine();
    }
    public void VoteChar5()
    {
        statsManager.characterStats[4].EliminationVoteCountsInt += 1;
        selectAudio.Play();
        x_Audio.Play();
        voteTexts[4].text = "X";
        StartVoteRoutine();
    }
    public void VoteChar6()
    {
        statsManager.characterStats[5].EliminationVoteCountsInt += 1;
        selectAudio.Play();
        x_Audio.Play();
        voteTexts[5].text = "X";
        StartVoteRoutine();
    }

    public void StartVoteRoutine()
    {
        StartCoroutine(VoteRoutine());
    }

    public IEnumerator VoteRoutine()
    {
        foreach (CharacterStatsSO charStat in statsManager.characterStats)
        {
            // Delay before each vote
            yield return new WaitForSeconds(0.2f); // Adjust delay timing if needed

            if (charStat.VoteTargetInt == 0)
            {
                statsManager.characterStats[0].EliminationVoteCountsInt += 1;
                voteTexts[0].text += "X";
                x_Audio.Play();
            }
            if (charStat.VoteTargetInt == 1)
            {
                statsManager.characterStats[1].EliminationVoteCountsInt += 1;
                voteTexts[1].text += "X";
                x_Audio.Play();
            }
            if (charStat.VoteTargetInt == 2)
            {
                statsManager.characterStats[2].EliminationVoteCountsInt += 1;
                voteTexts[2].text += "X";
                x_Audio.Play();
            }
            if (charStat.VoteTargetInt == 3)
            {
                statsManager.characterStats[3].EliminationVoteCountsInt += 1;
                voteTexts[3].text += "X";
                x_Audio.Play();
            }
            if (charStat.VoteTargetInt == 4)
            {
                statsManager.characterStats[4].EliminationVoteCountsInt += 1;
                voteTexts[4].text += "X";
                x_Audio.Play();
            }
            if (charStat.VoteTargetInt == 5)
            {
                statsManager.characterStats[5].EliminationVoteCountsInt += 1;
                voteTexts[5].text += "X";
                x_Audio.Play();
            }
        }

        int highestVoteCount = -1;

        for(int i = 0; i < statsManager.characterStats.Length; i++)
        {
            if (statsManager.characterStats[i].EliminationVoteCountsInt > highestVoteCount)
            {
                highestVoteCount = statsManager.characterStats[i].EliminationVoteCountsInt;
                voteIndex = i;
            }
        }

        // use shader material for selected character
        buttons[voteIndex].image.material = buttonMaterials[voteIndex];

        // Lerp shader float value (for the "burn" effect)
        Material burnMaterial = buttons[voteIndex].image.material;
        string shaderProperty = "_NoiseFade";
        float targetValue = 1.0f;
        float duration = 1.0f;

        eliminateAudio.Play();
        yield return LerpShaderFloat(burnMaterial, shaderProperty, targetValue, duration);

        //make sure the character is eliminated
        characterManager.characterDataSOs[voteIndex].wasEliminated = true;

        EndScene();
    }
    private IEnumerator LerpShaderFloat(Material material, string property, float targetValue, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float value = Mathf.Lerp(0, targetValue, time / duration);
            material.SetFloat(property, value);
            yield return null;
        }

        material.SetFloat(property, targetValue); // Ensure the final value is set
    }

    private void EndScene()
    {
        if (characterManager.characterDataSOs[voteIndex].characterRoleSelect != CharacterData.CharacterRole.Player)
        {
            sceneSwitcher.SwitchToNextLevelKey();
            sceneSwitcher.LoadSceneByName("MakeDance");
        }
        else
        {
            GameOver();
        }
    }
    private void GameOver()
    {
        sceneCanvas.gameObject.SetActive(false);
        menuCanvas.gameObject.SetActive(true);
        restartGameButton.Select();
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }
}
