using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MEGA
{
    public class CameraTransitionSection : MonoBehaviour
    {
        //What rail should the camera use in this section.
        [SerializeField] private Vector2 transitionRail_;
        public Vector2 TransitionRail { get { return transitionRail_; } }
        [SerializeField] private bool useXRail_;
        public bool UseXRail { get { return useXRail_; } }
        [SerializeField] private bool useYRail_;
        public bool UseYRail { get { return useYRail_; } }

        //Where should the player move to upon entering the section.
        [SerializeField] private Vector2 playerEntrancePoint_;
        public Vector2 PlayerEntrancePoint { get { return transform.position + (Vector3)playerEntrancePoint_; } }
        [SerializeField] private bool usePlayerEntranceTransitionX_;
        public bool UsePlayerEntranceTransitionX { get { return usePlayerEntranceTransitionX_; } }
        [SerializeField] private bool usePlayerEntranceTransitionY_;
        public bool UsePlayerEntranceTransitionY { get { return usePlayerEntranceTransitionY_; } }

        //Where should the player move to upon returning to the section.
        [SerializeField] private Vector2 playerExitPoint_;
        public Vector2 PlayerExitPoint { get { return transform.position + (Vector3)playerExitPoint_; } }
        [SerializeField] private bool usePlayerExitTransitionX_;
        public bool UsePlayerExitTransitionX { get { return usePlayerExitTransitionX_; } }
        [SerializeField] private bool usePlayerExitTransitionY_;
        public bool UsePlayerExitTransitionY { get { return usePlayerExitTransitionY_; } }


        [SerializeField] private Vector2 minAnchor_;
        public Vector2 MinAnchor { get { return transform.position + (Vector3)minAnchor_; } }
        [SerializeField] private Vector2 maxAnchor_;
        public Vector2 MaxAnchor { get { return transform.position + (Vector3)maxAnchor_; } }
        [SerializeField] private Vector2 cameraDimensions;

        private Coroutine cr_Transition = null;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + (Vector3)playerEntrancePoint_, 1.0f);
            Gizmos.DrawWireSphere(transform.position + (Vector3)playerExitPoint_, 1.0f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + (Vector3)minAnchor_, cameraDimensions);
            Gizmos.DrawWireCube(transform.position + (Vector3)maxAnchor_, cameraDimensions);
        }

        public void TransitionToNext()
        {
            OnTransition(new TransitionArgs(CameraTransitionType.EXIT));
        } 

        public void TransitionToPrevious()
        {
            OnTransition(new TransitionArgs(CameraTransitionType.ENTRANCE));
        }


        #region Transition Event
        public event EventHandler<TransitionArgs> Transition;

        public class TransitionArgs : EventArgs
        {
            public CameraTransitionType transitionType;

            public TransitionArgs(CameraTransitionType t)
            {
                transitionType = t;
            }
        }

        private void OnTransition(TransitionArgs e)
        {
            EventHandler<TransitionArgs> handler = Transition;

            if(handler != null) {
                handler(this, e);
            }
        }

        #endregion
    }

    public enum CameraTransitionType
    {
        ENTRANCE,
        EXIT
    }
}

