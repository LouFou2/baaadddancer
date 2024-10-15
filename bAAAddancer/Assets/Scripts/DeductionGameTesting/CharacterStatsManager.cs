using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    public CharacterStatsSO[] characterStats;
    [SerializeField] SceneSwitcher sceneSwitcher; // assign in inspector

    public int playerIndex = -1;
    public int demonIndex = -1;
    public int lastSpeakerIndex = -1;
    
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
            // should find a way to update the Deception between scenes
            // should find a way to update the Eliminator between scenes
            // should find a way to update the VoteTarget between scenes

            if (sceneSwitcher != null)
            {
                if (sceneSwitcher.GetCurrentSceneName() != "VoteScene2") // need the vote targets to persist for vote scene
                {
                    characterStats[i].VoteTargetInt = -1; // but otherwise we reset it
                }
            }
            else
            {
                Debug.LogWarning("Please assign scene switcher in inspector");
            }

            characterStats[i].DeceptionInt = 0; 
            characterStats[i].EliminatorInt = 0;
            characterStats[i].EliminationVoteCountsInt = 0;

            statsDictionaries[i] = new Dictionary<CharacterStat, int>
            {
                { CharacterStat.IsPlayer, characterStats[i].IsPlayerInt },
                { CharacterStat.IsDemon, characterStats[i].IsDemonInt },
                { CharacterStat.Cursed, characterStats[i].CursedInt },
                { CharacterStat.LastCursed, characterStats[i].LastCursedInt },
                { CharacterStat.Influence, characterStats[i].InfluenceInt },
                { CharacterStat.Perception, characterStats[i].PerceptionInt },

                { CharacterStat.Deception, characterStats[i].DeceptionInt },
                { CharacterStat.Eliminator, characterStats[i].EliminatorInt },
                { CharacterStat.VoteTarget, characterStats[i].VoteTargetInt },
                { CharacterStat.EliminationVoteCounts, characterStats[i].EliminationVoteCountsInt },
                { CharacterStat.Eliminated, characterStats[i].EliminatedInt },

                { CharacterStat.SideGud, characterStats[i].SideGudInt },
                { CharacterStat.SideCurse, characterStats[i].SideCursedInt },

                { CharacterStat.NpcIndex, characterStats[i].NpcIndexInt },

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
            statsDictionaries[i][CharacterStat.Eliminator] = 0;
            characterStats[i].EliminatorInt = 0;
            statsDictionaries[i][CharacterStat.VoteTarget] = -1;
            characterStats[i].VoteTargetInt = -1;
            statsDictionaries[i][CharacterStat.EliminationVoteCounts] = 0;
            characterStats[i].EliminationVoteCountsInt = 0;
            statsDictionaries[i][CharacterStat.Eliminated] = 0;
            characterStats[i].EliminatedInt = 0;
            statsDictionaries[i][CharacterStat.SideGud] = 0;
            characterStats[i].SideGudInt = 0;
            statsDictionaries[i][CharacterStat.SideCurse] = 0;
            characterStats[i].SideCursedInt = 0;
            statsDictionaries[i][CharacterStat.NpcIndex] = -1;
            characterStats[i].NpcIndexInt = -1;

            // influence and perception is handled below
        }

        // Assign Indices for NPCs (not player or Demon)
        int NpcIndexer = -1;
        for (int i = 0; i < characterStats.Length; i++)
        {
            if (i != playerIndex && i != demonIndex)
            {
                NpcIndexer += 1;
                statsDictionaries[i][CharacterStat.NpcIndex] = NpcIndexer;
                characterStats[i].NpcIndexInt = NpcIndexer;
            }
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

    private void UpdateCharacterStats()
    {
        for (int i = 0; i < characterStats.Length; i++)
        {
            // first we set eliminated stat
            if (characterManager.characterDataSOs[i].wasEliminated)
            {
                characterStats[i].EliminatedInt = 1;
                ModifyCharacterStat(i, CharacterStat.Eliminated, 1);
            }

            //== The Curse Stats gets handled differently: ==
            // (because cursing happens between dialogue scenes, and gets stored in characterDataSOs)

            // --The last cursed character:
            characterStats[i].LastCursedInt = 0;
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

                if (characterManager.characterDataSOs[i].infectionLevel > 0.1)
                {
                    characterStats[i].SideCursedInt = 1;
                    ModifyCharacterStat(i, CharacterStat.SideCurse, 1);
                }
                else
                {
                    DetermineAlignment(i); // this method will use char's influence to determine if it aligns with the norm, or accepts curse
                }
            }
            else
            {
                characterStats[i].CursedInt = 0;
                ModifyCharacterStat(i, CharacterStat.Cursed, 0);
            }
                
        }
    }
    public void DetermineAlignment(int charIndex)
    {
        int charInfluence = GetCharacterStats(charIndex)[CharacterStat.Influence];
        bool groupAlignedGud = true;
        bool influenceCursed = false;
        int sideGudMembers = 0;
        int sideCursedMembers = 0;
        for (int i = 0; i < statsDictionaries.Length; i++)
        {
            if (statsDictionaries[i][CharacterStat.SideCurse] == 1)
            {
                sideCursedMembers += 1;
                //we can also check if this char has high influence
                if (statsDictionaries[i][CharacterStat.Influence] == 2)
                {
                    influenceCursed = true; // if at least one high influence char is side curse
                }
            }
            if (statsDictionaries[i][CharacterStat.SideGud] == 1)
            {
                sideGudMembers += 1;
            }
        }
        if (sideCursedMembers > sideGudMembers)
        {
            groupAlignedGud = false;
        }
        if (charInfluence == 2) // if the char has high influence, it will be proud of curse
        {
            characterStats[charIndex].SideCursedInt = 1;
            ModifyCharacterStat(charIndex, CharacterStat.SideCurse, 1);
        }
        else if (charInfluence == 1 && !groupAlignedGud) // the medium influence char goes with group alignment
        {
            characterStats[charIndex].SideCursedInt = 1;
            ModifyCharacterStat(charIndex, CharacterStat.SideCurse, 1);
        }
        else if (charInfluence == 0 && !groupAlignedGud && influenceCursed) // the low influence is very insecure
        {
            characterStats[charIndex].SideCursedInt = 1;
            ModifyCharacterStat(charIndex, CharacterStat.SideCurse, 1);
        }
        else
        {
            characterStats[charIndex].SideCursedInt = 0;
            ModifyCharacterStat(charIndex, CharacterStat.SideCurse, 0); // so there is a chance a cursed character sides with "git gud" chars
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
            if (statsDictionaries[i][CharacterStat.Eliminated] == 1)
            {
                continue;
            }
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
    public bool IsPlayerCriteriaMatch(Dictionary<CharacterStat, int> queryCriteria) 
    {
        //Debug.Log("Checking character index: " + i);
        foreach (var criterion in queryCriteria)
        {
            //Debug.Log("Checking criterion: " + criterion.Key + " with value: " + criterion.Value);
            if (!statsDictionaries[playerIndex].ContainsKey(criterion.Key) || statsDictionaries[playerIndex][criterion.Key] != criterion.Value)
            {
                return false;
                //Debug.Log("Criterion does not match for character index: " + i);
            }
        }
        return true;
    }

    public void HandleSpeakerHasLied()
    {
        ModifyCharacterStat(lastSpeakerIndex, CharacterStat.Deception, 1);
        characterStats[lastSpeakerIndex].DeceptionInt = 1;

        // they potentially also become a VoteTarget:
        for (int i = 0; i < characterStats.Length; i++) 
        {
            if (GetCharacterStats(i)[CharacterStat.LastSpeaker] == 0
                && GetCharacterStats(i)[CharacterStat.IsPlayer] == 0
                && GetCharacterStats(i)[CharacterStat.IsDemon] == 0) // (the demon gets handled differently)
            {
                HandleSuspicion(i);
            }
        }
    }
    public void HandleSuspicion(int suspiciousChar) // if speaker becomes suspect, they become a vote target
    {
        float suspicionChance = Random.Range(0f, 1f);
        //Perception level will determine the chance of suspicion
        if (GetCharacterStats(suspiciousChar)[CharacterStat.Perception] == 2 && suspicionChance < 0.9f) //high chance
        {
            ModifyCharacterStat(suspiciousChar, CharacterStat.VoteTarget, lastSpeakerIndex); // speaker becomes vote target
            characterStats[suspiciousChar].VoteTargetInt = lastSpeakerIndex;

            ModifyCharacterStat(suspiciousChar, CharacterStat.Eliminator, 1); // suspicious Char also becomes an eliminator
            characterStats[suspiciousChar].EliminatorInt = 1;
        }
        if (GetCharacterStats(suspiciousChar)[CharacterStat.Perception] == 1 && suspicionChance < 0.6f) //lower chance
        {
            ModifyCharacterStat(suspiciousChar, CharacterStat.VoteTarget, lastSpeakerIndex);
            characterStats[suspiciousChar].VoteTargetInt = lastSpeakerIndex;

            ModifyCharacterStat(suspiciousChar, CharacterStat.Eliminator, 1); // suspicious Char also becomes an eliminator
            characterStats[suspiciousChar].EliminatorInt = 1;
        }
        if (GetCharacterStats(suspiciousChar)[CharacterStat.Perception] == 0 && suspicionChance < 0.3f) //lowest chance
        {
            ModifyCharacterStat(suspiciousChar, CharacterStat.VoteTarget, lastSpeakerIndex);
            characterStats[suspiciousChar].VoteTargetInt = lastSpeakerIndex;

            ModifyCharacterStat(suspiciousChar, CharacterStat.Eliminator, 1); // suspicious Char also becomes an eliminator
            characterStats[suspiciousChar].EliminatorInt = 1;
        }
    }

    public void HandleCurseVsGudTargeting() // a method that sets vote targets depending on Gud vs Cursed Alignment
    {
        //first check if char is Team Gud or Team Cursed
        for (int i = 0; i < characterStats.Length; i++)
        {
            if (characterStats[i].SideGudInt == 1 && characterStats[i].InfluenceInt == 2) // ===== if Team Gud and high influence
            {
                // we look for most cursed character
                int mostCursedIndex = -1;
                float mostCursedAmount = 0;
                for (int j = 0; j < characterManager.characterDataSOs.Length; j++)
                {
                    if (characterManager.characterDataSOs[j].wasEliminated) continue;

                    if (characterManager.characterDataSOs[j].infectionLevel > mostCursedAmount)
                    {
                        mostCursedAmount = characterManager.characterDataSOs[j].infectionLevel;
                        mostCursedIndex = j;
                    }
                }
                // then the most cursed character becomes the vote target
                characterStats[i].VoteTargetInt = mostCursedIndex;
                ModifyCharacterStat(i, CharacterStat.VoteTarget, mostCursedIndex);
            }
            if (characterStats[i].SideCursedInt == 1) // ========================================= if Team Cursed
            {
                // we find characters that are team gud + Eliminator + not cursed
                for (int targetIndex = 0; targetIndex < characterStats.Length; targetIndex++)
                {
                    if (characterManager.characterDataSOs[targetIndex].wasEliminated) continue;

                    if (characterStats[targetIndex].SideGudInt == 1 && characterStats[targetIndex].EliminatorInt == 1 && characterStats[targetIndex].CursedInt == 0)
                    {
                        characterStats[i].VoteTargetInt = targetIndex;
                        break;
                    }
                }
            }
        }
    }

    public void HandleSpeakerWantsElimination() 
    {
        int speakerIndex = -1;
        for (int i = 0; i < statsDictionaries.Length; i++) 
        {
            speakerIndex = (GetCharacterStats(i)[CharacterStat.LastSpeaker] == 1) ? i : speakerIndex;
        }
        ModifyCharacterStat(speakerIndex, CharacterStat.Eliminator, 1);
        characterStats[speakerIndex].EliminatorInt = 1;
    }
    public void HandleSpeakerBlocksElimination()
    {
        int speakerIndex = -1;
        for (int i = 0; i < statsDictionaries.Length; i++)
        {
            speakerIndex = (GetCharacterStats(i)[CharacterStat.LastSpeaker] == 1) ? i : speakerIndex;
        }
        ModifyCharacterStat(speakerIndex, CharacterStat.Eliminator, 0);
        characterStats[speakerIndex].EliminatorInt = 0;
    }

    public bool ShouldEliminationHappen() 
    {
        int compliantChars = 0; // We will see who agrees to eliminate *
        int totalEliminators = 0;

        foreach (CharacterStatsSO charStat in characterStats) 
        {
            if (charStat.EliminatorInt == 1)
            {
                totalEliminators += 1;
            }
            if (charStat.EliminatorInt == 1 && charStat.InfluenceInt == 2) 
            {
                //if they want to eliminate and have high influence, we give them a compliant follower *
                compliantChars += 1;
            }
        }

        // we update any compliant characters' stats to "Eliminator". They can be compliant if they have low influence.
        for(int i = 0; i < statsDictionaries.Length; i++)  //(CharacterStatsSO charStat in characterStats)
        {
            if (compliantChars > 0 && 
                characterStats[i].EliminatorInt == 0 && 
                characterStats[i].InfluenceInt == 0) 
            {
                ModifyCharacterStat(i, CharacterStat.Eliminator, 1);
                characterStats[i].EliminatorInt = 1;

                totalEliminators += 1;

                compliantChars -= 1;
            }
        }

        if (totalEliminators >= 4) // so that is the majority (4 out of 6)
        {
            return true;
        }

        return false;
    }

    public void HandleSpeakerSidesGud()
    {
        int speakerIndex = -1;
        for (int i = 0; i < statsDictionaries.Length; i++)
        {
            speakerIndex = (GetCharacterStats(i)[CharacterStat.LastSpeaker] == 1) ? i : speakerIndex;
        }
        ModifyCharacterStat(speakerIndex, CharacterStat.SideGud, 1);
        characterStats[speakerIndex].SideGudInt = 1;
        ModifyCharacterStat(speakerIndex, CharacterStat.SideCurse, 0);
        characterStats[speakerIndex].SideCursedInt = 0;
    }
    public void HandleSpeakerSidesCursed()
    {
        int speakerIndex = -1;
        for (int i = 0; i < statsDictionaries.Length; i++)
        {
            speakerIndex = (GetCharacterStats(i)[CharacterStat.LastSpeaker] == 1) ? i : speakerIndex;
        }
        ModifyCharacterStat(speakerIndex, CharacterStat.SideGud, 0);
        characterStats[speakerIndex].SideGudInt = 0;
        ModifyCharacterStat(speakerIndex, CharacterStat.SideCurse, 1);
        characterStats[speakerIndex].SideCursedInt = 1;
    }
}
