using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class SpringThing : Enemy
    {
        [SerializeField] private Collider2D groundCheckCollider;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Vector2 jumpForce;
        private bool isGrounded = true;

        protected override void Start()
        {
            base.Start();
            GetIsGrounded();
        }

        protected override IEnumerator EnemyBehaviour()
        {
            float jumpTimer = 1.0f;
            while (true)
            {
                if(isGrounded && jumpTimer <= 0)
                {
                    rb2d.velocity = jumpForce;
                    jumpTimer = 1.0f;
                } else if(isGrounded && jumpTimer > 0)
                {
                    jumpTimer -= Time.deltaTime;
                } else
                {
                    jumpTimer = 1.0f;
                }
                yield return null;
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
            GetIsGrounded();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            GetIsGrounded();
        }

        public bool GetIsGrounded()
        {
            isGrounded = groundCheckCollider.IsTouchingLayers(whatIsGround);
            animator.SetBool("isGrounded", isGrounded);
            return isGrounded;
        }
    }
}

