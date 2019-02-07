using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public static class ResetManager
    {
        private static List<IResettable> resettables = new List<IResettable>();

        public static void AddResettable(IResettable resettable)
        {
            if (!resettables.Contains(resettable)) {
                resettables.Add(resettable);
            }           
        }

        public static void RemoveResettable(IResettable resettable)
        {
            resettables.Remove(resettable);
        }

        public static void ResetAll()
        {
            foreach(IResettable r in resettables) {
                r.IReset();
            }
        }

        public static void Reinitialize()
        {
            resettables = new List<IResettable>();
        }
    }
}

