using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    public class DesertEagle : Weapon
    {
        public override void Fire()
        {
            Debug.Log("DesertEagle.Fire() triggered.");
        }
    }
}
