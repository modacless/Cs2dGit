using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerMenu : NetworkBehaviour
{
    //References
    [Header("References")]
    [SerializeField]
    private ScriptablePlayerData playerData;
    [SerializeField]
    private GameObject mainMenuUi;

    //Observer pattern for disconnect
    public delegate void StaticShootDisconnect();
    public static event StaticShootDisconnect staticDisconnectPlayer;

    void Start()
    {
        mainMenuUi.SetActive(false);
    }

    void Update()
    {
        if (IsOwner)
        {
            EnableDisableMenu();
        }

    }

    void EnableDisableMenu()
    {
        if (Input.GetKeyDown(playerData.openCloseMenu))
        {
            mainMenuUi.SetActive(!mainMenuUi.activeSelf);
        }
    }

    #region Button callback
    public void OnPressedExit()
    {
        staticDisconnectPlayer?.Invoke();
    }

    public void OnPressedResume()
    {
        mainMenuUi.SetActive(false);
    }
    #endregion


}
