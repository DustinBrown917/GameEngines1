using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class EnergyPickup : Pickup
    {
        [SerializeField] private float energyRestored;

        public override void OnPickup(GameObject whoPickedMeUp)
        {
            IEnergyObject eo = whoPickedMeUp.GetComponent<IEnergyObject>();

            if (eo != null) {
                eo.GainEnergy(energyRestored);
            }
        }
    }
}

