using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace MEGA
{
    public class CameraFollow : MonoBehaviour, IResettable
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
        private Coroutine cr_Transition = null;
        public bool IsTransitioning { get { return cr_Transition != null; } }

        private void Awake()
        {
            if (instance_ == null) {
                instance_ = this;
                SetTransitionSection(currentTransitionSectionIndex);
                Register();
            }
            else {
                Destroy(this.gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (IsTransitioning) { return; }
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

            if(transitionSections[currentTransitionSectionIndex].MinAnchor.y <= transitionSections[currentTransitionSectionIndex].MaxAnchor.y) {
                cachedPosition.y = Mathf.Clamp(cachedPosition.y, transitionSections[currentTransitionSectionIndex].MinAnchor.y, transitionSections[currentTransitionSectionIndex].MaxAnchor.y);
            } else {
                cachedPosition.y = Mathf.Clamp(cachedPosition.y, transitionSections[currentTransitionSectionIndex].MaxAnchor.y, transitionSections[currentTransitionSectionIndex].MinAnchor.y);
            }
            

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
            if (IsTransitioning) { return; }
            switch (e.transitionType)
            {
                case CameraTransitionType.ENTRANCE:
                    SetTransitionSection(currentTransitionSectionIndex - 1);
                    break;
                case CameraTransitionType.EXIT:
                    SetTransitionSection(currentTransitionSectionIndex + 1);
                    break;
                default:
                    break;
            }
            CoroutineManager.BeginCoroutine(Transition(e.transitionType), ref cr_Transition, this);
        }

        private IEnumerator Transition(CameraTransitionType transitionType)
        {
            
            float transitionDuration = 1.0f;
            float t = 0;
            CameraTransitionSection targetSection = transitionSections[currentTransitionSectionIndex];

            Vector3 cStartPos = cameraTransform.position;
            Vector3 newCPos = cStartPos;           
            Vector3 cTargetPos;

            Vector3 ftStartPos = followTarget.position;
            Vector3 newFtPos = ftStartPos;
            Vector3 ftTargetPos;

            if (transitionType == CameraTransitionType.EXIT) {
                cTargetPos = targetSection.MinAnchor;
                cTargetPos.z = cStartPos.z;

                ftTargetPos = targetSection.PlayerEntrancePoint;
                ftTargetPos.z = ftStartPos.z;
            }
            else {
                cTargetPos = targetSection.MaxAnchor;
                cTargetPos.z = cStartPos.z;

                ftTargetPos = targetSection.PlayerExitPoint;
                ftTargetPos.z = ftStartPos.z;
            }

            Time.timeScale = 0;
            while (t < transitionDuration)
            {             
                if (!GameManager.Instance.Paused) { t += Time.unscaledDeltaTime; }                
                newCPos = Vector3.Lerp(cStartPos, cTargetPos, t/transitionDuration);
                newFtPos = Vector3.Lerp(ftStartPos, ftTargetPos, t / transitionDuration);
                cameraTransform.position = newCPos;
                followTarget.position = newFtPos;
                yield return null;
            }

            newCPos = cTargetPos;
            cameraTransform.position = newCPos;

            newFtPos = ftTargetPos;
            followTarget.position = newFtPos;

            Time.timeScale = 1.0f;
            cr_Transition = null;
        }

        public void Register()
        {
            ResetManager.AddResettable(this);
        }

        public void IReset()
        {
            SetTransitionSection(0);
        }
    }
}

