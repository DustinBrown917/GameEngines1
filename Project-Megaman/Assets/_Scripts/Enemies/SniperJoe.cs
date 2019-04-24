using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class SniperJoe : Enemy
    {

        [SerializeField] private float playerDistanceThreshold;
        [SerializeField] private Vector2 jumpVelocity;
        [SerializeField] private Vector2 pursuitMaxVelocity;
        [SerializeField] private Vector2 projectileOffset;
        [SerializeField] private Vector2 projectileSpeed;
        [SerializeField] private Collider2D groundCheckCollider;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private float shootWaitTime;
        [SerializeField] private float waitWaitTime;

        private WaitForSeconds wfs;
        private bool isGrounded;
        private bool canTakeDamage = true;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            GetIsGrounded();
            if (isGrounded) { rb2d.velocity = new Vector2(); }
        }

        protected override IEnumerator EnemyBehaviour()
        {
            while (true)
            {
                GetDirectionToPlayer();
                JoeActions act = DecideOnAction();
                switch (act)
                {
                    case JoeActions.APPROACH:
                        Approach();
                        while (!isGrounded) { yield return null; }
                        //yield return new WaitForSeconds(waitWaitTime);
                        break;
                    case JoeActions.WAIT:
                        Wait();
                        yield return wfs;
                        canTakeDamage = true;
                        break;
                    case JoeActions.ATTACK:
                        Attack();
                        yield return wfs;
                        animator.SetBool("isShooting", false);
                        break;
                    case JoeActions.JUMP:
                        Jump(jumpVelocity);
                        while (!isGrounded) { yield return null; }
                        break;
                    default:
                        yield return null;
                        break;
                }
                
                yield return null;
            }
        }

        public JoeActions DecideOnAction()
        {
            if(Vector2.Distance(PlayerController.Instance.transform.position, transform.position) > playerDistanceThreshold)
            {
                return JoeActions.APPROACH;
            }

            return (JoeActions)UnityEngine.Random.Range(1, 4); //Joe only has four possible actions, hence the magic number. Could get the number of enum value by converting to array, but unnecessary here.
        }

        public void Approach()
        {
            //Approach the player by jumping towards them.
            float distanceFactor = (Vector2.Distance(PlayerController.Instance.transform.position, transform.position) / playerDistanceThreshold) * GetDirectionToPlayer();
            Vector2 modifiedPursuitVel = pursuitMaxVelocity;
            modifiedPursuitVel.x *= distanceFactor;
            Jump(modifiedPursuitVel);
        }

        public void Wait()
        {
            //Raise shield and do nothing.
            canTakeDamage = false;
            wfs = new WaitForSeconds(waitWaitTime);
        }

        public void Attack()
        {
            //Shoot at the player
            animator.SetBool("isShooting", true);
            FireProjectile();
            wfs = new WaitForSeconds(shootWaitTime);
        }

        public void Jump(Vector2 impulse)
        {
            if (!isGrounded) { return; }
            Debug.Log(impulse);
            rb2d.AddForce(impulse);
            isGrounded = false;
            animator.SetBool("isGrounded", isGrounded);
        }

        public float GetDirectionToPlayer()
        {
            float flipFactor = (PlayerController.Instance.transform.position.x - transform.position.x < 0) ? -1.0f : 1.0f;

            if(flipFactor < 0) {
                spriteRenderer.flipX = false;
            } else if (flipFactor > 0) {
                spriteRenderer.flipX = true;
            }

            if(spriteRenderer.flipX && projectileOffset.x < 0) {
                projectileOffset.x *= -1;
                projectileSpeed.x *= -1;
            } else if (!spriteRenderer.flipX && projectileOffset.x > 0) {
                projectileOffset.x *= -1;
                projectileSpeed.x *= -1;
            }
            
            return flipFactor;
        }

        public bool GetIsGrounded()
        {
            isGrounded = groundCheckCollider.IsTouchingLayers(whatIsGround);
            animator.SetBool("isGrounded", isGrounded);
            return isGrounded;
        }

        protected override void FireProjectile()
        {
            base.FireProjectile();
            Instantiate(projectile, transform.position + (Vector3)projectileOffset, Quaternion.identity).GetComponent<Projectile>().SetLaunchVelocity(projectileSpeed);
        }

        public override void Damage(float amount)
        {
            if (!canTakeDamage) { return; }
            base.Damage(amount);
        }

        public enum JoeActions
        {
            APPROACH = 0,
            WAIT,
            ATTACK,
            JUMP
        }
    }
}

