using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class ParticleManager : MonoBehaviour
    {
        private static ParticleManager instance_ = null;
        public static ParticleManager Instance { get { return instance_; } }
        [SerializeField] private GameObject[] particlePrefabs;
        public int NumOfParticleBlasts { get { return particlePrefabs.Length; } }


        private void Awake()
        {
            if(instance_ == null) //I'm literally writing this singleton as you're telling us why they are bad. I'm sorry! I would never do this on a team, but its a quick and dirty way to get the job done solo.
            {
                instance_ = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if(instance_ == this) { instance_ = null; }
        }

        public void CreateParticleExplosion(int index, Vector3 location)
        {
            if(index < 0 || index >= particlePrefabs.Length) { return; }
            ParticleBlast pb = Instantiate(particlePrefabs[index], location, Quaternion.identity).GetComponent<ParticleBlast>();
            pb.Fire();
        }
    }
}

