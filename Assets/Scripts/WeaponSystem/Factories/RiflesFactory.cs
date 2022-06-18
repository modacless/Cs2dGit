using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    public class RiflesFactory : WeaponFactory
    {
        public enum WeaponType { Unknown = 0 };

        public override Weapon GetWeapon(string WeaponType)
        {
            return new DesertEagle();
        }
    }
}
