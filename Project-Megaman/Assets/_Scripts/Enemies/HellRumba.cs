using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class HellRumba : Enemy
    {
        [SerializeField] private Vector3 leftMostPos;
        [SerializeField] private Vector3 rightMostPos;
        [SerializeField] private float speed;

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

        // Update is called once per frame
        void Update()
        {

        }

        protected override IEnumerator EnemyBehaviour()
        {
            float currentVelocity = speed;
            Vector3 newPos = transform.position;
            while (true)
            {
                newPos.x += currentVelocity * Time.deltaTime;
                if(newPos.x >= rightMostPos.x + spawnPosition.x|| newPos.x <= spawnPosition.x + leftMostPos.x) { currentVelocity *= -1.0f; }
                transform.position = newPos;
                yield return null;
            }
        }
    }
}

