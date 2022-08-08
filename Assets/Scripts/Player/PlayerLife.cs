using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class PlayerLife : NetworkBehaviour
{
    //Références
    [SerializeField]
    private ScriptablePlayerData playerData;

    [SyncVar(OnChange = nameof(PlayerHpChange))]
    public int playerHp;

    private WaitForFixedUpdate waitForFixed;

    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);
        playerHp = playerData.playerLifePoint;
        waitForFixed = new WaitForFixedUpdate();
    }

    #region Change Hp

    void PlayerHpChange(int oldValue, int newValue, bool asServer)
    {
        Debug.Log("Hit : " + playerHp);
        if(newValue <= 0 && oldValue > 0)
        {
            PlayerDie();
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
        RespawnManager(playerData.timeToRespawn);
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
    }

    private void RespawnOnPoint()
    {

        playerHp = playerData.playerLifePoint;
    }
    #endregion
}
