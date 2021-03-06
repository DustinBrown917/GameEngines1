﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class Spikes : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            IHealthyObject ho = collision.GetComponent<IHealthyObject>();
            if (ho != null)
            {
                ho.Kill(true);
            }
        }
    }
}

