using UnityEngine;

namespace WeaponSystem
{
    public class Bullet : MonoBehaviour
    {
        public int LifeTime { get; set; } = 3;
        public float Speed { get; set; } = 10f;

        private void Start()
        {
            Destroy(gameObject, this.LifeTime);
        }

        private void Update()
        {
            transform.Translate(Speed * Time.deltaTime * Vector3.right);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Destroy(gameObject);
        }
    }

}
