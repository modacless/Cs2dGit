using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerCameraBehaviour : NetworkBehaviour
{
    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    private Camera playerCamera;
    // Update is called once per frame
    [HideInInspector]
    public  Vector3 actualRotation = new Vector3(0,0,0);
    public void Start()
    {
        playerData.rotationCameraTransform = transform;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if(IsOwner)
            playerCamera.transform.rotation = Quaternion.Euler(actualRotation);
    }

    void Update()
    {
        RotateCamera();
        playerCamera.transform.rotation = Quaternion.Euler(actualRotation);
        playerData.rotationCamera = actualRotation;
        playerData.rotationCameraTransform = transform;

    }

    private void RotateCamera()
    {
        if (Input.GetKey(playerData.rotateRCamera))
        {
            actualRotation.z += RotateInSeconds(playerData.rotateCameraSpeed);
        }

        if (Input.GetKey(playerData.rotateLCamera))
        {
            actualRotation.z -= RotateInSeconds(playerData.rotateCameraSpeed);
        }
    }

    private float RotateInSeconds(float goal)
    {
        return goal * Time.fixedDeltaTime;
    }


}
