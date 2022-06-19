using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class Player : MonoBehaviour
{
    public Weapon weapon;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) this.weapon.Fire();
    }
}
