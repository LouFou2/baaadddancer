using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndTitleManager : MonoBehaviour
{
    private CharacterManager characterManager;
    [SerializeField] private bool raveDemonEliminated = true;
    [SerializeField] private GameObject demonWinsTitle;
    [SerializeField] private GameObject playerWinsTitle;

    private ClockCounter clock;

    void Start()
    {
        clock = FindObjectOfType<ClockCounter>();

        raveDemonEliminated = true;

        demonWinsTitle.SetActive(false);
        playerWinsTitle.SetActive(false);

        characterManager = FindObjectOfType<CharacterManager>();
        for (int i = 0; i < characterManager.characterDataSOs.Length; i++)
        {
            CharacterData characterData = characterManager.characterDataSOs[i];
            if (characterData.characterRoleSelect == CharacterData.CharacterRole.Bug && !characterData.wasEliminated)
            {
                raveDemonEliminated = false;
            }
        }
    }

    public void ShowEndTitle()
    {
        if (raveDemonEliminated) playerWinsTitle.SetActive(true);
        else demonWinsTitle.SetActive(true);
        StartCoroutine(DisableWinnerTitles());
    }

    private IEnumerator DisableWinnerTitles() 
    {
        float winnerTitleDuration = clock.GetBeatInterval() * 16;
        yield return new WaitForSeconds(winnerTitleDuration);

        playerWinsTitle.SetActive(false);
        demonWinsTitle.SetActive(false);
    }

    public void QuitGame() 
    {
        Application.Quit();
    }

}
