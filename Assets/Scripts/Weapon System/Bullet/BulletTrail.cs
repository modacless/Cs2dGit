using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{

    public float speed;
    public Vector3 direction;
    public float time = 2;

    public void Start()
    {
        time = 2;
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

    public void UpdateImpact(Vector3 impact)
    {

    }
}
