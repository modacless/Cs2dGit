using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Data",fileName ="Data",order = 0)]
public class ScriptablePlayerData : ScriptableObject
{
    [Header("Input")]
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode upKey;
    public KeyCode downKey;

    public float speed;
}
