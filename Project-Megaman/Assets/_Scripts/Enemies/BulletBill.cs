using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class BulletBill : Enemy
    {
        [SerializeField] private Vector2 velocity;
        [SerializeField] private float bobFrequency;

        [SerializeField] private bool delayedStart;
        [SerializeField] private float delaySeconds;
        private float hideZ = 10.0f;
        private float showZ;
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            showZ = transform.position.z;
        }

        protected override IEnumerator EnemyBehaviour()
        {
            if (delayedStart)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, hideZ);
                yield return new WaitForSeconds(delaySeconds);
                transform.position = new Vector3(transform.position.x, transform.position.y, showZ);
            }

            Vector3 newPos = transform.position;
            newPos.y = PlayerController.Instance.transform.position.y;
            while (true)
            {
                newPos.x += velocity.x * Time.deltaTime;
                newPos.y += Mathf.Sin(Time.time * bobFrequency) * velocity.y * Time.deltaTime;
                transform.position = newPos;
                yield return null;
            }
        }

    }
}

