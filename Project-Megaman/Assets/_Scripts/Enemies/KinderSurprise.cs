using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class KinderSurprise : Enemy
    {
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float fireFrequency;
        [SerializeField] private Vector2 projectileVelocity;
        [SerializeField] private Vector3 projectileOffset;
        private bool shooting;
        private bool isClosed = true;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        protected override IEnumerator EnemyBehaviour()
        {
            WaitForSeconds wfs = new WaitForSeconds(fireFrequency);

            while (true)
            {
                rb2d.velocity = velocity;
                yield return wfs;
                rb2d.velocity = new Vector2(0.0f, 0.0f);

                StartShooting();

                while (shooting) { yield return null; }

            }
        }

        public override void Damage(float amount)
        {
            if (isClosed) { return; }
            base.Damage(amount);
        }

        public void StartShooting()
        {
            isClosed = false;
            shooting = true;
            animator.SetBool("shooting", shooting);
        }

        public void StopShooting()
        {
            isClosed = true;
            shooting = false;
            animator.SetBool("shooting", shooting);
        }

        protected override void FireProjectile()
        {
            //Gonna use magic numbers here purely because I'm trying to match something specific from the game. Otherwise, they would be class variables.

            for(int i = 0; i < 8; i++)
            {
                Vector2 projVel = Vector2FromAngle(projectileVelocity, 45.0f * i);
                Instantiate(projectile, transform.position + projectileOffset, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(projVel);
            }
        }

        public Vector2 Vector2FromAngle(Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);

            return v;
        }
    }
}

