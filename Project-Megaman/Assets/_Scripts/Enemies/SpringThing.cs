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
        [SerializeField] private float maxJumpAngle;
        [SerializeField] private float minJumpAngle;
        [SerializeField] private float maxDistanceThreshold;
        private bool isGrounded = true;

        protected override void Start()
        {
            base.Start();
            GetIsGrounded();
        }

        protected override IEnumerator EnemyBehaviour()
        {
            float jumpTimer = 0.0f;
            while (true)
            {
                if(isGrounded && jumpTimer <= 0) {
                    rb2d.velocity = Vector2FromAngle(jumpForce, GetPlayerApproachAngle());
                    jumpTimer = 1.0f;
                } else if(isGrounded && jumpTimer > 0)
                {
                    jumpTimer -= Time.deltaTime;
                } else {
                    jumpTimer = 1.0f;
                }
                yield return null;
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            GetIsGrounded();
            if (isGrounded) { rb2d.velocity = new Vector2(0, rb2d.velocity.y); }
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

        public float GetPlayerApproachAngle()
        {
            float t = (transform.position.x - PlayerController.Instance.transform.position.x)/maxDistanceThreshold;

            t *= (t < 0) ? -1 : 1;

            float angle = Mathf.Lerp(minJumpAngle, maxJumpAngle, t);


            if(PlayerController.Instance.transform.position.x > transform.position.x) { angle *= -1; }
            return angle;
        }
    }
}

