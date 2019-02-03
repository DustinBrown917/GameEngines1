using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MEGA
{
    public class CameraFollow : MonoBehaviour
    {
        private static CameraFollow instance_ = null;
        public static CameraFollow Instance { get { return instance_; } }

        private const float Z_POS = -10.0f;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform followTarget;
        [SerializeField] private bool followX;
        [SerializeField] private bool followY;
        [SerializeField] private CameraTransitionSection[] transitionSections;
        private int currentTransitionSectionIndex = 0;
        private Vector3 cachedPosition;

        private void Awake()
        {
            if (instance_ == null) {
                instance_ = this;
                SetTransitionSection(currentTransitionSectionIndex);
            }
            else {
                Destroy(this.gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if (transitionSections[currentTransitionSectionIndex].UseXRail) {
                cachedPosition.x = transitionSections[currentTransitionSectionIndex].TransitionRail.x;
            } else {
                cachedPosition.x = followTarget.position.x;
            }

            if (transitionSections[currentTransitionSectionIndex].UseYRail) {
                cachedPosition.y = transitionSections[currentTransitionSectionIndex].TransitionRail.y;
            }
            else {
                cachedPosition.y = followTarget.position.y;
            }

            cachedPosition.x = Mathf.Clamp(cachedPosition.x, transitionSections[currentTransitionSectionIndex].MinAnchor.x, transitionSections[currentTransitionSectionIndex].MaxAnchor.x);
            cachedPosition.y = Mathf.Clamp(cachedPosition.y, transitionSections[currentTransitionSectionIndex].MinAnchor.y, transitionSections[currentTransitionSectionIndex].MaxAnchor.y);

            cachedPosition.z = Z_POS;
            cameraTransform.position = cachedPosition;
        }

        private void OnDestroy()
        {
            if (instance_ == this)
            {
                instance_ = null;
            }
        }

        public void SetTransitionSection(int index)
        {
            if(index < 0 || index >= transitionSections.Length) { return; }

            transitionSections[currentTransitionSectionIndex].Transition -= TransitionSection_Transition;

            transitionSections[index].Transition += TransitionSection_Transition;

            currentTransitionSectionIndex = index;
        }

        private void TransitionSection_Transition(object sender, CameraTransitionSection.TransitionArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

