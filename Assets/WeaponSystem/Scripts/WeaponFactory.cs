using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    public abstract class WeaponFactory
    {
        public List<GameObject> Weapons;

        public abstract void Create();
    }

}
