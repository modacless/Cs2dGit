using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
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
        InputManager();
        Vector2 inputMovement = new Vector2(right - left, up - down);
        Debug.Log(right);
        rb.velocity = inputMovement * playerData.speed * Time.deltaTime;
    }

    void FixedUpdate()
    {

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
