using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public abstract class BulletBehavior : NetworkBehaviour
{

    // Update is called once per frame

    public virtual void BulletDie()
    {
        Destroy(gameObject);
    }

}
