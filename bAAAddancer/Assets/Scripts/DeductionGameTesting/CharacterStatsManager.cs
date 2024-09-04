using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    // probably need to plug into the Character Manager
    [SerializeField] private CharacterManager characterManager;
    public CharacterStatsSO[] characterStats;

    public int playerIndex = -1;
    public int demonIndex = -1;
    
    private Dictionary<CharacterStat, int>[] statsDictionaries;

    private void Awake()
    {
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        statsDictionaries = new Dictionary<CharacterStat, int>[characterStats.Length];
        for (int i = 0; i < characterStats.Length; i++)
        {
            statsDictionaries[i] = new Dictionary<CharacterStat, int>
            {
                { CharacterStat.IsPlayer, characterStats[i].IsPlayerInt },
                { CharacterStat.IsDemon, characterStats[i].IsDemonInt },
                { CharacterStat.Cursed, characterStats[i].CursedInt },
                { CharacterStat.LastCursed, characterStats[i].LastCursedInt },
                { CharacterStat.Influence, characterStats[i].InfluenceInt },
                { CharacterStat.Perception, characterStats[i].PerceptionInt },
                { CharacterStat.Deception, characterStats[i].DeceptionInt },

                // Every scene these can start with defaults:
                { CharacterStat.SpokenAmount, 0 },
                { CharacterStat.LastSpeaker, -1 },
                { CharacterStat.LastSpokenTo, -1 },
                { CharacterStat.SpeakToGroup, 0 },
                { CharacterStat.SpeakToCamera, 0 },
            };
        }
    }

    private void Start()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        SetPlayerIndex();
        SetDemonIndex();

        // We set up the stats in the first dialogue scene
        // (After player has been selected and Demon has been assigned)
        if (GameManager.Instance.GetCurrentLevelKey() == LevelKey.IntroDialogue)
        {
            AssignCharacterStats();
        }
        else
        { 
            UpdateCharacterStats();
        }
    }
    private void UpdateCharacterStats() 
    {
        for (int i = 0; i < characterStats.Length; i++) 
        {
            //== The Curse Stats gets handled differently: ==
            // (because cursing happens between dialogue scenes, and gets stored in characterDataSOs)

            // --The last cursed character:
            ModifyCharacterStat(i, CharacterStat.LastCursed, 0); // reset all 
            if (characterManager.characterDataSOs[i].lastCursedCharacter) 
            {
                ModifyCharacterStat(i, CharacterStat.LastCursed, 1);
                characterStats[i].LastCursedInt = 1; //*** just for the sake of debugging (cant see dictionary value in inspector)
            }
            // --Cursed Amount:
            if (characterManager.characterDataSOs[i].infectionLevel > 0) 
            {
                characterStats[i].CursedInt = 1;
                ModifyCharacterStat(i, CharacterStat.Cursed, 1);
            }    
            else
                ModifyCharacterStat(i, CharacterStat.Cursed, 0);
        }
    }
    public void StoreCharacterStats()  //***PROBABLY DONT NEED THIS
    {
        for (int i = 0; i < characterStats.Length; i++) 
        {
            // Debug logs to check the values in the dictionary before storing them in the SO fields
            Debug.Log($"Storing Character Stats for index {i}:");
            Debug.Log($"IsPlayer: {statsDictionaries[i][CharacterStat.IsPlayer]}");
            Debug.Log($"IsDemon: {statsDictionaries[i][CharacterStat.IsDemon]}");
            Debug.Log($"Cursed: {statsDictionaries[i][CharacterStat.Cursed]}");
            Debug.Log($"Influence: {statsDictionaries[i][CharacterStat.Influence]}");
            Debug.Log($"Perception: {statsDictionaries[i][CharacterStat.Perception]}");

            characterStats[i].IsPlayerInt = statsDictionaries[i][CharacterStat.IsPlayer];
            characterStats[i].IsDemonInt = statsDictionaries[i][CharacterStat.IsDemon];
            characterStats[i].CursedInt = statsDictionaries[i][CharacterStat.Cursed];
            // LastCursed is handled different, as it happens between dialogue scenes
            characterStats[i].InfluenceInt = statsDictionaries[i][CharacterStat.Influence];
            characterStats[i].PerceptionInt = statsDictionaries[i][CharacterStat.Perception];

            // Debug logs to confirm the values were stored correctly
            Debug.Log($"Stored IsPlayerInt: {characterStats[i].IsPlayerInt}");
            Debug.Log($"Stored IsDemonInt: {characterStats[i].IsDemonInt}");
            Debug.Log($"Stored CursedInt: {characterStats[i].CursedInt}");
            Debug.Log($"Stored InfluenceInt: {characterStats[i].InfluenceInt}");
            Debug.Log($"Stored PerceptionInt: {characterStats[i].PerceptionInt}");
        }
    }

    private void SetPlayerIndex()
    {
        playerIndex = characterManager.playerIndex;
    }

    private void SetDemonIndex()
    {
        demonIndex = characterManager.demonIndex;
    }

    private void AssignCharacterStats()
    {
        // first lets reset some stats:
        for (int i = 0; i < characterStats.Length; i++) 
        {
            statsDictionaries[i][CharacterStat.IsPlayer] = 0;
            characterStats[i].IsPlayerInt = 0;
            statsDictionaries[i][CharacterStat.IsDemon] = 0;
            characterStats[i].IsDemonInt = 0;
            statsDictionaries[i][CharacterStat.Cursed] = 0;
            characterStats[i].CursedInt = 0;
            statsDictionaries[i][CharacterStat.LastCursed] = 0;
            characterStats[i].LastCursedInt = 0;
            statsDictionaries[i][CharacterStat.Deception] = 0;
            characterStats[i].DeceptionInt = 0;
            // influence and perception is handled below
        }

        // == Now Assign the basic stats == 

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
                statsDictionaries[index][CharacterStat.IsPlayer] = 1;
                //Debug.Log($"Character {index}: IsPlayer assigned to {characterStats[index].stats[CharacterStat.IsPlayer]}");
                characterStats[index].IsPlayerInt = 1;
            }
            if (index == demonIndex)
            {
                statsDictionaries[index][CharacterStat.IsDemon] = 1;
                //Debug.Log($"Character {index}: IsDemon assigned to {characterStats[index].stats[CharacterStat.IsDemon]}");
                characterStats[index].IsDemonInt = 1;
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

            statsDictionaries[index][CharacterStat.Influence] = influence;
            //Debug.Log($"Character {index}: Influence assigned to {characterStats[index].stats[CharacterStat.Influence]}");
            characterStats[index].InfluenceInt = influence; // just so I can see in inspector

            statsDictionaries[index][CharacterStat.Perception] = perception;
            //Debug.Log($"Character {index}: Perception assigned to {characterStats[index].stats[CharacterStat.Perception]}");
            characterStats[index].PerceptionInt = perception;
        }
    }

    public Dictionary<CharacterStat, int> GetCharacterStats(int index)
    {
        if (index >= 0 && index < characterStats.Length)
        {
            return statsDictionaries[index];
        }
        return null;
    }
    public void ModifyCharacterStat(int characterIndex, CharacterStat characterStat, int newValue)
    {
        if (characterIndex >= 0 && characterIndex < characterStats.Length)
        {
            if (statsDictionaries[characterIndex].ContainsKey(characterStat))
            {
                statsDictionaries[characterIndex][characterStat] = newValue;

                //Debug.Log($"Character {characterIndex}: {characterStat} updated to {characterStats[characterIndex].stats[characterStat]}");
            }
            else
                Debug.LogWarning("no valid stat " + characterStat + " to modify");
        }
    }
    public int GetFirstMatchingIndex(Dictionary<CharacterStat, int> queryCriteria)
    {
        for (int i = 0; i < statsDictionaries.Length; i++)
        {
            bool match = true;
            //Debug.Log("Checking character index: " + i);

            foreach (var criterion in queryCriteria)
            {
                //Debug.Log("Checking criterion: " + criterion.Key + " with value: " + criterion.Value);
                if (!statsDictionaries[i].ContainsKey(criterion.Key) || statsDictionaries[i][criterion.Key] != criterion.Value)
                {
                    match = false;
                    //Debug.Log("Criterion does not match for character index: " + i);
                    break;
                }
            }

            if (match)
            {
                //Debug.Log("Found match: " + i);
                return i; // Return the first matching index
            }
        }

        return -1; // Return -1 if no match is found
    }

    public List<int> GetMatchingCharacterIndices(Dictionary<CharacterStat, int> queryCriteria)
    {
        List<int> matchingIndices = new List<int>();

        for (int i = 0; i < statsDictionaries.Length; i++)
        {
            bool match = true;
            //Debug.Log("Checking character index: " + i);
            foreach (var criterion in queryCriteria)
            {
                //Debug.Log("Checking criterion: " + criterion.Key + " with value: " + criterion.Value);
                if (!statsDictionaries[i].ContainsKey(criterion.Key) || statsDictionaries[i][criterion.Key] != criterion.Value)
                {
                    match = false;
                    //Debug.Log("Criterion does not match for character index: " + i);
                    break;
                }
            }
            if (match)
            {
                matchingIndices.Add(i);
                //Debug.Log("Found match: " + i);
            }
        }

        return matchingIndices;
    }

    public void HandleSpeakerHasLied()
    {
        for (int i = 0; i < characterStats.Length; i++) 
        {
            if (GetCharacterStats(i)[CharacterStat.LastSpeaker] == 1) 
            {
                int deceptionValue = GetCharacterStats(i)[CharacterStat.Deception];
                ModifyCharacterStat(i, CharacterStat.Deception, deceptionValue + 1);
                characterStats[i].DeceptionInt += 1;
            }
        }
    }
}
