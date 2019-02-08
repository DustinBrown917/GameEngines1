using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class HealthPickup : Pickup
    {
        [SerializeField] private float healthRestored;

        public override void OnPickup(GameObject whoPickedMeUp)
        {
            IHealthyObject ho = whoPickedMeUp.GetComponent<IHealthyObject>();

            if(ho != null) {
                ho.Heal(healthRestored);
            }
        }
    }
}

