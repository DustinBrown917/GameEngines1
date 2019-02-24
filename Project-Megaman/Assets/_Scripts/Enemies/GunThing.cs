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
            float flipFactor = (spriteRenderer.flipX) ? 1.0f : -1.0f;
            Vector2 projVel = new Vector2(projectileVelocity.x * flipFactor, projectileVelocity.y);
            Instantiate(projectile, transform.position + (projectileOffset * flipFactor), Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(projVel);
            
        }

        protected override IEnumerator EnemyBehaviour()
        {
            yield return null; //This particular behaviour handles everything via animation at the moment.
        }

        public override void IReset()
        {
            base.IReset();
            animator.Play("GunThing_Idle");
        }
    }
}

