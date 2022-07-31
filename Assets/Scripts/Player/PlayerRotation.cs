using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerRotation : NetworkBehaviour
{
    //Références
    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    private Camera cameraPlayer;
    [SerializeField]
    private GameObject bodyObject;
    [SerializeField]
    private GameObject legsObject;


    void Start()
    {
        
    }

    void Update()
    {
        if (IsOwner)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();
            float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            bodyObject.transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + 90f); ;
        }

        

    }

}
