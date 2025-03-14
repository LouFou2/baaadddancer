using UnityEngine;

[CreateAssetMenu(fileName = "NewRecordingData", menuName = "RecordingData")]
public class RecordingData : ScriptableObject
{
    public Vector3[] initialPositions = new Vector3[64]; // array to store sequence of recorded positions of moving object
    public Quaternion[] initialRotations = new Quaternion[64];
    public Vector3[] recordedPositions = new Vector3[64]; // array to store sequence of recorded positions of moving object
    public Quaternion[] recordedRotations = new Quaternion[64];
}