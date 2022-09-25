using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public enum CameraState
{
    Lock,
    Unlock,
    Dead
}

public class PlayerCamera : NetworkBehaviour
{

    //Player data
    [Header("References")]
    [SerializeField]
    private ScriptablePlayerData playerData;

    //Camera initialisation
    private Camera playerCamera;
    private Vector3 _initCamPos;

    //Camera state (to change pos ect..)
    [HideInInspector]
    public CameraState cameraState;

    //Camera Position
    Vector3 startLockPosition;

    //Camera unlock boolean
    private bool canUnlock = true;
    private float unlockTime;

    //Camera Data
    [Header("Camera movement")]
    [SerializeField]
    [Tooltip("Curve movement when camera is stable")]
    private AnimationCurve cameraCurveMovement;
    [SerializeField]
    [Tooltip("Curve movement that show stabilize Unlock movement")]
    private AnimationCurve cameraUnlockStabilizeCurveMovement;
    [SerializeField]
    [Tooltip("Curve movement that show stabilize lock movement")]
    private AnimationCurve cameraLockStabilizeCurveMovement;


    [SerializeField]
    [Min(0)]
    [Tooltip("multiply force to adjust distance of camera moving")]
    private float forceDistance;

    [SerializeField]
    [Min(1)]
    [Tooltip("Distance btw player and transform camera")]
    private float maxDistance;

    [SerializeField]
    [Tooltip("Time before camera stabilize (in seconds)")]
    private float speedFactorToUnlock; //In seconds

    [SerializeField]
    [Tooltip("Time before camera stabilize (in seconds)")]
    private float speedFactorToLock;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            Weapon.staticShoot += CameraShake;
            playerCamera = Camera.main;
            _initCamPos = playerCamera.transform.position;
            cameraState = CameraState.Lock;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(playerData.lockUnlockCamera) && canUnlock)
            {
                if (cameraState == CameraState.Lock)
                {
                    cameraState = CameraState.Unlock;
                    return;
                }

                if (cameraState == CameraState.Unlock)
                {
                    CameraLock();
                    cameraState = CameraState.Lock;
                    return;
                }
            }
        }

        if(cameraState == CameraState.Unlock)
        {
            CameraUnlock();
        }


    }

    private void CameraShake(float magnitude)
    {
        StartCoroutine(CameraShakeManager(magnitude*0.01f));
    }

    private IEnumerator CameraShakeManager(float magnitude)
    {
        
        float t = 0f, x, y;
        while (t < 0.35f)
        {
            x = Random.Range(-1f, 1f) * magnitude;
            y = Random.Range(-1f, 1f) * magnitude;

            playerCamera.transform.position += new Vector3(x, y, 0);
            t += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.localPosition = new Vector3(0,0, _initCamPos.z);
    }


    //Do Every update time when unlockState
    private void CameraUnlock()
    {
        float distanceCameraPlayer = (playerCamera.ScreenToWorldPoint(Input.mousePosition) - playerCamera.transform.position).magnitude;  //Distance btw mouse and player object
        float distanceCamera = (playerCamera.transform.position - transform.position - new Vector3(0,0,-10)).magnitude; //Distance btw camera object and player object
        if (distanceCamera < maxDistance)
        {
            float normalizeDistance = ((cameraCurveMovement.length-1) / maxDistance)* distanceCameraPlayer; //Normalize distance btw mouse and player
            float movementSpeed = cameraCurveMovement.Evaluate(normalizeDistance);

            Vector3 directionCamera = (playerCamera.ScreenToWorldPoint(Input.mousePosition) - playerCamera.transform.position).normalized; //Direction of camera relative to player
            directionCamera.z = 0;

            Vector3 goalPosition = directionCamera * (movementSpeed * forceDistance); //Goal position of camera

            
            if(unlockTime < 1)
            {
                unlockTime += Time.deltaTime *speedFactorToUnlock;
                Debug.Log("unlock time : " + unlockTime);
                playerCamera.transform.localPosition = new Vector3(cameraUnlockStabilizeCurveMovement.Evaluate(unlockTime) * goalPosition.x, cameraUnlockStabilizeCurveMovement.Evaluate(unlockTime) * goalPosition.y, -10); //Apply curve until reach timing

            }
            else
            {
                playerCamera.transform.localPosition = goalPosition + new Vector3(0, 0, -10); //If timing is pass, player can place camera instant
            }
            
            
        }

        if(distanceCamera >= maxDistance && unlockTime > speedFactorToUnlock) //Max Distance btw player and camera
        {
            Vector3 directionCamera = (playerCamera.ScreenToWorldPoint(Input.mousePosition) - playerCamera.transform.position).normalized;
            float movementSpeed = cameraCurveMovement.Evaluate(1);
            playerCamera.transform.localPosition = directionCamera * (movementSpeed * forceDistance) + new Vector3(0, 0, -10);
        }
    }

    private void CameraLock()
    {
        StartCoroutine(CameraLockManager());
    }

    //Do once, when press lockCamera
    private IEnumerator CameraLockManager()
    {
        startLockPosition = playerCamera.transform.localPosition;
        unlockTime = 0; //reset for unlock
        float time = 0;
        canUnlock = false;
        while (time< 1)
        {
            playerCamera.transform.localPosition = new Vector3(startLockPosition.x * (1-cameraLockStabilizeCurveMovement.Evaluate(time)), startLockPosition.y * (1 - cameraLockStabilizeCurveMovement.Evaluate(time)), -10);
            time += Time.deltaTime*speedFactorToLock;
            yield return new WaitForEndOfFrame();
        }
        playerCamera.transform.localPosition = new Vector3(0, 0, -10);
        canUnlock = true;
        yield return null;
    }
}
