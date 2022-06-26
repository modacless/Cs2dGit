using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using TMPro;
using FishNet.Object.Synchronizing;

public class TeamMenuBehaviour : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject teamMenu;
    public TMP_Text terroristNumberText;
    public TMP_Text antiterroristNumberText;

    [SyncVar(OnChange = nameof(TerroristNumberChange))]
    [HideInInspector]
    public int terroristNumber;
    [SyncVar(OnChange = nameof(AntiterroristNumberChange))]
    [HideInInspector]
    public int antiterroristNumber;

    [SyncObject]
    public readonly SyncList<GameObject> terroristTeam = new SyncList<GameObject>();
    [SyncObject]
    public readonly SyncList<GameObject> antiterroristTeam = new SyncList<GameObject>();

    public GameObject prefabPlayer;
    public SpawnTeam terroristSpawn;
    public SpawnTeam antiterroristSpawn;

    void Start()
    {
        teamMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        
    }

    public void OnPressedTerrorist()
    {
        HideTeamMenu();
        RpcPressedTerrorist(LocalConnection);
        //terroristTeam.Add(CreatePlayer());
    }

    [ServerRpc(RequireOwnership = false)]
    public void RpcPressedTerrorist(NetworkConnection clientConnection)
    {
        terroristTeam.Add(CreatePlayer(clientConnection, terroristSpawn.SpawnPosition()));
    }

    public void OnPressedAntiTerrorist()
    {
        HideTeamMenu();
        RpcPressedAntiTerrorist(LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RpcPressedAntiTerrorist(NetworkConnection clientConnection)
    {
        antiterroristTeam.Add(CreatePlayer(clientConnection, antiterroristSpawn.SpawnPosition()));
    }

    private GameObject CreatePlayer(NetworkConnection conn, Vector3 spawn)
    {
        GameObject player = Instantiate(prefabPlayer, spawn, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(player, conn);
        
        return player;
    }

    private void HideTeamMenu()
    {
        teamMenu.SetActive(false);
    }


    private void TerroristNumberChange(int oldValue, int newValue, bool isServer)
    {
        terroristNumberText.text = newValue.ToString();
    }


    private void AntiterroristNumberChange(int oldValue, int newValue, bool isServer)
    {
        antiterroristNumberText.text = newValue.ToString();
    }
}
