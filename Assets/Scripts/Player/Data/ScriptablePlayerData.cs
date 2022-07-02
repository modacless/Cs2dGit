using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Data",fileName ="Data",order = 0)]
public class ScriptablePlayerData : ScriptableObject
{

    //Global Player Data
    [HideInInspector]
    public Transform rotationCameraTransform;  //Never change it
    [HideInInspector]
    public Vector3 rotationCamera;  //Never change it

    [Header("Input")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode upKey;
    public KeyCode downKey;

    //Camera
    public KeyCode rotateRCamera;
    public KeyCode rotateLCamera;

    //Weapon
    public KeyCode reloadKey;
    
    [Header("Player Data")]
    public float speed;

    [Header("Camera Data")]
    //Rotate speed in degres per s
    public float rotateCameraSpeed;
}
