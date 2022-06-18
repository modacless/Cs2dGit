using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPattern : MonoBehaviour
{
    public Rigidbody2D rbd;
    public float speedFactor;
    private float vx, vy;
    private float ax, ay;

    // Start is called before the first frame update
    void Start()
    {
        vx = 0;
        vy = 0;
        ax = 0;
        ay = 0;
        rbd = this.GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        this.getMove();
    }

    private void FixedUpdate()
    {
        rbd.velocity = new Vector2(vx*speedFactor, vy*speedFactor);
    }

    private void getMove()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            ay += 0.1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ay = -0.1f;
        }
        else
        {
            ay = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            ax += 0.1f;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            ax -= 0.1f;
        }
        else
        {
            ax = 0;
        }

       //checkAccelerate();
        vx = ax * Time.deltaTime;
        vy = ay * Time.deltaTime;
    }



    private void checkAccelerate()
    {
        if (Mathf.Abs(ax) > 1)
        {
            ax = ax / Mathf.Abs(ax);
        }
        if (Mathf.Abs(ax) > 1)
        {
            ay = ay / Mathf.Abs(ay);
        }
    }

}