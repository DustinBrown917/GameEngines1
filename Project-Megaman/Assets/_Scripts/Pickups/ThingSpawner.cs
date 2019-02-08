using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class ThingSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject thingToSpawn;
        private GameObject thingToHandle;

        private void Start()
        {
            thingToHandle = Instantiate(thingToSpawn, transform);
        }

        private void OnBecameVisible()
        {
            thingToHandle.SetActive(true);
        }

        private void OnBecameInvisible()
        {
            thingToHandle.SetActive(false);
        }
    }
}

