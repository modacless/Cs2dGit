using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public float speed;
    public virtual IEnumerator TrailManager(Vector3 impactPosition)
    {
        Vector3 startPosition = transform.position;
        float distance = (impactPosition - startPosition).magnitude;
        float startingDistance = distance;

        while (distance > 0)
        {
            transform.position = Vector3.Lerp(startPosition, impactPosition, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * speed;
            yield return null;
        }
        Destroy(gameObject);
    }
}
