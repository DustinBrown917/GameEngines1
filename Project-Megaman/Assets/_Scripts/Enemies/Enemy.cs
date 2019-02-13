using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class Enemy : MonoBehaviour, IHealthyObject, IResettable
    {

        [SerializeField] protected float maxHealth;
        [SerializeField] protected bool respawns;
        protected float currentHealth;
        [SerializeField] protected float meleeDamage;
        [SerializeField] protected GameObject projectile;

        [SerializeField] private SpriteRenderer spriteRenderer;
        protected Animator animator;
        protected Vector3 spawnPosition;

        private Coroutine cr_safeToSpawnChecker;
        private Coroutine cr_enemyBehaviour;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            Register();
            currentHealth = maxHealth;
            spawnPosition = transform.position;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerController p = collision.GetComponent<PlayerController>();

            if (p != null) {
                p.Damage(meleeDamage);
            }
        }

        protected virtual void OnBecameVisible()
        {
            if(cr_enemyBehaviour != null){
                CoroutineManager.BeginCoroutine(EnemyBehaviour(), ref cr_enemyBehaviour, this);
            }
            
        }

        protected virtual void OnBecameInvisible()
        {
            if(currentHealth <= 0) { return; }
            HandleOffscreen();
        }

        public virtual void Damage(float amount)
        {
            Debug.Log(amount);
            currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

            if (currentHealth == 0) { Kill(false); }
        }

        public virtual void Heal(float amount)
        {
            Damage(-amount);
        }

        public virtual void Kill(bool withAnim = false)
        {
            if (withAnim) {
                animator.SetTrigger("death");
            }
            else {
                HandleDeath();
            }
        }

        public virtual void HandleDeath()
        {
            HandleOffscreen();
        }

        public virtual void HandleOffscreen()
        {
            CoroutineManager.HaltCoroutine(ref cr_enemyBehaviour, this);

            if (!respawns) {
                Destroy(gameObject);
                return;
            }

            CoroutineManager.BeginCoroutine(SafeToSpawnChecker(), ref cr_safeToSpawnChecker, this);
        }

        public virtual void RestoreToMaxHP()
        {
            currentHealth = maxHealth;
        }

        public void Register()
        {
            ResetManager.AddResettable(this);
        }

        public virtual void IReset()
        {
            RestoreToMaxHP();
            transform.position = spawnPosition;
        }

        private IEnumerator SafeToSpawnChecker()
        {
            transform.position = new Vector3(-100.0f, -100.0f, 0.0f); //Would love to use a const for this, but would have to split x and y up and build the vector in the coroutine anyways...

            bool safeToReset = false;
            while (!safeToReset) {
                Vector2 screenCoords = Camera.main.WorldToViewportPoint(spawnPosition);
                if(screenCoords.x < 0 || screenCoords.x > 1 || screenCoords.y < 0 || screenCoords.y > 1) { safeToReset = true; }
                yield return null;
            }
            IReset();
        }

        protected virtual IEnumerator EnemyBehaviour()
        {
            while (true) {               
                yield return null;
            }           
        }
    }
}

