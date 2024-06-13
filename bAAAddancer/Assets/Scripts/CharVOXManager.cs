using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharVOXManager : MonoBehaviour
{
    /// <summary>
    /// Will use this script to organise and manage voice clips for each character
    /// </summary>
    [System.Serializable]
    public class VoxUnit 
    {
        public DialogueData.SpeakingCharacter speakingCharacter;
        public VoxExpression expression;
        public AudioClip voxClip;
    }
    public enum VoxExpression { Casual, Excited, Worried, Shady, Mad }

    public VoxUnit[] voxUnits;
}
