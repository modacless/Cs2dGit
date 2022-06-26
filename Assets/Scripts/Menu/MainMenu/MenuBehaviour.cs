using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet;
using FishNet.Transporting.Tugboat;
using UnityEngine.SceneManagement;
using System;

public class MenuBehaviour : MonoBehaviour
{
    public List<GameObject> sceneMenu;
    public Dictionary<string, GameObject> sceneMenuSort = new Dictionary<string, GameObject>();

    [SerializeField]
    private TMP_InputField inputIp;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in sceneMenu)
        {
            sceneMenuSort.Add(go.name, go);
        }

        ChangeSceneMenu("StartMenu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region Main Menu Button
    public void OnPressedHost()
    {
        ChangeSceneMenu("HostMenu");
    }

    public void OnPressedJoin()
    {

        string sceneToUnload = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        InstanceFinder.TransportManager.Transport.SetClientAddress(inputIp.text);
        InstanceFinder.ClientManager.StartConnection();

    }

    public void OnPressedSettings()
    {
    
    }

    public void OnPressedExit()
    {

    }

    public void OnPressedMenu()
    {
        ChangeSceneMenu("StartMenu");
    }

    #endregion


    void ChangeSceneMenu(string nameOfScene)
    {
        foreach (string name in sceneMenuSort.Keys)
        {
            if(nameOfScene == name)
            {
                sceneMenuSort[name].SetActive(true);
            }
            else
            {
                sceneMenuSort[name].SetActive(false);
            }
        }
    }

}
