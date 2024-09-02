using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsSO", menuName = "CharacterStats/CharacterStatsSO")]
public class CharacterStatsSO : ScriptableObject
{
    public int IsPlayerInt = 0;
    public int IsDemonInt = 0;
    public int CursedInt = 0;
    public int LastCursedInt = 0;
    public int InfluenceInt = 0;
    public int PerceptionInt = 0;

    public Dictionary<CharacterStat, int> stats = new Dictionary<CharacterStat, int>
    {
        { CharacterStat.IsPlayer, 0 }, // player and demon: 0 false, 1 true
        { CharacterStat.IsDemon, 0 },
        { CharacterStat.Cursed, 0 }, // cursed: t.b.d useful index range... 0-15? (4 for each round?)
        { CharacterStat.LastCursed, 0},

        { CharacterStat.Influence, 0 }, // influence and perception: 0 bad, 1 neutral, 2 good
        { CharacterStat.Perception, 0 },

        { CharacterStat.SpokenAmount, 0},
        { CharacterStat.LastSpeaker, 0},
        { CharacterStat.LastSpokenTo, 0},

        { CharacterStat.SpeakToGroup, 0},
        { CharacterStat.SpeakToCamera, 0},
    };
}
