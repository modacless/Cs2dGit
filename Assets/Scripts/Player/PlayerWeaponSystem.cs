using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;

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

    //Sync data
    [SyncObject]
    public readonly SyncList<InventoryWeapon> inventoryPlayerWeapon = new SyncList<InventoryWeapon>();
    [SyncVar]
    public int money;

    [SyncVar] public int inventorySelection;

    //Data
    private delegate void EndSpawn(Weapon wepeaonToWait);

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();

            //Init Delegate

            //Init Inventory;
            playerData.actualPlayerWeapon = null;
            InventoryWeapon mainInventory = new InventoryWeapon();
            mainInventory.Init(null, WeaponTypeInHand.Primary);
            InventoryWeapon SecondInventory = new InventoryWeapon();
            SecondInventory.Init(null, WeaponTypeInHand.Secondary);
            InventoryWeapon AccesInventory = new InventoryWeapon();
            AccesInventory.Init(null, WeaponTypeInHand.Accessory);

            inventoryPlayerWeapon.Add(mainInventory);
            inventoryPlayerWeapon.Add(SecondInventory);
            inventoryPlayerWeapon.Add(AccesInventory);

        //Init all Weapon

        if (IsOwner)
        {
            
            foreach (AllWeapon weapon in playerData.allweapon)
            {
                ScriptablePlayerData.allWeaponDictionary.Add(weapon.name, weapon.weapon);
            }
        }
  
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
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

            //Spawn Weapon
            if (Input.GetKeyDown(KeyCode.F))
            {
                RpcAddInInventory("Ak47");
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
    public void RpcAddInInventory(string weaponName)
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
                base.Spawn(weaponToSpawn,ClientManager.Connection);
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
            Debug.Log(weapontToSpawn.IsSpawned);
            return weapontToSpawn.IsSpawned;
        });

        weapontToSpawn.HideWeapon(true);
        ServerRpcSelectWeapon(i);
    }

    #endregion

    #region Player Shoot and reload

    #endregion
}
