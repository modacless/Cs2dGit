using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public abstract class MenuTemple : NetworkBehaviour
{
    public List<GameObject> sceneMenu;
    public Dictionary<string, GameObject> sceneMenuSort = new Dictionary<string, GameObject>();

    public virtual void Start()
    {
        foreach (GameObject go in sceneMenu)
        {
            sceneMenuSort.Add(go.name, go);
        }

    }

    protected void ChangeSceneMenu(string nameOfScene)
    {
        foreach (string name in sceneMenuSort.Keys)
        {
            if (nameOfScene == name)
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
