using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using TMPro;
using FishNet.Object.Synchronizing;

public class TeamMenuBehaviour : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject teamMenu;
    public TMP_Text terroristNumberText;
    public TMP_Text antiterroristNumberText;

    //Observer pattern to create object player and set in a team
    public delegate void StaticSpawnInTerrorist(NetworkConnection clientConnection);
    public static event StaticSpawnInTerrorist staticSpawnTerrorist;

    public delegate void StaticSpawnInAntiTerrorist(NetworkConnection clientConnection);
    public static event StaticSpawnInAntiTerrorist staticSpawnAntiTerrorist;


    void Start()
    {
        teamMenu.SetActive(true);
        NetworkTeamManager.staticAntiTerroristNumber += UpdateTextAntiTerroristNumber;
        NetworkTeamManager.staticTerroristNumber += UpdateTextTerroristNumber;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (IsOwner)
        {
            NetworkTeamManager.staticAntiTerroristNumber -= UpdateTextAntiTerroristNumber;
            NetworkTeamManager.staticTerroristNumber -= UpdateTextTerroristNumber;
        }
    }


    private void HideTeamMenu()
    {
        teamMenu.SetActive(false);
    }

    #region Ui Interraction

    //Button
    public void OnPressedTerrorist()
    {
        HideTeamMenu();
        staticSpawnTerrorist?.Invoke(LocalConnection);
    }

    public void OnPressedAntiTerrorist()
    {
        HideTeamMenu();
        staticSpawnAntiTerrorist?.Invoke(LocalConnection);
    }

    //Text Update
    private void UpdateTextTerroristNumber(string number)
    {
        terroristNumberText.text = number;
    }

    private void UpdateTextAntiTerroristNumber(string number)
    {
        antiterroristNumberText.text = number;
    }

    #endregion
}
