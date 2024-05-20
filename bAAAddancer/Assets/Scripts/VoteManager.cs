using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VoteManager : MonoBehaviour
{
    private CharacterManager characterManager;
    private SceneSwitcher sceneSwitcher;

    [SerializeField] private Button[] buttons;
    [SerializeField] private TextMeshProUGUI[] voteTexts;

    private int playerVoteIndex = -1;

    void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        sceneSwitcher = FindObjectOfType<SceneSwitcher>();

        foreach (CharacterData characterData in characterManager.characterDataSOs)
        {
            characterData.wasEliminated = false;
        }
        foreach (TextMeshProUGUI voteText in voteTexts) 
        {
            voteText.text = string.Empty;
        }
    }

    public void VoteChar1() 
    {
        characterManager.characterDataSOs[0].wasEliminated = true;
        voteTexts[0].text = "X";
        playerVoteIndex = 0;
    }
    public void VoteChar2()
    {
        characterManager.characterDataSOs[1].wasEliminated = true;
        voteTexts[1].text = "X";
        playerVoteIndex = 1;
    }
    public void VoteChar3()
    {
        characterManager.characterDataSOs[2].wasEliminated = true;
        voteTexts[2].text = "X";
        playerVoteIndex = 2;
    }
    public void VoteChar4()
    {
        characterManager.characterDataSOs[3].wasEliminated = true;
        voteTexts[3].text = "X";
        playerVoteIndex = 3;
    }
    public void VoteChar5()
    {
        characterManager.characterDataSOs[4].wasEliminated = true;
        voteTexts[4].text = "X";
        playerVoteIndex = 4;
    }
    public void VoteChar6()
    {
        characterManager.characterDataSOs[5].wasEliminated = true;
        voteTexts[5].text = "X";
        playerVoteIndex = 5;
    }

    public void StartVoteRoutine() 
    {
        StartCoroutine(VoteRoutine());
    }

    public IEnumerator VoteRoutine() 
    {
        List<CharacterData> charDataList = new List<CharacterData>(characterManager.characterDataSOs);
        int playerIndex = -1;
        int bugIndex = -1;
        int randomNPC_Index = -1;

        // Find indices of Player and Bug roles
        for (int i = 0; i < charDataList.Count; i++)
        {
            if (charDataList[i].characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                playerIndex = i;
            }
            else if (charDataList[i].characterRoleSelect == CharacterData.CharacterRole.Bug)
            {
                bugIndex = i;
            }
        }

        // Remove Player and Bug from the list based on found indices
        if (playerIndex != -1)
        {
            charDataList.RemoveAt(playerIndex);
            // Adjust bugIndex if it was after playerIndex
            if (bugIndex > playerIndex)
            {
                bugIndex--;
            }
        }

        if (bugIndex != -1)
        {
            charDataList.RemoveAt(bugIndex);
        }

        // Optionally, find a random NPC index from the remaining list
        if (charDataList.Count > 0)
        {
            randomNPC_Index = Random.Range(0, charDataList.Count);
            Debug.Log($"Random NPC Index: {randomNPC_Index}");
        }

        yield return new WaitForSeconds(0.5f);
        voteTexts[playerVoteIndex].text = "XXX";

        voteTexts[playerIndex].text = "X";
        yield return new WaitForSeconds(0.2f);

        voteTexts[randomNPC_Index].text = voteTexts[randomNPC_Index].text + "XX";
        yield return new WaitForSeconds(0.5f);

        // Lerp shader float value (for the "burn" effect)
        Material playerMaterial = buttons[playerVoteIndex].image.material;
        string shaderProperty = "_NoiseFade";
        float targetValue = 1.0f;
        float duration = 1.0f;

        yield return LerpShaderFloat(playerMaterial, shaderProperty, targetValue, duration);

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
        sceneSwitcher.LoadNextScene();
    }

}
