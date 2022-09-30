using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;


public enum PlayerState
{
    Normal,
    Dead
}

public class PlayerLife : NetworkBehaviour
{
    //Références
    [SerializeField]
    private ScriptablePlayerData playerData;

    [SyncVar(OnChange = nameof(PlayerHpChange))]
    public int playerHp;

    [HideInInspector]
    public PlayerState playerState;

    private WaitForFixedUpdate waitForFixed;

    //Observer pattern (for revive and die)
    public delegate void StaticRevivePlayerDelegate();
    public static event StaticRevivePlayerDelegate staticRevive;

    public delegate void StaticDiePlayerDelegate();
    public static event StaticDiePlayerDelegate staticDie;

    //Debug
    [Header("Debug")]
    public bool debugActivate;

    #region Fishnet Base Function
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            ServerRpcInitHp();
            playerState = PlayerState.Normal;
        }

    }


    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);
        waitForFixed = new WaitForFixedUpdate();
    }

    #endregion

    #region Change Hp

    void PlayerHpChange(int oldValue, int newValue, bool asServer)
    {
        if (IsOwner)
        {
            if (newValue <= 0 && oldValue > 0)
            {
                PlayerDie();
            }
        }
        else
        {
            if (debugActivate)
            {
                if (newValue <= 0)
                {
                    Debug.Log("Hit : " + playerHp);
                    PlayerDie();
                }
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void ServerRpcTakeDamage(int Damage)
    {
        playerHp -= Damage;
    }

    #endregion

    #region Die and Spawn
    private void PlayerDie()
    {
        playerState = PlayerState.Dead;
        GetComponent<BoxCollider2D>().enabled = false;
        staticDie?.Invoke();
        //RespawnManager(playerData.timeToRespawn);
    }

    private void PlayerRevive()
    {
        playerState = PlayerState.Normal;
        GetComponent<BoxCollider2D>().enabled = true;
        staticRevive?.Invoke();
    }

    private IEnumerator RespawnManager(float time)
    {
        Debug.Log("Try to respawn");
        double _timeToRespawn = 0;
        while(_timeToRespawn < time)
        {
            _timeToRespawn += InstanceFinder.TimeManager.TickDelta;
            yield return waitForFixed;
        }
        RespawnOnPoint();
        PlayerRevive();
    }

    private void RespawnOnPoint()
    {

        playerHp = playerData.playerLifePoint;
    }

    [ServerRpc]
    private void ServerRpcInitHp()
    {
        playerHp = playerData.playerLifePoint;
    }
    #endregion
}
