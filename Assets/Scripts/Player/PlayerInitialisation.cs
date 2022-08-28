using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class PlayerInitialisation : NetworkBehaviour
{
    [SerializeField]
    private Camera playerCamera;

    [SerializeField]
    private GameObject CursorObjects;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            Camera.main.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            //CursorObjects.SetActive(true);
        }
        else
        {
            playerCamera.gameObject.SetActive(false);
            CursorObjects.SetActive(false);
        }

        GetComponent<Rigidbody2D>().gravityScale = 0;
    }

}
