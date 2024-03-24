using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    //public string name; // *** here I might set up a condition dictionary

    [TextArea(3, 10)]
    public string[] sentences;
    public string responseNo;
    public string responseYes;
}