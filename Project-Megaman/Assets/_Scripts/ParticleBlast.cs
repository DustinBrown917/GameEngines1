using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class ParticleBlast : MonoBehaviour
    {
        private ParticleSystem ps;
        private bool hasFired = false;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
        }

        public void Fire()
        {
            ps.Play();
            hasFired = true;
        }
        // Update is called once per frame
        void Update()
        {
            if (!ps.isPlaying && hasFired) { Destroy(gameObject); }
        }
    }
}

