using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public struct WeaponButtonData
{
    public string WeaponName;
}

public class PlayerStore : MenuTemple
{
    [Header("Reference")]
    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    private GameObject StoreUi;
    [SerializeField]
    private GameObject ButtonBuyWeapon;

    [Header("Money")]
    public int money;

    //Array Of Button


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        StoreUi.SetActive(false);

        foreach(AllWeapon weapon in playerData.allweapon)
        {
            CreateButton(weapon.name);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(playerData.openStoreUi))
            {
                ChangeSceneMenu("MainStore");
                StoreUi.SetActive(!StoreUi.activeSelf);
            }
        }

    }

    public void OnPressedPistolStore()
    {
        ChangeSceneMenu("PistolStore");
    }
    public void OnPressedRifleStore()
    {
        ChangeSceneMenu("RifleStore");
    }
    public void OnPressedAccessoryStore()
    {
        ChangeSceneMenu("AccessoryStore");
    }
    public void OnPressedShotgunStore()
    {
        ChangeSceneMenu("ShotgunStore");
    }

    void OnpressedMainStore()
    {
        ChangeSceneMenu("MainStore");
    }

    void BuyItem(string ItemName)
    {
        Weapon item = ScriptablePlayerData.allWeaponDictionary[ItemName].GetComponent<Weapon>();
        if (money - item.moneyCostWeapon > 0)
        {
            GetComponent<PlayerWeaponSystem>().RpcAddInInventory(ItemName);
            money -= item.moneyCostWeapon;
        }
    }

    void CreateButton(string weaponName)
    {
        GameObject weapon = sceneMenuSort[weaponName];
        GameObject menuToAddWeapon;
        switch (weapon.GetComponent<Weapon>().weaponType)
        {
            case WeaponType.Pistol:
                menuToAddWeapon = sceneMenuSort["PistolStore"];
                break;
            case WeaponType.Rifle:
                menuToAddWeapon = sceneMenuSort["RifleStore"];
                break;
            case WeaponType.Shotgun:
                menuToAddWeapon = sceneMenuSort["ShotgunStore"];
                break;
            case WeaponType.Accessory:
                menuToAddWeapon = sceneMenuSort["AccessoryStore"];
                break;
            default:
                menuToAddWeapon = sceneMenuSort["PistolStore"];
            break;
        }

        Instantiate(weapon, menuToAddWeapon.transform);
        //weapon.GetComponent<ButtonBuyWeaponBehavior>().InitButton();


    }

}
