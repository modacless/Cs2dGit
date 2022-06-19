using UnityEngine;

namespace WeaponSystem
{
    public class Bullet : MonoBehaviour
    {
        public float Speed { get; set; }

        private void Start()
        {
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            transform.Translate(Vector3.right * 0.25f);
        }
    }
}
