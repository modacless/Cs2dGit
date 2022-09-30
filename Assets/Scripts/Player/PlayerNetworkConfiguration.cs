using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Object;

public class PlayerNetworkConfiguration : NetworkBehaviour
{
    // Start is called before the first frame update


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            PlayerMenu.staticDisconnectPlayer += DisconnectPlayer;
            NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }
    }

    private void ClientManager_OnClientConnectionState(FishNet.Transporting.ClientConnectionStateArgs obj)
    {
        if(obj.ConnectionState == FishNet.Transporting.LocalConnectionState.Stopped)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("BaseMenu");
        }
    }

    public void DisconnectPlayer()
    {
        if (IsServer) //Send Disconnect message to EveryBody
        {
            NetworkManager.ServerManager.StopConnection(true);
        }
        else //Disconnect only this client
        {
            NetworkManager.ClientManager.StopConnection();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("BaseMenu");
    }

}
