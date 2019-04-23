using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class OctopusBattery : Enemy
    {

        [SerializeField] private Vector3 leftMostPos;
        [SerializeField] private Vector3 rightMostPos;
        [SerializeField] private float speed;
        [SerializeField] private float waitTime;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(leftMostPos + transform.position, 0.5f);
            Gizmos.DrawWireSphere(rightMostPos + transform.position, 0.5f);
        }

        protected override IEnumerator EnemyBehaviour()
        {
            WaitForSeconds wfs = new WaitForSeconds(waitTime);
            float currentVelocity = speed;
            Vector3 newPos = transform.position;
            while (true)
            {
                newPos.x += currentVelocity * Time.deltaTime;
                if (newPos.x >= rightMostPos.x + spawnPosition.x || newPos.x <= spawnPosition.x + leftMostPos.x) {
                    currentVelocity *= -1.0f;
                    animator.SetTrigger("closeEye");
                    yield return wfs;
                    animator.SetTrigger("openEye");
                }
                transform.position = newPos;
                yield return null;
            }
        }
    }
}

