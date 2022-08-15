using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using FishNet.Connection;

[System.Serializable]
public struct InventoryWeapon
{
    public void Init(Weapon weaponInit, WeaponTypeInHand typeinHand)
    {
        weaponInInventory = weaponInit;
        weaponTypeInHand = typeinHand;
    }
    public Weapon weaponInInventory;
    public WeaponTypeInHand weaponTypeInHand;
}

public class PlayerWeaponSystem : NetworkBehaviour
{
    [SerializeField]
    private ScriptablePlayerData playerData;

    private PlayerLife playerLife;

    //Sync data
    [SyncObject]
    public readonly SyncList<InventoryWeapon> inventoryPlayerWeapon = new SyncList<InventoryWeapon>();
    private InventoryWeapon mainInventory;
    private InventoryWeapon SecondInventory;
    private InventoryWeapon AccesInventory;

    [SyncVar] public int inventorySelection;

    //Data
    private delegate void EndSpawn(Weapon wepeaonToWait);

    private void Start()
    {
        playerLife = GetComponent<PlayerLife>();
    }

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();

        //Init Inventory;
        playerData.actualPlayerWeapon = null;
        mainInventory = new InventoryWeapon();
        mainInventory.Init(null, WeaponTypeInHand.Primary);
        SecondInventory = new InventoryWeapon();
        SecondInventory.Init(null, WeaponTypeInHand.Secondary);
        AccesInventory = new InventoryWeapon();
        AccesInventory.Init(null, WeaponTypeInHand.Accessory);

        if(ScriptablePlayerData.allWeaponDictionary.Count != playerData.allweapon.Length)
        {
            foreach (AllWeapon weapon in playerData.allweapon)
            {

                ScriptablePlayerData.allWeaponDictionary.Add(weapon.name, weapon.weapon);
            }
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        //Add on Server cause sync list
        inventoryPlayerWeapon.Add(mainInventory);
        inventoryPlayerWeapon.Add(SecondInventory);
        inventoryPlayerWeapon.Add(AccesInventory);

    }


    // Update is called once per frame
    void Update()
    {
        if (IsOwner && playerLife.playerHp > 0)
        {
            //Check mosue wheel
            if(Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                ServerRpcSelectInInventory(Input.GetAxis("Mouse ScrollWheel"));
            }
            //Check key
            if (Input.GetKeyDown(playerData.primaryWeaponInventory))
            {
                ServerRpcSelectWeaponWithKey(playerData.primaryWeaponInventory);
            }

            if (Input.GetKeyDown(playerData.secondaryWeaponInventory))
            {
                ServerRpcSelectWeaponWithKey(playerData.secondaryWeaponInventory);
            }

            if (Input.GetKeyDown(playerData.accessoryWeaponInventory))
            {
                ServerRpcSelectWeaponWithKey(playerData.accessoryWeaponInventory);
            }

            if (Input.GetMouseButton(playerData.mouseShootButton) && playerData.actualPlayerWeapon != null)
            {
                playerData.actualPlayerWeapon.Shoot();
            }

            if (!Input.GetMouseButton(playerData.mouseShootButton) && playerData.actualPlayerWeapon != null)
            {
                playerData.actualPlayerWeapon.DecraseSpread();
            }

            if (Input.GetKeyDown(playerData.reloadKey) && playerData.actualPlayerWeapon != null)
            {
                playerData.actualPlayerWeapon.Reload();
            }

        }
    }

    #region Select weapon
    //Select weapon with key
    [ServerRpc]
    public void ServerRpcSelectWeaponWithKey(KeyCode key)
    {
        Debug.Log(key);
        if (key == playerData.primaryWeaponInventory)
        {
            inventorySelection = 0;
        }

        if (key == playerData.secondaryWeaponInventory)
        {
            inventorySelection = 1;
        }


        if (key == playerData.accessoryWeaponInventory)
        {
            inventorySelection = 2;
        }

        ObserverRpcSelectWeapon(inventorySelection);
        
    }

    //SelectWeapon with mosueWheel
    [ServerRpc]
    public void ServerRpcSelectInInventory(float mouseWheel)
    {

        if (mouseWheel > 0f)
        {
            inventorySelection--;
        }
        else if (mouseWheel < 0f) // backwards
        {
            inventorySelection++;
        }

        if (inventorySelection < 0)
        {
            inventorySelection = inventoryPlayerWeapon.Count - 1;
        }

        if (inventorySelection > inventoryPlayerWeapon.Count - 1)
        {
            inventorySelection = 0;
        }
        
        ObserverRpcSelectWeapon(inventorySelection);

    }

    [ServerRpc]
    private void ServerRpcSelectWeapon(int WeaponToChooseInInventory)
    {
        ObserverRpcSelectWeapon(WeaponToChooseInInventory);
    }

    [ObserversRpc]
    private void ObserverRpcSelectWeapon(int WeaponToChooseInInventory)
    {

        for (int i = 0; i < inventoryPlayerWeapon.Count; i++)
        {
            if (inventoryPlayerWeapon[i].weaponInInventory != null)
            {
                if (i == WeaponToChooseInInventory)
                {
                    inventoryPlayerWeapon[i].weaponInInventory.HideWeapon(true);

                }
                else
                {
                    inventoryPlayerWeapon[i].weaponInInventory.HideWeapon(false);
                }
            }

        }
        if (IsOwner)
        {
            playerData.actualPlayerWeapon = inventoryPlayerWeapon[WeaponToChooseInInventory].weaponInInventory;
        }
        
    }

    #endregion

    #region Add Weapon in inventory

    [ServerRpc(RequireOwnership = false)]
    public void RpcAddInInventory(string weaponName, NetworkConnection conn = null)
    {
        if(weaponName == string.Empty)
        {
            throw new System.ArgumentNullException("This weapon doesn't exist");
        }

        Weapon weaponToAdd = ScriptablePlayerData.allWeaponDictionary[weaponName].GetComponent<Weapon>();
        for (int i = 0; i< inventoryPlayerWeapon.Count - 1; i++)
        {
            if(inventoryPlayerWeapon[i].weaponTypeInHand == weaponToAdd.weaponTypeInHand)
            {
                //Drop
                //---
                //---

                if(inventoryPlayerWeapon[inventorySelection].weaponInInventory != null)
                {
                    //Drop
                    return;
                }

                //Create Item
                if (transform.Find("BodySprite") == null)
                    throw new System.ArgumentException("BodySprite doesn't exist on player");

                //Spawn weapon
                GameObject weaponToSpawn = Instantiate(weaponToAdd.gameObject,new Vector3(0,0,0),Quaternion.identity,transform.Find("BodySprite"));
                base.Spawn(weaponToSpawn, conn);
                //weaponToSpawn.transform.position = new Vector3(0, 0, 0);
                ClientRpcEndSpawnWeapon(weaponToSpawn.GetComponent<Weapon>(), i);



                //Add in list
                InventoryWeapon copy = inventoryPlayerWeapon[i];
                copy.weaponInInventory = weaponToSpawn.GetComponent<Weapon>();
                inventoryPlayerWeapon[i] = copy;
                return;
            }
        }

    }

    [ObserversRpc]
    private void ClientRpcEndSpawnWeapon(Weapon weapontoSpawn, int i)
    {
        if (IsOwner)
        {
            StartCoroutine(endSpawnWeapon(weapontoSpawn, i));
        }

    }

    private IEnumerator endSpawnWeapon(Weapon weapontToSpawn, int i)
    {
        weapontToSpawn.HideWeapon(false);
        yield return new WaitUntil(delegate () {
            return weapontToSpawn.IsSpawned;
        });

        weapontToSpawn.HideWeapon(true);
        ServerRpcSelectWeapon(i);
    }

    #endregion

    #region Player Shoot and reload

    #endregion
}
