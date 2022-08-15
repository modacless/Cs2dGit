using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBuyWeaponBehavior : MonoBehaviour
{
    private Button buttonBuy;
    public string WeaponToBuy;

    public void InitButton(WeaponButtonData weaponData)
    {
        string WeaponToBuy;
    }

    void Start()
    {
        buttonBuy = GetComponent<Button>();
        buttonBuy.onClick.AddListener(Buy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Buy()
    {
        Weapon weapon = ScriptablePlayerData.allWeaponDictionary[WeaponToBuy].GetComponent<Weapon>();
        if(weapon != null)
        {
            
        }
        else
        {
            Debug.Log("Doesn't exist");
        }
    }
}
