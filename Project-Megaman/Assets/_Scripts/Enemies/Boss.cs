using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MEGA
{
    public class Boss : MonoBehaviour, IHealthyObject, IResettable
    {
        
        [SerializeField] private float maxHealth;
        private float currentHealth;
        [SerializeField] private string gameOverSceneName;

        private void Start()
        {
            IReset();
        }

        public void Damage(float amount)
        {
            currentHealth -= amount;
            if(currentHealth <= 0) { Kill(); }
        }

        public void Heal(float amount)
        {
            Damage(-amount);
        }

        public void IReset()
        {
            RestoreToMaxHP();
        }

        public void Kill(bool withAnim = false)
        {
            SceneManager.LoadScene(gameOverSceneName);
        }

        public void Register()
        {
            ResetManager.AddResettable(this);
        }

        public void RestoreToMaxHP()
        {
            currentHealth = maxHealth;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerController p = collision.gameObject.GetComponent<PlayerController>();

            if (p != null)
            {
                p.Damage(1000);
            }
        }
    }
}

