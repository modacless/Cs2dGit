using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{

    public float speed;
    public Vector3 direction;
    public float time = 2;

    public Vector3 endPoint;
    private bool hitSomething;

    public void Start()
    {
        int layerMask = LayerMask.GetMask("Wall", "Player");
        time = 2;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 2000f, layerMask, -Mathf.Infinity, Mathf.Infinity);
        hitSomething = hit;
        if (hit)
        {
            endPoint = hit.point;
            if((transform.position - endPoint).magnitude < 0.1f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if(time < 0)
        {
            Destroy(gameObject);
        }
        transform.position += direction * speed ;
        time -= Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("test");
        if(collision.tag == "Wall" || collision.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
