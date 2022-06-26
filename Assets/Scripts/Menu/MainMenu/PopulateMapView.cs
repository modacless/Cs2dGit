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
    public static MapToLoad mapSelected;

    private MapPrefabsItem[] mapsToLoadObjects;
    public GameObject prefabsToAdd;
    public RectTransform contentContainer;

    void Start()
    {
        mapsToLoadObjects = new MapPrefabsItem[maps.Length];
        for (int i = 0; i < maps.Length; i++)
        {
            GameObject obj = Instantiate(prefabsToAdd);
            mapsToLoadObjects[i] = obj.GetComponent<MapPrefabsItem>();
            mapsToLoadObjects[i].InitMapItem(maps[i].name, maps[i].description, maps[i].image, maps[i].loadName,maps[i],this);
            
            obj.transform.SetParent(contentContainer);
        }
    }

    public void UnselectMap()
    {
        foreach(MapPrefabsItem mapPrefabsItem in mapsToLoadObjects)
        {
            mapPrefabsItem.UnSelect();
        }
        if(maps.Length > 0)
        {
            mapSelected = maps[0];
        }
    }

    public void SelectMap(MapPrefabsItem mapToSelect)
    {
        UnselectMap();
        mapSelected = mapToSelect.Select();
        HostSettings.gameSettings.map = mapSelected;
    }
}
