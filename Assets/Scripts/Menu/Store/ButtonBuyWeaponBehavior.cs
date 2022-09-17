using FishNet.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonBuyWeaponBehavior : MonoBehaviour
{
    //References
    [SerializeField] private Button buttonBuy;
    [SerializeField] private TMP_Text weaponNameUI;
    [SerializeField] private TMP_Text weaponCostUI;
    [SerializeField] private Image weaponImageUi;

    //Data
    public string weaponToBuy;
    private WeaponButtonData buttonData;

    //To create weapon
    public delegate void StaticBuyItemDelegate(string weaponName, NetworkConnection conn = null);
    public static event StaticBuyItemDelegate staticBuy;

    //To Update UI money
    public delegate void StaticBuyItemUiDelegate();
    public static event StaticBuyItemUiDelegate staticBuyUpdateUi;

    //To Drop if already got weapon
    public delegate void StaticDropitemDelegate(Weapon weapon);
    public static event StaticDropitemDelegate staticDropitem;
    //--//

    public void InitButton(WeaponButtonData weaponData)
    {
        weaponToBuy = weaponData.weaponName;
        weaponNameUI.text = weaponData.weaponName;
        weaponCostUI.text = weaponData.weaponCost + " $";
        weaponImageUi.sprite = weaponData.weaponImage;

        buttonData = weaponData;
    }

    void Start()
    {
        buttonBuy = GetComponent<Button>();
        buttonBuy.onClick.AddListener(Buy);
    }

    void Buy()
    {
        int costWeapon = int.Parse(buttonData.weaponCost);
        if (PlayerStore.money >= costWeapon)
        {
            PlayerStore.money -= costWeapon;
            staticBuyUpdateUi?.Invoke();

            Weapon weapon = ScriptablePlayerData.allWeaponDictionary[weaponToBuy].GetComponent<Weapon>();
            staticDropitem?.Invoke(weapon);

            if (weapon != null)
            {
               staticBuy?.Invoke(weaponToBuy);
            }
            else
            {
                Debug.Log("Doesn't exist");
            }

        }
    }
}
