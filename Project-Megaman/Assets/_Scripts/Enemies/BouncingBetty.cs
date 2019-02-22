using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class BouncingBetty : Enemy
    {
        [SerializeField] private Vector2 launchForce;
        [SerializeField] private float destructionVelocityThreshold = 0.1f; //The speed at below which the bouncing betty will self destruct.

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        //Handle off screen as normal.
        //Handle death: Start respawn timer.

        protected override IEnumerator EnemyBehaviour()
        {
            rb2d.AddForce(launchForce);

            while (true)
            {
                if(rb2d.velocity.magnitude < destructionVelocityThreshold) {
                    Kill();
                }
                yield return null;
            }
        }
    }
}

