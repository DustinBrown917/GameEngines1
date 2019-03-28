using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MEGA
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance_ = null;
        public static GameManager Instance { get { return instance_; } }

        private static Camera mainCamera_ = null;
        public static Camera MainCamera { get { return mainCamera_; } }

        [SerializeField] private Camera targetMainCamera;
        [SerializeField] private float delayAfterDeath = 4.0f;
        [SerializeField] private string gameOverSceneName;
        private Coroutine cr_delayAfterDeathTimer;

        [SerializeField] private GameObject pauseScreen;

        private bool paused_ = false;
        public bool Paused { get { return paused_; } }

        private void Awake()
        {
            if(instance_ == null) {
                instance_ = this;
                mainCamera_ = targetMainCamera;
            } else {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerController.Instance.Death += Player_Death;
        }

        private void Update()
        {
            if (Input.GetButtonDown("Pause")) { SetPause(!Paused); }
        }

        private void OnDestroy()
        {
            if (instance_ == this)
            {
                ResetManager.Reinitialize();
                mainCamera_ = null;
                instance_ = null;
            }
        }

        private void Player_Death(object sender, System.EventArgs e)
        {
            CoroutineManager.BeginCoroutine(DelayAfterDeathTimer(delayAfterDeath), ref cr_delayAfterDeathTimer, this);
        }

        private IEnumerator DelayAfterDeathTimer(float time)
        {
            yield return new WaitForSeconds(time);
            if(PlayerController.Instance.LivesRemaining <= 0) {
                SceneManager.LoadScene(gameOverSceneName);
            }
            else {
                ResetManager.ResetAll();
            }
            
        }


        public void SetPause(bool pause)
        {
            if (pause == paused_) { return; }
            paused_ = pause;

            if (paused_) { Time.timeScale = 0; }
            else {
                if (!CameraFollow.Instance.IsTransitioning) { Time.timeScale = 1.0f; }
            }

            pauseScreen.SetActive(paused_);
        }
    }
}

