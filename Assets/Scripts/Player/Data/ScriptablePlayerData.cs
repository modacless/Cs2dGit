using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;

[CreateAssetMenu(menuName = "Player/Data", fileName = "Data", order = 0)]
public class ScriptablePlayerData : ScriptableObject
{

    //Global Player Data
    [HideInInspector]
    public Transform rotationCameraTransform;  //Never change it
    [HideInInspector]
    public Vector3 rotationCamera;  //Never change it
    [HideInInspector]
    public Weapon actualPlayerWeapon;
    [HideInInspector]
    public bool inverseAnimation = false;

    [Header("Input")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode upKey;
    public KeyCode downKey;

    //Inventory
    public KeyCode primaryWeaponInventory;
    public KeyCode secondaryWeaponInventory;
    public KeyCode accessoryWeaponInventory;

    public KeyCode openStoreUi;

    //Camera
    public KeyCode rotateRCamera;
    public KeyCode rotateLCamera;

    public KeyCode lockUnlockCamera;

    //Menu
    public KeyCode openCloseMenu;

    //Weapon
    public KeyCode reloadKey;
    [Range(0, 3)]
    public int mouseShootButton;
    public KeyCode dropWeapon;

    [Header("Player Data")]
    public int playerLifePoint = 100;
    public float speed;

    [Header("Camera Data")]
    //Rotate speed in degres per s
    public float rotateCameraSpeed;

    [Header("Respawn")]
    public float timeToRespawn;

    public AllWeapon[] allweapon;
    public static Dictionary<string, GameObject> allWeaponDictionary = new Dictionary<string, GameObject>();


}

[System.Serializable]
public struct AllWeapon
{
    public string name;
    public GameObject weapon;
}

