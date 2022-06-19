using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem
{
    public class Weapon : MonoBehaviour
    {
        [Header("Bullet")]
        public GameObject Bullet;
        public Transform BulletSpawn;

        public void Fire()
        {
            Instantiate(Bullet, gameObject.transform.position, Quaternion.identity);
        }
    }
}
