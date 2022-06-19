using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;
using FishNet;


public class MenuBehaviour : MonoBehaviour
{
    public List<GameObject> sceneMenu;
    public Dictionary<string, GameObject> sceneMenuSort = new Dictionary<string, GameObject>();
    public MapToLoad mapToLoad;

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

    public void OnPressedHost()
    {
        ChangeSceneMenu("HostMenu");
    }

    public void OnPressedJoin()
    {
        InstanceFinder.ClientManager.StartConnection();
        //Récupération de l'id de la map puis chargement.
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

    //Host Menu

    public void MapSelection()
    {

    }

}
