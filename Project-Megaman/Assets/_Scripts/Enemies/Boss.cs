using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class Boss : MonoBehaviour
    {

        private void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerController p = collision.gameObject.GetComponent<PlayerController>();

            if (p != null)
            {
                p.Damage(1000);
            }
        }
    }
}

