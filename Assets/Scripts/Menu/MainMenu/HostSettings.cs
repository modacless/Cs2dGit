using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Scened;
using UnityEngine.SceneManagement;
using FishNet.Connection;
using UnityEditor;

[System.Serializable]
public struct GameSettings
{
    public int maxPlayer;
    public int timePerRound;
    public MapToLoad map;
    public Dictionary<NetworkConnection, GameObject> clientObjects;
    
}

public class HostSettings : MonoBehaviour
{
    public TMP_InputField inputMaxPlayer, inputTimePerRound;
    public static GameSettings gameSettings;

    [SerializeField]
    private GameObject prefabsServerSettings;

    #region Host Parameter && Button
    public void OnPressedStartHost()
    {
        StartCoroutine(StartServer());

    }

    public IEnumerator StartServer()
    {
        //Start server
        InstanceFinder.ServerManager.StartConnection();
        yield return new WaitUntil(() => InstanceFinder.ServerManager.Started);

        string sceneToUnload = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        //Load Scene
        SceneLoadData sld = new SceneLoadData(gameSettings.map.loadName);
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.NetworkManager.SceneManager.LoadGlobalScenes(sld);

        InstanceFinder.ClientManager.StartConnection();

        //Unload Scene
        SceneUnloadData sud = new SceneUnloadData(sceneToUnload);
        InstanceFinder.NetworkManager.SceneManager.UnloadGlobalScenes(sud);

    }

    #endregion
}
