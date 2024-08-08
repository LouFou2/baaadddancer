using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    // probably need to plug into the Character Manager
    [SerializeField] private CharacterManager characterManager;
    public CharacterStats[] characterStats;

    public int demonIndex = -1;
    public int playerIndex = -1;

    private bool gameStarted = false;

    [System.Serializable]
    public class CharacterStats
    {
        public Dictionary<CharacterStat, int> stats = new Dictionary<CharacterStat, int>
        {
            { CharacterStat.IsPlayer, 0 }, // player and demon: 0 false, 1 true
            { CharacterStat.IsDemon, 0 },
            { CharacterStat.Cursed, 0 }, // cursed: t.b.d useful index range... 0-15? (4 for each round?)
            { CharacterStat.LastBugged, 0},

            { CharacterStat.Influence, 0 }, // influence and perception: 0 bad, 1 neutral, 2 good
            { CharacterStat.Perception, 0 },

            { CharacterStat.SpokenAmount, 0},
            { CharacterStat.CurrentSpeaker, 0},
            { CharacterStat.PreviousSpeaker, 0},
            { CharacterStat.CurrentSpokenTo, 0},
            { CharacterStat.PreviousSpokenTo, 0},

            { CharacterStat.SpeakToGroup, 0},
            { CharacterStat.SpeakToCamera, 0},
        };
    }
    private void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        if (GameManager.Instance.GetCurrentLevelKey() == LevelKey.IntroDialogue && !gameStarted) 
        {
            SetupNewGameStats();
        }
    }

    private void SetupNewGameStats()
    {
        gameStarted = true;

        characterStats = new CharacterStats[characterManager.characterDataSOs.Length];
        for (int i = 0; i < characterStats.Length; i++)
        {
            characterStats[i] = new CharacterStats();
        }

        SetPlayerIndex();
        SetDemonIndex();
        AssignCharacterStats();
    }

    private void SetPlayerIndex()
    {
        //playerIndex = Random.Range(0, characters.Length);
        playerIndex = characterManager.playerIndex;
    }

    private void SetDemonIndex()
    {
        /*do
        {
            demonIndex = Random.Range(0, characters.Length);
        } while (demonIndex == playerIndex);*/
        demonIndex = characterManager.demonIndex;
    }

    private void AssignCharacterStats()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < characterStats.Length; i++)
        {
            indices.Add(i);
        }

        // Shuffle the indices list
        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Count);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        List<int> goodPerceptionCandidates = new List<int>();
        List<int> goodInfluenceCandidates = new List<int>();
        List<int> badPerceptionCandidates = new List<int>();
        List<int> badInfluenceCandidates = new List<int>();

        for (int i = 0; i < characterStats.Length; i++)
        {
            int index = indices[i];

            if (index == playerIndex)
            {
                characterStats[index].stats[CharacterStat.IsPlayer] = 1;
            }
            if (index == demonIndex)
            {
                characterStats[index].stats[CharacterStat.IsDemon] = 1;
            }

            int perception = 0;
            int influence = 0;

            // WOULD BE GOOD TO MAKE THE ASSGNMENTS A BIT BETTER ** see notes

            // Assign good perception if not already assigned to bad perception (** this will probably be the first two indices)
            if (goodPerceptionCandidates.Count < 2 && !badPerceptionCandidates.Contains(index))
            {
                perception = 2;
                goodPerceptionCandidates.Add(index);
            }
            // Assign bad perception if not already assigned to good perception (** and the next two indices)
            else if (badPerceptionCandidates.Count < 2 && !goodPerceptionCandidates.Contains(index))
            {
                perception = 0;
                badPerceptionCandidates.Add(index);
            }
            else
            {
                perception = 1; // (** the final two indices...)
            }

            // Assign good influence if not already assigned to bad influence and not in good perception (** so it will select the second pair of indices)
            if (goodInfluenceCandidates.Count < 2 && !badInfluenceCandidates.Contains(index) && !goodPerceptionCandidates.Contains(index))
            {
                influence = 2;
                goodInfluenceCandidates.Add(index);
            }
            // Assign bad influence if not already assigned to good influence and not in bad perception (** and here the first pair again)
            else if (badInfluenceCandidates.Count < 2 && !goodInfluenceCandidates.Contains(index) && !badPerceptionCandidates.Contains(index))
            {
                influence = 0;
                badInfluenceCandidates.Add(index);
            }
            else
            {
                influence = 1; // Default random assignment if no specific criteria (** so leaving the final pair again for "middle" stat)
            }

            characterStats[index].stats[CharacterStat.Influence] = influence;
            characterStats[index].stats[CharacterStat.Perception] = perception;
        }
    }

    public Dictionary<CharacterStat, int> GetCharacterStats(int index)
    {
        if (index >= 0 && index < characterStats.Length)
        {
            return characterStats[index].stats;
        }
        return null;
    }
    public void ModifyCharacterStat(int characterIndex, CharacterStat characterStat, int newValue)
    {
        if (characterIndex >= 0 && characterIndex < characterStats.Length)
        {
            if (characterStats[characterIndex].stats.ContainsKey(characterStat))
            {
                characterStats[characterIndex].stats[characterStat] = newValue;
                /*if(newValue == 1)
                    Debug.Log("Key: " + characterStat + " :" + newValue + ", char" + characterIndex);*/
            }
            /*else
            {
                characterStats[characterIndex].stats.Add(characterStat, newValue);
            }*/
        }
    }

    public List<int> GetMatchingCharacterIndices(Dictionary<CharacterStat, int> queryCriteria)
    {
        List<int> matchingIndices = new List<int>();

        for (int i = 0; i < characterStats.Length; i++)
        {
            bool match = true;
            foreach (var criterion in queryCriteria)
            {
                if (!characterStats[i].stats.ContainsKey(criterion.Key) || characterStats[i].stats[criterion.Key] != criterion.Value)
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                matchingIndices.Add(i);
                Debug.Log("Found match: " + i);
            }
        }

        return matchingIndices;
    }
}
