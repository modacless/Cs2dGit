using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Transporting;

public interface IShootable 
{

    //Shoot
    public void RaycastShoot();
    public void Shoot();
    public void ServerRpcImpact(Vector3 position, Quaternion rotation);

    //Reload
    public void Reload();
    public IEnumerator ReloadManager(double time);

    //Spread
    public void UpdateSpread();
    public void DecraseSpread();


}
