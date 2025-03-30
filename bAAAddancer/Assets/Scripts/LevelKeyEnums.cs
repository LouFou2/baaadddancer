using UnityEngine;

[System.Serializable]
public enum LevelKey
{
    TitleSequence,

    IntroDialogue,

    IntroMakeDance,
    IntroCopyDance,     // ***Changes: stripping gameloop down to the makeDance + Copy Dance Scenes
    /*IntroMeeting,
    IntroDebug,

    Round2Dialogue,*/
    Round2MakeDance,
    Round2CopyDance,
    /*Round2Meeting,
    Round2Debug,

    Round3Dialogue,*/
    Round3MakeDance,
    Round3CopyDance,
    /*Round3Meeting,
    Round3Debug,

    Round4Dialogue,*/
    Round4MakeDance,
    Round4CopyDance,
    /*Round4Meeting,
    Round4Vote,*/

    RaveScene,          // *** and game culminates with Rave Scene
/*
    EliminationScene,*/
}
