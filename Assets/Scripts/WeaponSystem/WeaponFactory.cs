using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    public abstract class WeaponFactory
    {
        public enum Type { Melee = 0, Pistols = 1, Heavy = 2, SMGs = 3, Rifles = 4 };

        public abstract Weapon GetWeapon(string WeaponType);

        public static WeaponFactory Create(Type FactoryType)
        {
            switch (FactoryType)
            {
                case Type.Melee:
                    return new MeleeFactory();
                case Type.Pistols:
                    return new PistolsFactory();
                case Type.Heavy:
                    return new HeavyFactory();
                case Type.SMGs:
                    return new SMGsFactory();
                case Type.Rifles:
                    return new RiflesFactory();
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
