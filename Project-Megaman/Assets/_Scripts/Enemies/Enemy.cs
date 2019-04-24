using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public abstract class Enemy : MonoBehaviour, IHealthyObject, IResettable
    {
        protected Rigidbody2D rb2d;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected bool respawns;
        protected float currentHealth;
        [SerializeField] protected float meleeDamage;
        [SerializeField] protected GameObject projectile;

        [SerializeField] protected SpriteRenderer spriteRenderer;
        protected Animator animator;
        protected Vector3 spawnPosition; //if no spawn parent exists when trying to respawn, will default to spawning at this location.
        [SerializeField] protected Transform spawnParent;
        protected Vector3 offScreenHoldingPosition = new Vector3(0.0f, 0.0f, -100.0f);

        protected AudioSource audioSource;

        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] protected AudioClip shootSound;
        

        private Coroutine cr_safeToSpawnChecker;
        private Coroutine cr_enemyBehaviour;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
        }

        protected virtual void Start()
        {
            Register();
            currentHealth = maxHealth;
            spawnPosition = transform.position;
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerController p = collision.collider.GetComponent<PlayerController>();

            if (p != null) {
                p.Damage(meleeDamage);
            }
        }

        protected virtual void OnBecameVisible()
        {
            animator.enabled = true;
            rb2d.simulated = true;
            if (cr_enemyBehaviour == null){
                CoroutineManager.BeginCoroutine(EnemyBehaviour(), ref cr_enemyBehaviour, this);
            }

        }

        protected virtual void OnBecameInvisible()
        {
            if(currentHealth <= 0) { return; } //If they have already been killed, they will not need to handle offscreen.
            HandleOffscreen();
        }

        public void SetSpawnParent(Transform t)
        {
            spawnParent = t;
        }

        public virtual void Damage(float amount)
        {
            currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

            if (currentHealth == 0) { Kill(true); }
            else {
                audioSource.clip = hitSound;
                audioSource.Play();
            }
        }

        public virtual void Heal(float amount)
        {
            Damage(-amount);
        }

        public virtual void Kill(bool withAnim = false)
        {
            audioSource.clip = deathSound;
            audioSource.Play();
            if (withAnim) {
                animator.SetTrigger("death");
                rb2d.simulated = false;
                ParticleManager.Instance.CreateParticleExplosion(UnityEngine.Random.Range(0, ParticleManager.Instance.NumOfParticleBlasts), transform.position);
                CoroutineManager.HaltCoroutine(ref cr_enemyBehaviour, this);
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
            animator.enabled = false;
            rb2d.simulated = false;
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
            if(spawnParent != null) { transform.position = spawnParent.position; }
            else { transform.position = spawnPosition; }
        }

        protected virtual IEnumerator SafeToSpawnChecker()
        {
            transform.position = offScreenHoldingPosition;

            bool safeToReset = false;
            Vector3 testPosition = (spawnParent == null) ? spawnPosition : spawnParent.position;
            while (!safeToReset) {
                Vector2 screenCoords = GameManager.MainCamera.WorldToViewportPoint(spawnPosition);
                if(screenCoords.x < 0 || screenCoords.x > 1 || screenCoords.y < 0 || screenCoords.y > 1) { safeToReset = true; }
                yield return null;
            }
            IReset();
        }

        protected virtual void FireProjectile() {
            audioSource.clip = shootSound;
            audioSource.Play();
        }
        protected abstract IEnumerator EnemyBehaviour();
    }
}

