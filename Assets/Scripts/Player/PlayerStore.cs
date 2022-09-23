using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using UnityEngine.UI;
using TMPro;
public struct WeaponButtonData
{
    public string weaponName;
    public string weaponCost;
    public Sprite weaponImage;
}

public class PlayerStore : MenuTemple
{
    [Header("Reference")]
    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    private GameObject storeUi;
    [SerializeField]
    private GameObject buttonBuyWeapon;
    [SerializeField]
    private RectTransform sizeOfUi;
    [SerializeField]
    private TMP_Text moneyTextUi;

    //Array Of Button
    public int maxSizeOfArrayButtonWeapon;
    public Dictionary<WeaponType, int>positionInStore = new Dictionary<WeaponType, int>();


    [Header("Money")]
    public int startMoney;

    [HideInInspector] public static int money;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        storeUi.SetActive(false);

        for(int i = 0; i< playerData.allweapon.Length; i++)
        {
            bool hasKey = positionInStore.TryGetValue(playerData.allweapon[i].weapon.GetComponent<Weapon>().weaponType, out int value);
            if(hasKey)
            {
                CreateButton(playerData.allweapon[i].weapon, value);
                positionInStore[playerData.allweapon[i].weapon.GetComponent<Weapon>().weaponType]++;
            }
            else
            {
                CreateButton(playerData.allweapon[i].weapon, 0);
                positionInStore.Add(playerData.allweapon[i].weapon.GetComponent<Weapon>().weaponType, 1);
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            ButtonBuyWeaponBehavior.staticBuyUpdateUi += UpdateMoney;
            money = startMoney;
            UpdateMoney();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(playerData.openStoreUi))
            {
                ChangeSceneMenu("MainStore");
                storeUi.SetActive(!storeUi.activeSelf);
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


    void CreateButton(GameObject weapon, int position)
    {
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

        GameObject button = Instantiate(buttonBuyWeapon,Vector3.zero, Quaternion.identity, menuToAddWeapon.transform);
        button.GetComponent<RectTransform>().localPosition = FindPositionInUi(maxSizeOfArrayButtonWeapon, position);
        Weapon weaponToCreateButton = weapon.GetComponent<Weapon>();
        button.GetComponent<ButtonBuyWeaponBehavior>().InitButton(new WeaponButtonData
        {
            weaponName = weaponToCreateButton.name,
            weaponCost = weaponToCreateButton.moneyCostWeapon.ToString(),
            weaponImage = weaponToCreateButton.weaponSprite
        });

    }

    private Vector3 FindPositionInUi(int maxNumber, int position)
    {
        Vector3 positionUi = sizeOfUi.rect.center + new Vector2(-sizeOfUi.rect.width/2, sizeOfUi.rect.height / 2);
        float widthCase = sizeOfUi.rect.width / maxNumber;
        float heightCase = sizeOfUi.rect.height/ maxNumber;

        Vector2 midPosition = new Vector2(widthCase / 2, heightCase / 2);
        positionUi += new Vector3(widthCase * (position % maxNumber), heightCase * Mathf.Floor(position / maxNumber)) + new Vector3(midPosition.x, -midPosition.y,0);
        return positionUi;
    }

    private void UpdateMoney()
    {
        moneyTextUi.text = money.ToString() + " $";
    }

}
