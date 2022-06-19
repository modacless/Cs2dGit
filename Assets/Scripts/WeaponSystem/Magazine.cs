using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    public struct Magazine
    {
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }

        public Magazine(int capacity)
        {
            Capacity = capacity;
            CurrentCapacity = capacity;
        }
    }
}
