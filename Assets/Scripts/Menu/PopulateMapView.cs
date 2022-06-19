using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapToLoad
{
    public string name;
    public string description;
    public Sprite image;
    public string loadName;
}

public class PopulateMapView : MonoBehaviour
{
    public MapToLoad[] maps;
    public GameObject prefabsToAdd;
    public RectTransform contentContainer;

    void Start()
    {
        for(int i = 0; i < maps.Length; i++)
        {
            GameObject obj = Instantiate(prefabsToAdd);
            prefabsToAdd.GetComponent<MapPrefabsItem>().InitMapItem(maps[i].name, maps[i].description, maps[i].image, maps[i].loadName,maps[i]);
            obj.transform.SetParent(contentContainer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
