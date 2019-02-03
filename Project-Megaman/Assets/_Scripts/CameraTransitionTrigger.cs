using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraTransitionTrigger : MonoBehaviour
    {
        [SerializeField] private CameraTransitionSection owner;
        [SerializeField] private TransitionTriggerType triggerType;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerController p = collision.GetComponent<PlayerController>();

            if (p != null)
            {
                switch (triggerType)
                {
                    case TransitionTriggerType.ENTRANCE:
                        owner.TransitionToPrevious();
                        break;
                    case TransitionTriggerType.EXIT:
                        owner.TransitionToNext();
                        break;
                }
            }
        }

        private enum TransitionTriggerType
        {
            EXIT,
            ENTRANCE
        }
    }

    
}

