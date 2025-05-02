using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

// THIS SCRIPT IS FOR DIALOGUE SCENE and MEETING SCENE *** CLEAN UP A LOT OF THIS FOR NEW GAME OVERHAUL
public class CameraManager : MonoBehaviour
{
    //[SerializeField] private DialogueSwitcher dialogueSwitcher;
    [SerializeField] private Animator cameraAnimator; // script should be attached to GameObject with Camera Animator Controller
    [SerializeField] private CharacterManager characterManager;

    [SerializeField] private string[] cameraFlags; // the camera flags are the bool names in the Animator

    [SerializeField] private Cinemachine.CinemachineVirtualCamera[] virtualCameras;
    [SerializeField] private GameObject[] cameraAimTargets; // assign each camera's aim target (example head bone) in inspector
    [SerializeField] private GameObject playerCameraTransforms;
    [SerializeField] private GameObject[] npcCameraTransforms; // assign each camera's position object (example "PlayerCamPos")in inspector


    void Start()
    {
        // Set each virtual camera's position
        int npcIndex = 0;

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            GameObject character = characterManager.characters[i];

            CharacterProfile characterProfile = character.GetComponent<CharacterProfile>();

            if (characterProfile.characterDataSO.characterRoleSelect == CharacterData.CharacterRole.Player)
            {
                virtualCameras[i].transform.position = playerCameraTransforms.transform.position;
                virtualCameras[i].transform.rotation = playerCameraTransforms.transform.rotation;
            }
            else
            {
                virtualCameras[i].transform.position = npcCameraTransforms[npcIndex].transform.position;
                virtualCameras[i].transform.rotation = npcCameraTransforms[npcIndex].transform.rotation;

                npcIndex++;
            }
        }

        // Assign each virtual camera's LookAt target
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (i < cameraAimTargets.Length && virtualCameras[i] != null && cameraAimTargets[i] != null)
            {
                virtualCameras[i].LookAt = cameraAimTargets[i].transform;
            }
            else
            {
                Debug.LogWarning($"Missing camera or target at index {i}");
            }
        }

        cameraFlags = new string[6];

        cameraFlags[0] = "CharCam0"; // the caracters these cameras are aimed at are arranged in same order as in Character Manager
        cameraFlags[1] = "CharCam1"; // which is why we need logic below to figure out which indexes are the player, last bugged character, etc
        cameraFlags[2] = "CharCam2"; // (for the sake of the dialogue moments)
        cameraFlags[3] = "CharCam3";
        cameraFlags[4] = "CharCam4";
        cameraFlags[5] = "CharCam5";

    }

    public void SetCamera(int camIndex)
    {
        foreach (string camFlag in cameraFlags)
        {
            cameraAnimator.SetBool(camFlag, false);
        }
        cameraAnimator.SetBool(cameraFlags[camIndex], true);
    }

}
