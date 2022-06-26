using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MapPrefabsItem : MonoBehaviour
{
    //Data show by gameObject
    public TMP_Text textName;
    public TMP_Text textDescription;
    public Button buttonSelect;
    public Image image;
    public string loadName;
    public GameObject selectedUi;

    public PopulateMapView mapView;

    //Usefull data 
    MapToLoad data;

    public void InitMapItem(string name, string desc, Sprite sprite, string loadName, MapToLoad data, PopulateMapView view  )
    {
        selectedUi.SetActive(false);
        this.data = data;
        this.loadName = loadName;
        mapView = view;
        textName.text = name;
        textDescription.text = desc;
        if(sprite != null)
            image.sprite = sprite;
    }

    public MapToLoad Select()
    {
        selectedUi.SetActive(true);
        return data;
    }

    public void UnSelect()
    {
        selectedUi.SetActive(false);
    }

    public void OnPressedSelect()
    {
        mapView.SelectMap(this);
    }

}
