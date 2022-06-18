using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    public class PistolsFactory : WeaponFactory
    {
        public enum WeaponType { DesertEagle = 0, FiveSeven = 1, DualBerettas = 2 };

        public override Weapon GetWeapon(string WeaponType)
        {
            switch (WeaponType)
            {
                case "DesertEagle":
                    return new DesertEagle();
                case "FiveSeven":
                    return new FiveSeven();
                case "DualBerettas":
                    return new DualBerettas();
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
