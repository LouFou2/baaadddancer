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
        List<int> goodPerceptionCandidates = new List<int>();
        List<int> goodInfluenceCandidates = new List<int>();

        for (int i = 0; i < characters.Length; i++)
        {
            if (i == playerIndex)
            {
                characterStats[i].stats[CharacterStat.IsPlayer] = 1;
            }
            if (i == demonIndex)
            {
                characterStats[i].stats[CharacterStat.IsDemon] = 1;
            }

            int perception = 0;
            int influence = 0;

            if (goodPerceptionCandidates.Count < 2)
            {
                perception = 2;
                goodPerceptionCandidates.Add(i);
            }
            else
            {
                perception = Random.Range(0, 2);
            }

            if (goodInfluenceCandidates.Count < 2 && perception != 2)
            {
                influence = 2;
                goodInfluenceCandidates.Add(i);
            }
            else
            {
                influence = Random.Range(0, 2);
            }

            characterStats[i].stats[CharacterStat.Influence] = influence;
            characterStats[i].stats[CharacterStat.Perception] = perception;
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
