using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet;
using FishNet.Object.Synchronizing;

public class NetworkTeamManager : NetworkBehaviour
{
  
    //Ref of spawn data
    [Header("References")]
    public GameObject prefabPlayer;
    public SpawnTeam terroristSpawn;
    public SpawnTeam antiterroristSpawn;


    //Client Data
    [SyncVar(OnChange = nameof(TerroristNumberChange))]
    [HideInInspector]
    public int terroristNumber;
    [SyncVar(OnChange = nameof(AntiterroristNumberChange))]
    [HideInInspector]
    public int antiterroristNumber;


    //Observer pattern to create object player and set in a team
    public delegate void StaticUpdateTerroristTeamNumber(string Text);
    public static event StaticUpdateTerroristTeamNumber staticTerroristNumber;

    public delegate void StaticUpdateAntiTerroristTeamNumber(string Text);
    public static event StaticUpdateAntiTerroristTeamNumber staticAntiTerroristNumber;

    //Server Data
    [SyncObject]
    public readonly SyncList<GameObject> terroristTeam = new SyncList<GameObject>();
    [SyncObject]
    public readonly SyncList<GameObject> antiterroristTeam = new SyncList<GameObject>();
    [SyncObject]
    public readonly SyncList<GameObject> allPlayerList = new SyncList<GameObject>();




    public void Start()
    {
        TeamMenuBehaviour.staticSpawnTerrorist += ServerRpcCreateTerrorist;
        TeamMenuBehaviour.staticSpawnAntiTerrorist += ServerRpcCreateAntiTerrorist;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

    }

    #region FishNet Function Base


    public override void OnStartClient()
    {
        base.OnStartClient();

        staticTerroristNumber?.Invoke(terroristTeam.Count.ToString());
        staticAntiTerroristNumber?.Invoke(antiterroristTeam.Count.ToString());
        Debug.Log("Terrorist Team : "+terroristTeam.Count);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (IsOwner)
        {
            TeamMenuBehaviour.staticSpawnTerrorist -= ServerRpcCreateTerrorist;
            TeamMenuBehaviour.staticSpawnAntiTerrorist -= ServerRpcCreateAntiTerrorist;
        }
    }

    #endregion

    [Server]
    private GameObject CreatePlayer(NetworkConnection conn, Vector3 spawn)
    {
        GameObject player = Instantiate(prefabPlayer, spawn, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(player, conn);

        return player;
    }

    #region Team Management
    [ServerRpc(RequireOwnership = false)]
    public void ServerRpcCreateAntiTerrorist(NetworkConnection clientConnection = null)
    {
        antiterroristTeam.Add(CreatePlayer(clientConnection, antiterroristSpawn.SpawnPosition()));
        antiterroristNumber += 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerRpcCreateTerrorist(NetworkConnection clientConnection = null)
    {
        terroristTeam.Add(CreatePlayer(clientConnection, terroristSpawn.SpawnPosition()));
        terroristNumber += 1;
    }

    //Sync func to  updtae number player in each team
    private void TerroristNumberChange(int oldValue, int newValue, bool isServer)
    {
        staticTerroristNumber?.Invoke(newValue.ToString());
    }

    private void AntiterroristNumberChange(int oldValue, int newValue, bool isServer)
    {
        staticAntiTerroristNumber?.Invoke(newValue.ToString());
    }

    #endregion
}
