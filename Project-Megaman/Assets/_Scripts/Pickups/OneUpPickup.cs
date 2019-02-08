using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class OneUpPickup : Pickup
    {
        public override void OnPickup(GameObject whoPickedMeUp)
        {
            Debug.Log("Extra Life!");
        }
    }
}

