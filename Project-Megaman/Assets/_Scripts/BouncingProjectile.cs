using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class BouncingProjectile : Projectile
    {
        [SerializeField] private float duration = 2.0f;
        private float elapsedTime = 0.0f;
        private bool pendingDestruction = false;

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            if (pendingDestruction) { return; }

            if(elapsedTime >= duration)
            {
                HandleImpact();
                pendingDestruction = true;
            }
            elapsedTime += Time.deltaTime;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            IHealthyObject ho = collision.gameObject.GetComponent<IHealthyObject>();
            if (ho != null)
            {
                ho.Damage(damage);
                HandleImpact();
            } 
        }
    }
}

