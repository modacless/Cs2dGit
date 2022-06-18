using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSystem;

public class player : MonoBehaviour
{
    public Weapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        this.weapon = WeaponFactory
            .Create(WeaponFactory.Type.Pistols)
            .GetWeapon("FiveSeven");
    }

    // Update is called once per frame
    void Update()
    {
        weapon.Fire();
    }
}
