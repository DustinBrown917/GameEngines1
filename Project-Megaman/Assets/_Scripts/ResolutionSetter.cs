using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class ResolutionSetter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Screen.SetResolution(1024, 1024, false);
        }
    }
}


