using UnityEngine;

namespace WeaponSystem
{
    public abstract class Weapon : MonoBehaviour
    {
        public Bullet Bullet;
        public Magazine Magazine;

        public void Start()
        {
            for (int i = 0; i < Magazine.CurrentCapacity; i++)
            {
                //Instantiate(this.Bullet, new Vector3(0f, 0f, 0f), Quaternion.identity);
            }
        }

        public abstract void Fire();
    }
}
