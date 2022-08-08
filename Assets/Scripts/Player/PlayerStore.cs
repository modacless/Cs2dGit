using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerStore : MenuTemple
{
    [SerializeField]
    private ScriptablePlayerData playerData;

    [Header("Reference")]
    public GameObject StoreUi;

    [Header("Money")]
    public int money;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        StoreUi.SetActive(false);
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

    void BuyItem()
    {

    }
}
