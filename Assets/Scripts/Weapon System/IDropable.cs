using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDropable
{
    public void DropItem();
    public void ServerRpcDropItem();
    public void ObserverRpcDropItem();
}
