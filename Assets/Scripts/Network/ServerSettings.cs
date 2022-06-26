using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Managing.Scened;
using FishNet;

public class ServerSettings : NetworkBehaviour
{
    
    public GameSettings serverSettings;
    void Start()
    {
        if(IsServer && IsClient)
        {

        }
    }

    public void SetSettings(GameSettings sett)
    {
        serverSettings = sett;
    }

}
