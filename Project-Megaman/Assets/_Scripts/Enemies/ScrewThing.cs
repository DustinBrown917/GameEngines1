using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class ScrewThing : Enemy
    {
        [SerializeField] private float projectileSpeed = 10.0f;
        [SerializeField] private Vector3 projectileOffset;
        [SerializeField] private float targetingDistance = 10.0f;
        private bool hasTarget = false;
        [SerializeField] private float checkForPlayerInterval = 0.5f;

        protected override void Start()
        {
            base.Start();
        }

        protected override IEnumerator EnemyBehaviour()
        {
            WaitForSeconds wfs = new WaitForSeconds(checkForPlayerInterval);
            while (true)
            {
                if(Vector2.Distance(transform.position, PlayerController.Instance.transform.position) <= targetingDistance) {
                    SetHasTarget(true);
                } else {
                    SetHasTarget(false);
                }
                yield return wfs;
            }
        }

        public void SetHasTarget(bool b)
        {
            if(b == hasTarget) { return; }
            hasTarget = b;
            animator.SetBool("HasTarget", hasTarget);
        }


        protected override void FireProjectile()
        {
            base.FireProjectile();
            Instantiate(projectile, transform.position + projectileOffset, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(new Vector2(0, projectileSpeed));
            Instantiate(projectile, transform.position + projectileOffset, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(new Vector2(projectileSpeed, 0.0f));
            Instantiate(projectile, transform.position + projectileOffset, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(new Vector2(-projectileSpeed, 0.0f));
            Instantiate(projectile, transform.position + projectileOffset, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(Vector2.ClampMagnitude(new Vector2(projectileSpeed, projectileSpeed), projectileSpeed));
            Instantiate(projectile, transform.position + projectileOffset, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(Vector2.ClampMagnitude(new Vector2(-projectileSpeed, projectileSpeed), projectileSpeed));

        }

    }
}

