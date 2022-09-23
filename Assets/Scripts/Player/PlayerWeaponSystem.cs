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
    private InventoryWeapon secondInventory;
    private InventoryWeapon accesInventory;

    [SyncVar] public int inventorySelection;

    //Data
    private delegate void EndSpawn(Weapon wepeaonToWait);

    public List<Weapon> weaponsOnGroundNearPlayer = new List<Weapon>();

    private bool isBuyingWeapon = false;

    private void Start()
    {
        playerLife = GetComponent<PlayerLife>();

        if (ScriptablePlayerData.allWeaponDictionary.Count == 0)
        {
            foreach (AllWeapon weapon in playerData.allweapon)
            {
                ScriptablePlayerData.allWeaponDictionary.Add(weapon.name, weapon.weapon);
            }
        }

    }

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();

        //Init Inventory;
        playerData.actualPlayerWeapon = null;
        mainInventory = new InventoryWeapon();
        secondInventory = new InventoryWeapon();
        accesInventory = new InventoryWeapon();

        if (IsOwner)
        {
            ButtonBuyWeaponBehavior.staticBuy += RpcAddInInventory;
            ButtonBuyWeaponBehavior.staticDropitem += DropItem;
        }

        //Observer patterns for buying interface
        

    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        mainInventory.Init(null, WeaponTypeInHand.Primary);
        secondInventory.Init(null, WeaponTypeInHand.Secondary);
        accesInventory.Init(null, WeaponTypeInHand.Accessory);

        //Add on Server cause sync list
        inventoryPlayerWeapon.Add(mainInventory);
        inventoryPlayerWeapon.Add(secondInventory);
        inventoryPlayerWeapon.Add(accesInventory);

        for(int i = 0; i< inventoryPlayerWeapon.Count; i++)
        {
            Debug.Log(inventoryPlayerWeapon[i].weaponTypeInHand);
        }


    }


    // Update is called once per frame
    void Update()
    {
        if (IsOwner && playerLife.playerHp > 0)
        {
            //Check mouse wheel inventory
            if(Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                ServerRpcSelectInInventory(Input.GetAxis("Mouse ScrollWheel"));
            }
            //Key inventory
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

            //Shoot Action
            if (Input.GetMouseButton(playerData.mouseShootButton) && playerData.actualPlayerWeapon != null)
            {
                playerData.actualPlayerWeapon.Shoot();
            }

            if (!Input.GetMouseButton(playerData.mouseShootButton) && playerData.actualPlayerWeapon != null)
            {
                playerData.actualPlayerWeapon.DecraseSpread();
            }

            //Reload Action
            if (Input.GetKeyDown(playerData.reloadKey) && playerData.actualPlayerWeapon != null)
            {
                playerData.actualPlayerWeapon.Reload();
            }

            //Drop action
            if(Input.GetKeyDown(playerData.dropWeapon))
            {
                Weapon weaponToPickup = FindNearestWeaponOnGround(weaponsOnGroundNearPlayer); // Chekc the nearest weapon to pickup
                if(weaponToPickup != null)
                {
                    if (inventoryPlayerWeapon[WeaponInWichInventory(weaponToPickup)].weaponInInventory != null)
                    {
                        inventoryPlayerWeapon[WeaponInWichInventory(weaponToPickup)].weaponInInventory.DropItem();
                        DropWeapon(inventoryPlayerWeapon[WeaponInWichInventory(weaponToPickup)].weaponInInventory.gameObject); // Drop weapon if already have one in inventory
                    }

                    ServerRpcPlaceWeaponInHand(weaponToPickup); // Pickup Item
                    weaponToPickup.PickupWeapon();
                    ServerRpcSelectWeapon(inventorySelection);
                }
                else
                {
                    if(playerData.actualPlayerWeapon != null) //If no actual weapon drop item
                    {
                        playerData.actualPlayerWeapon.DropItem();
                        DropWeapon(playerData.actualPlayerWeapon.gameObject);
                    }

                }
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

    private Weapon FindNearestWeaponOnGround(List<Weapon> weaponsGround)
    {
        if(weaponsGround.Count > 0)
        {
            Weapon weaponToReturn = weaponsGround[0];
            float nearestDistance = (transform.position - weaponsGround[0].transform.position).magnitude;
            for(int i = 1; i< weaponsGround.Count; i++)
            {
                float testDistance = (transform.position - weaponsGround[i].transform.position).magnitude;

                if (nearestDistance > testDistance)
                {
                    nearestDistance = testDistance;
                    weaponToReturn = weaponsGround[i];
                }
            }

            return weaponToReturn;
        }
        else
        {
            return null;
        }
    }

    private int WeaponInWichInventory(Weapon weaponToCheck)
    {
        int inventorySlot = 0;
        switch (weaponToCheck.weaponTypeInHand)
        {
            case WeaponTypeInHand.Primary:
                inventorySlot = 0;
                break;
            case WeaponTypeInHand.Secondary:
                inventorySlot = 1;
                break;
            case WeaponTypeInHand.Accessory:
                inventorySlot = 2;
                break;
            default:
                inventorySlot = 0;
                break;
        }

        return inventorySlot;
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

        if (isBuyingWeapon)
        {
            PlayerStore.money += weaponToAdd.moneyCostWeapon;
            return;
        }

        for (int i = 0; i< inventoryPlayerWeapon.Count; i++)
        {
            Debug.Log(inventoryPlayerWeapon[i].weaponTypeInHand);
            if(inventoryPlayerWeapon[i].weaponTypeInHand == weaponToAdd.weaponTypeInHand)
            {
                StopCoroutine("ManageAddInInventory");
                StartCoroutine(ManageAddInInventory(weaponToAdd, i, conn));  
            }
        }

    }


    public IEnumerator ManageAddInInventory(Weapon weaponToAdd, int i, NetworkConnection conn)
    {
        isBuyingWeapon = true;
        yield return new WaitUntil(() => inventoryPlayerWeapon[i].weaponInInventory == null);
        yield return new WaitForSeconds(0.1f);

        //Create Item
        if (transform.Find("BodySprite") == null)
            throw new System.ArgumentException("BodySprite doesn't exist on player");

        //Spawn weapon
        GameObject weaponToSpawn = Instantiate(weaponToAdd.gameObject, new Vector3(0, 0, 0), Quaternion.identity, transform.Find("BodySprite"));
        base.Spawn(weaponToSpawn, conn);
        //weaponToSpawn.transform.position = new Vector3(0, 0, 0);
        ObserverRpcEndSpawnWeapon(weaponToSpawn.GetComponent<Weapon>(), i);

        //Add in list
        InventoryWeapon copy = inventoryPlayerWeapon[i];
        copy.weaponInInventory = weaponToSpawn.GetComponent<Weapon>();
        inventoryPlayerWeapon[i] = copy;
        isBuyingWeapon = false;
        yield break;

    }

    [ServerRpc]
    public void ServerRpcPlaceWeaponInHand(Weapon weaponToPickup)
    {
        InventoryWeapon copy = inventoryPlayerWeapon[WeaponInWichInventory(weaponToPickup)];
        copy.weaponInInventory = weaponToPickup;
        inventoryPlayerWeapon[WeaponInWichInventory(weaponToPickup)] = copy;
        ObserverRpcPlaceWeaponInHand(weaponToPickup);
        weaponToPickup.isRealoading = false; // Need it to stop reloading
    }

    [ObserversRpc(IncludeOwner = true)]
    public void ObserverRpcPlaceWeaponInHand(Weapon weaponToPickup)
    {
        weaponToPickup.transform.parent = transform.Find("BodySprite");
        weaponToPickup.transform.localPosition = Vector3.zero;
        weaponToPickup.transform.localRotation = Quaternion.identity;//Quaternion.Euler(0,0,90);
        if(WeaponInWichInventory(weaponToPickup) != inventorySelection)
        {
            weaponToPickup.HideWeapon(false);
        }
        else
        {
            ServerRpcSelectInInventory(WeaponInWichInventory(weaponToPickup));
        }
    }

    [ObserversRpc]
    private void ObserverRpcEndSpawnWeapon(Weapon weapontoSpawn, int i)
    {
        if (IsOwner)
        {
            StartCoroutine(EndSpawnWeapon(weapontoSpawn, i));
        }

    }

    private IEnumerator EndSpawnWeapon(Weapon weapontToSpawn, int i)
    {
        weapontToSpawn.HideWeapon(false);
        yield return new WaitUntil(delegate () {
            return weapontToSpawn.IsSpawned;
        });

        weapontToSpawn.HideWeapon(true);
        ServerRpcSelectWeapon(i);
    }

    #endregion

    #region Gestion Weapon

    private void DropItem(Weapon weaponToPickup)
    {
        if (inventoryPlayerWeapon[WeaponInWichInventory(weaponToPickup)].weaponInInventory != null)
        {
            inventoryPlayerWeapon[WeaponInWichInventory(weaponToPickup)].weaponInInventory.DropItem();
            DropWeapon(inventoryPlayerWeapon[WeaponInWichInventory(weaponToPickup)].weaponInInventory.gameObject); // Drop weapon if already have one in inventory
        }
    }
    private void DropWeapon(GameObject weaponToDrop)
    {
        if(weaponToDrop != null)
        {
            playerData.actualPlayerWeapon = null;
            ServerRpcDropWeapon(weaponToDrop);
        }
       
    }

    [ServerRpc]
    public void ServerRpcDropWeapon(GameObject weaponToDrop)
    {
        weaponToDrop.GetComponent<NetworkBehaviour>().NetworkObject.RemoveOwnership();
        Weapon weaponComponent = weaponToDrop.GetComponent<Weapon>();
        if (weaponComponent != null)
        {
            InventoryWeapon emptyWeapon;
            switch (weaponComponent.weaponTypeInHand)
            {
                case WeaponTypeInHand.Primary:
                    emptyWeapon = inventoryPlayerWeapon[0];
                    emptyWeapon.weaponInInventory = null;
                    inventoryPlayerWeapon[0] = emptyWeapon;
                    break;
                case WeaponTypeInHand.Secondary:
                    emptyWeapon = inventoryPlayerWeapon[1];
                    emptyWeapon.weaponInInventory = null;
                    inventoryPlayerWeapon[1] = emptyWeapon;
                    break;
                case WeaponTypeInHand.Accessory:
                    emptyWeapon = inventoryPlayerWeapon[2];
                    emptyWeapon.weaponInInventory = null;
                    inventoryPlayerWeapon[2] = emptyWeapon;
                    break;
                default:
                    break;
            }
            //Pay attention can cause problem in mult
            ObserverRpcDropWeapon(weaponToDrop, weaponToDrop.transform.rotation, -weaponToDrop.transform.up);
        }
    }

    [ObserversRpc]
    public void ObserverRpcDropWeapon(GameObject toDrop,Quaternion rotation,Vector3 direction)
    {
        toDrop.transform.parent = null;
        toDrop.GetComponent<Weapon>().ThrowWeapon(direction * 2);
        toDrop.transform.rotation = Quaternion.Euler(0, 0, rotation.eulerAngles.z-90);
        toDrop.transform.position = transform.position;
        toDrop.GetComponent<SpriteRenderer>().enabled = true;
    }


    #endregion
}
