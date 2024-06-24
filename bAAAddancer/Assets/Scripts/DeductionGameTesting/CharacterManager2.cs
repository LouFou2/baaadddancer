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
        public int isPlayer = 0;
        public int isDemon = 0;
        public int cursed = 0;
        public int influence = 0;
        public int perception = 0;
    }
    private void Start()
    {
        SetPlayerIndex();
        SetDemonIndex();

        // Lists to track characters with good perception and good influence
        List<int> goodPerceptionCandidates = new List<int>();
        List<int> goodInfluenceCandidates = new List<int>();

        // Update isPlayer and isDemon flags and assign influence and perception stats to each character
        for (int i = 0; i < characters.Length; i++)
        {
            // Update isPlayer and isDemon flags
            if (i == playerIndex)
            {
                characterStats[i].isPlayer = 1;
            }
            if (i == demonIndex)
            {
                characterStats[i].isDemon = 1;
            }

            // Assign influence and perception stats to each character
            // 0 bad, 1 neutral, 2 good
            // Can't have good perception as well as good influence
            int perception = 0;
            int influence = 0;

            // Assign perception stat
            if (goodPerceptionCandidates.Count < 2)
            {
                // Ensure there are exactly two characters with good perception
                perception = 2;
                goodPerceptionCandidates.Add(i);
            }
            else
            {
                perception = Random.Range(0, 2); // Random perception value (0, 1, or 2)
            }

            // Assign influence stat
            if (goodInfluenceCandidates.Count < 2 && perception != 2)
            {
                // Ensure there are exactly two characters with good influence and not good perception
                influence = 2;
                goodInfluenceCandidates.Add(i);
            }
            else
            {
                influence = Random.Range(0, 2); // Random influence value (0, 1, or 2)
            }

            // Update character stats
            characterStats[i].influence = influence;
            characterStats[i].perception = perception;
        }
    }

    private void SetPlayerIndex() 
    {
        playerIndex = Random.Range(0, characters.Length);
    }
    private void SetDemonIndex() 
    {
        // choose a random index for demon and make sure it is not the same as player index
        do
        {
            demonIndex = Random.Range(0, characters.Length);
        } while (demonIndex == playerIndex);
    }
}
