using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MapPrefabsItem : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text textName;
    public TMP_Text textDescription;
    public Button buttonSelect;
    public Image image;
    public string loadName;

    public GameObject selectedUi;

    MapToLoad data;
    public void InitMapItem(string name, string desc, Sprite sprite, string loadName, MapToLoad data )
    {
        this.data = data;
        this.loadName = loadName;
        textName.text = name;
        textDescription.text = desc;
        if(sprite != null)
            image.sprite = sprite;
    }

    public void BindButton()
    {
        selectedUi.SetActive(selectedUi.activeSelf);
    }

}
