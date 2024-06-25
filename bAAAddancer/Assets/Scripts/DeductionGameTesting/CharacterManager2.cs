using System.Collections.Generic;
using UnityEngine;

public class CharacterManager2 : MonoBehaviour
{
    public GameObject[] characters;
    public CharacterStats[] characterStats;

    public int demonIndex = -1;
    public int playerIndex = -1;

    [System.Serializable]
    public class CharacterStats
    {
        public Dictionary<CharacterStat, int> stats = new Dictionary<CharacterStat, int>
        {
            { CharacterStat.IsPlayer, 0 },
            { CharacterStat.IsDemon, 0 },
            { CharacterStat.Cursed, 0 },
            { CharacterStat.Influence, 0 },
            { CharacterStat.Perception, 0 }
        };
    }

    private void Start()
    {
        characterStats = new CharacterStats[characters.Length];
        for (int i = 0; i < characters.Length; i++)
        {
            characterStats[i] = new CharacterStats();
        }

        SetPlayerIndex();
        SetDemonIndex();
        AssignCharacterStats();
    }

    private void SetPlayerIndex()
    {
        playerIndex = Random.Range(0, characters.Length);
    }

    private void SetDemonIndex()
    {
        do
        {
            demonIndex = Random.Range(0, characters.Length);
        } while (demonIndex == playerIndex);
    }

    private void AssignCharacterStats()
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < characters.Length; i++)
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

        for (int i = 0; i < characters.Length; i++)
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

            // Assign good perception if not already assigned to bad perception
            if (goodPerceptionCandidates.Count < 2 && !badPerceptionCandidates.Contains(index))
            {
                perception = 2;
                goodPerceptionCandidates.Add(index);
            }
            // Assign bad perception if not already assigned to good perception
            else if (badPerceptionCandidates.Count < 2 && !goodPerceptionCandidates.Contains(index))
            {
                perception = 0;
                badPerceptionCandidates.Add(index);
            }
            else
            {
                perception = 1;
            }

            // Assign good influence if not already assigned to bad influence and not in good perception
            if (goodInfluenceCandidates.Count < 2 && !badInfluenceCandidates.Contains(index) && !goodPerceptionCandidates.Contains(index))
            {
                influence = 2;
                goodInfluenceCandidates.Add(index);
            }
            // Assign bad influence if not already assigned to good influence and not in bad perception
            else if (badInfluenceCandidates.Count < 2 && !goodInfluenceCandidates.Contains(index) && !badPerceptionCandidates.Contains(index))
            {
                influence = 0;
                badInfluenceCandidates.Add(index);
            }
            else
            {
                influence = 1; // Default random assignment if no specific criteria
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
            }
        }

        return matchingIndices;
    }
}
