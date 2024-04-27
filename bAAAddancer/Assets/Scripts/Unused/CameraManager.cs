using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform cam_FrontML;
    [SerializeField] private Transform cam_NPC_M;
    [SerializeField] private Transform cam_Player_M;
    [SerializeField] private Transform cam_NPC_C;
    [SerializeField] private Transform cam_Player_C;

    [SerializeField] private GameObject[] characters;
    private GameObject currentSpeakingCharacter;
    
    public void MoveCam_FrontML(float camMoveSpeed) 
    {
        //This is a medium-long view to show all characters in scene
        Camera.main.transform.DOMove(cam_FrontML.position, camMoveSpeed);
        Camera.main.transform.DORotateQuaternion(cam_FrontML.rotation, camMoveSpeed);

    }
    public void MoveCam_NPC_M(GameObject speakingCharacter, float camMoveSpeed)
    {
        // logic to move camera into a medium-shot to speaking NPC
        // *** might need some finagling to make this work for multiple NPCs (with different positions + rotations)

        // get NPC transform (*** use prefab root object transform?)
        Transform speakerTransform = speakingCharacter.transform;

        // set up offset position and rotation for camera

        // aim Camera at currently speaking NPC
        Camera.main.transform.DOMove(cam_NPC_M.position, camMoveSpeed);
        Camera.main.transform.DORotateQuaternion(cam_NPC_M.rotation, camMoveSpeed);
    }
    public void MoveCam_Player_M(GameObject speakingCharacter, float camMoveSpeed)
    {
        // logic to move camera into a medium-shot to speaking Player
        Transform speakerTransform = speakingCharacter.transform;

        // set up offset position and rotation for camera

        Camera.main.transform.DOMove(cam_Player_M.position, camMoveSpeed);
        Camera.main.transform.DORotateQuaternion(cam_Player_M.rotation, camMoveSpeed);
    }
    public void MoveCam_NPC_C(GameObject speakingCharacter, float camMoveSpeed)
    {
        // logic to move camera into a Close-Up shot to speaking NPC
        Transform speakerTransform = speakingCharacter.transform;

        // set up offset position and rotation for camera

        Camera.main.transform.DOMove(cam_NPC_C.position, camMoveSpeed);
        Camera.main.transform.DORotateQuaternion(cam_NPC_C.rotation, camMoveSpeed);
    }
    public void MoveCam_Player_C(GameObject speakingCharacter, float camMoveSpeed)
    {
        // logic to move camera into a Close-Up shot to speaking Player
        Transform speakerTransform = speakingCharacter.transform;

        // set up offset position and rotation for camera

        Camera.main.transform.DOMove(cam_Player_C.position, camMoveSpeed);
        Camera.main.transform.DORotateQuaternion(cam_Player_C.rotation, camMoveSpeed);
    }
}
