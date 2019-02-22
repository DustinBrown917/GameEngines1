using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class BouncingBettySpawner : MonoBehaviour
    {
        [SerializeField] private Vector3 checkIfOnscreenPoint; //Where does the spawner check to see if the betty can be launched.
        [SerializeField] private Vector3 deploymentOffset; //Where should the spawn point move to to trigger the betty launch.
        [SerializeField] private GameObject bouncingBettyPrefab;
        [SerializeField] private float spawnCheckWaitSeconds;
        [SerializeField] private float spawnInterval;
        private BouncingBetty targetBouncingBetty;

        private Coroutine cr_CurrentAction;

        private void Awake()
        {
            targetBouncingBetty = Instantiate(bouncingBettyPrefab, transform).GetComponent<BouncingBetty>();
            targetBouncingBetty.SetSpawnParent(transform);
            targetBouncingBetty.SpawnedIn += TargetBouncingBetty_SpawnedIn;
            targetBouncingBetty.IReset();
            
        }

        private void OnEnable()
        {
            CoroutineManager.BeginCoroutine(CheckIfShouldSpawn(), ref cr_CurrentAction, this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + checkIfOnscreenPoint, 0.2f);
        }

        private void OnDisable()
        {
            CoroutineManager.HaltCoroutine(ref cr_CurrentAction, this);
        }

        private void TargetBouncingBetty_SpawnedIn(object sender, System.EventArgs e)
        {
            
        }

        private bool IsSpawnCheckOnScreen()
        {
            if(GameManager.MainCamera == null) { return false; }
            bool onScreen = false;
            Vector2 point = GameManager.MainCamera.WorldToViewportPoint(transform.position + checkIfOnscreenPoint);
            point *= point;
            
            if(point.x <= 1.0f && point.y <= 1.0f) { onScreen = true; }

            return onScreen;
        }

        private IEnumerator CheckIfShouldSpawn()
        {
            WaitForSeconds wfs = new WaitForSeconds(spawnCheckWaitSeconds);
            while (!IsSpawnCheckOnScreen())
            {
                yield return wfs;
            }

            CoroutineManager.BeginCoroutine(BettyLaunchTimer(), ref cr_CurrentAction, this);
        }

        private IEnumerator BettyLaunchTimer()
        {
            WaitForSeconds wfs = new WaitForSeconds(spawnInterval);
            do
            {
                Debug.Log("Spawning betty");
                transform.position += deploymentOffset;
                yield return null;
                transform.position -= deploymentOffset;
                yield return wfs;
            } while (IsSpawnCheckOnScreen());

            CoroutineManager.BeginCoroutine(CheckIfShouldSpawn(), ref cr_CurrentAction, this);
        }
    }
}

