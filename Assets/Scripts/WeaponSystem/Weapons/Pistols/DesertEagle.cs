using UnityEngine;

namespace WeaponSystem
{
    public class DesertEagle : Weapon
    {
        public override void Fire()
        {
            Debug.Log("DesertEagle.Fire() triggered.");
            Instantiate(this.Bullet, Vector3.forward, new Quaternion(0f, 0f, 0f, 0f));
        }
    }
}
