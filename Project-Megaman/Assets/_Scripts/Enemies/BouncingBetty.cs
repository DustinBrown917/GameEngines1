using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class BouncingBetty : Enemy
    {
        [SerializeField] private Vector2 launchForce;
        [SerializeField] private float destructionVelocityThreshold = 0.1f; //The speed at below which the bouncing betty will self destruct.
        [SerializeField] private float timeBetweenSpawns = 3.0f;
        [SerializeField] private Vector2 projectileSpeed1;
        [SerializeField] private Vector2 projectileSpeed2;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        //Handle off screen as normal.
        //Handle death: Start respawn timer.

        protected override IEnumerator EnemyBehaviour()
        {
            rb2d.velocity = new Vector2();
            rb2d.AddForce(launchForce);
            
            while (true)
            {
                if (rb2d.velocity.y < 0.0f)
                {
                    FireProjectile();
                    Kill(true);
                }
                yield return null;
            }
        }

        protected override IEnumerator SafeToSpawnChecker()
        {
            transform.position = offScreenHoldingPosition;
            yield return new WaitForSeconds(timeBetweenSpawns);
            IReset();
        }

        protected override void FireProjectile()
        {
            Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(projectileSpeed1);
            Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(projectileSpeed2);
            Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(new Vector2(projectileSpeed1.x * -1.0f, projectileSpeed1.y));
            Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(new Vector2(projectileSpeed2.x * -1.0f, projectileSpeed2.y));
        }
    }
}

