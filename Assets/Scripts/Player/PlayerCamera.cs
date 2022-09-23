using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    Lock,
    Unlock,
    Dead
}

public class PlayerCamera : MonoBehaviour
{

    //Player data
    [Header("References")]
    [SerializeField]
    private PlayerLife playerlife;

    //Camera initialisation
    private Camera playerCamera;
    private Vector3 _initCamPos;

    //Camera state (to change pos ect..)
    [HideInInspector]
    public CameraState cameraState;

    void Start()
    {
        Weapon.staticShoot += CameraShake;
        playerCamera = Camera.main;
        _initCamPos = playerCamera.transform.position;
        cameraState = CameraState.Lock;
    }

    // Update is called once per frame
    void Update()
    {
        
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

            Camera.main.transform.position += new Vector3(x, y, 0);
            t += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = new Vector3(0,0, _initCamPos.z);
    }

    private void CameraUnlock()
    {

    }
}
