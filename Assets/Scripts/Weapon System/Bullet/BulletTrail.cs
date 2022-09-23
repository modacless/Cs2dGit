using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{

    public float speed;
    public Vector3 direction;
    public float time;

    private int layerMask;

    public void Start()
    {
        layerMask = LayerMask.GetMask("Player", "Wall");
    }


    public virtual void FixedUpdate()
    {
        if(time < 0)
        {
            Destroy(gameObject);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed, layerMask);
        if (hit)
        {
            Destroy(gameObject);

        }

        transform.position += direction * speed ;
        time -= Time.deltaTime;
    }

   
}
