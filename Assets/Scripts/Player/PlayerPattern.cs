using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPattern : MonoBehaviour
{
    public Rigidbody2D rbd;
    public float speedFactor;
    private float vx, vy;
    
    // Start is called before the first frame update
    void Start()
    {
        vx = 0;
        vy = 0;
        rbd = this.GetComponent<Rigidbody2D>();

    }


    void Update()
    {
        this.getMove();
    }
   
    private void FixedUpdate()
    {
        rbd.velocity = new Vector2(speedFactor*vx, vy/(speedFactor+1));
    }



    private void getMove()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            vy = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            vy = -1;
        }
        else
        {
            vy = 0;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            vx = 1;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            vx = -1;
        }
        else
        {
            vx = 0;
        }
    }
}


