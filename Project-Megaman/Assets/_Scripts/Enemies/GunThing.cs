using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class GunThing : Enemy
    {

        private bool isClosed = true;
        [SerializeField] private Vector3 projectileOffset;
        [SerializeField] private Vector2 projectileVelocity;

        [SerializeField] private float initialShotAngle = 0.0f;
        [SerializeField] private float shotAngleIncrement = 15.0f;
        private int shotCount = 0;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Damage(float amount)
        {
            if (isClosed) { return; }
            base.Damage(amount);
        }

        private void Close()
        {
            isClosed = true;
        }

        private void Open()
        {
            isClosed = false;
        }

        protected override void FireProjectile()
        {
            base.FireProjectile();
            float flipFactor = (spriteRenderer.flipX) ? 1.0f : -1.0f;
            Vector2 projVel = new Vector2(projectileVelocity.x * flipFactor, projectileVelocity.y);
            projVel = Vector2FromAngle(projVel, initialShotAngle + (shotAngleIncrement * shotCount));
            Instantiate(projectile, transform.position + (projectileOffset * flipFactor), Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(projVel);
            shotCount++;
            if (shotCount == 4) { shotCount = 0; }
        }

        protected override IEnumerator EnemyBehaviour()
        {
            yield return null; //This particular behaviour handles everything via animation at the moment.
        }

        public override void IReset()
        {
            base.IReset();
            animator.Play("GunThing_Idle");
            shotCount = 0;
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

