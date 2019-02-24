using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] protected bool impactAnim = false;
        [SerializeField] protected float damage = 1;
        private Rigidbody2D rb2d;
        [SerializeField] private Vector2 launchVelocity_;
        public Vector2 LaunchVelocity { get { return launchVelocity_; } }
        private Animator animator;

        private void Awake()
        {
            rb2d = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            rb2d.velocity = launchVelocity_;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            IHealthyObject ho = collision.gameObject.GetComponent<IHealthyObject>();
            if (ho != null)
            {
                ho.Damage(damage);
            }

            HandleImpact();

        }


        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }

        public void SetLaunchVelocity(Vector2 vel)
        {
            launchVelocity_ = vel;
        }

        protected void HandleImpact()
        {
            if (impactAnim) {
                animator.SetTrigger("Impact");
                rb2d.simulated = false;
            } else {
                Destroy(gameObject);
            }
        }

        protected void DefferedSelfDestruct() //Wrapper for use during animation.
        {
            Destroy(gameObject);
        }
    }
}

