using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;

public class PlayerControl : NetworkBehaviour
{
    //Références
    [SerializeField]
    private ScriptablePlayerData playerData;
    private Rigidbody2D rb;
    //Data
    //Input var
    private int left, right, up, down;

    #region Unity Functions
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (IsOwner)
        {
            InputManager();
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            Vector2 inputMovementH = Quaternion.Euler(playerData.rotationCamera) * Vector2.right * (right - left);
            Vector2 inputMovementV = Quaternion.Euler(playerData.rotationCamera) * Vector2.up * (up - down);
            Vector2 inputMovement = inputMovementH + inputMovementV;
            rb.velocity = inputMovement * playerData.speed * Time.fixedDeltaTime;
        }
    }
    #endregion

    #region Input
    private void InputManager()
    {
        left = Input.GetKey(playerData.leftKey) ? 1 :0;
        right = Input.GetKey(playerData.rightKey) ? 1 : 0;
        down = Input.GetKey(playerData.downKey) ? 1 : 0;
        up = Input.GetKey(playerData.upKey) ? 1 : 0; 
    }
    #endregion
}
