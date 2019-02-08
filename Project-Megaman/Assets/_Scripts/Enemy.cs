using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class Enemy : MonoBehaviour, IHealthyObject, IResettable
    {
        [SerializeField] private float maxHealth;
        private float currentHealth;
        [SerializeField] private float meleeDamage;
        [SerializeField] private GameObject projectile;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            Register();
            currentHealth = maxHealth;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerController p = collision.GetComponent<PlayerController>();

            if (p != null)
            {
                p.Damage(meleeDamage);
            }
        }

        public void Damage(float amount)
        {
            Debug.Log(amount);
            currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

            if (currentHealth == 0) { Kill(false); }
        }

        public void Heal(float amount)
        {
            Damage(-amount);
        }

        public void Kill(bool withAnim = false)
        {
            gameObject.SetActive(false);
            if (withAnim) {
                animator.SetTrigger("death");
            }
        }

        public void RestoreToMaxHP()
        {
            currentHealth = maxHealth;
        }

        public void Register()
        {
            ResetManager.AddResettable(this);
        }

        public void IReset()
        {
            RestoreToMaxHP();
        }
    }
}

