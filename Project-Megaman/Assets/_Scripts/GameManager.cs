using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance_ = null;
        public static GameManager Instance;

        [SerializeField] private float delayAfterDeath = 4.0f;
        private Coroutine cr_delayAfterDeathTimer;

        private void Awake()
        {
            if(instance_ == null) {
                instance_ = this;
            } else {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerController.Instance.Death += Player_Death;
        }

        private void OnDestroy()
        {
            if (instance_ == this)
            {
                ResetManager.Reinitialize();
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
            ResetManager.ResetAll();
            Debug.Log("Resetting all");
        }

    }
}

