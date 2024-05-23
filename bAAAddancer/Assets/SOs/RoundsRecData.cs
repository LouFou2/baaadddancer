using UnityEngine;

[CreateAssetMenu(fileName = "NewRoundsRecordingData", menuName = "RoundsRecData")]
public class RoundsRecData : ScriptableObject
{
    public RecordingData[] recordingDataOfRounds = new RecordingData[4];
    public RecordingData currentRoundRecData;
}
