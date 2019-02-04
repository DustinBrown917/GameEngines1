using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class TestDamageBlock : MonoBehaviour
    {
        [SerializeField] private float damageToDeal = 10.0f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerController p = collision.GetComponent<PlayerController>();

            if (p != null)
            {
                p.Damage(damageToDeal);
            }
        }
    }
}

