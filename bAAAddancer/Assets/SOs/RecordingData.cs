using UnityEngine;

[CreateAssetMenu(fileName = "NewRecordingData", menuName = "RecordingData")]
public class RecordingData : ScriptableObject
{
    public Vector3[] recordedPositions = new Vector3[16]; // array to store sequence of recorded positions of moving object
}