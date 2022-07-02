using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public interface IShootable 
{

    //Shoot
    public void ServerRpcShoot();
    public void ClientRpcShoot();
    public void InstantiateBullet();
    public void Shoot();

    //Reload
    public void Reload();
    public void ServerRpcReload();
    public void ClientRpcReload(double time);

}
