using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float damage = 1;
        private Rigidbody2D rb2d;
        public Vector2 launchVelocity;

        private void Awake()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        void Start()
        {
            rb2d.velocity = launchVelocity;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            IHealthyObject ho = collision.gameObject.GetComponent<IHealthyObject>();
            if(ho != null)
            {
                ho.Damage(damage);
            }

            Destroy(gameObject);
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}

