using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class Enemy : MonoBehaviour, IHealthyObject, IResettable
    {
        

        protected Rigidbody2D rb2d;
        [SerializeField] protected float maxHealth;
        [SerializeField] protected bool respawns;
        [SerializeField] protected bool startsAsleep;
        protected float currentHealth;
        [SerializeField] protected float meleeDamage;
        [SerializeField] protected GameObject projectile;

        [SerializeField] private SpriteRenderer spriteRenderer;
        protected Animator animator;
        protected Vector3 spawnPosition; //if no spawn parent exists when trying to respawn, will default to spawning at this location.
        [SerializeField] protected Transform spawnParent;
        protected Vector3 offScreenHoldingPosition = new Vector3(0.0f, 0.0f, -100.0f);

        private Coroutine cr_safeToSpawnChecker;
        private Coroutine cr_enemyBehaviour;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();
        }

        protected virtual void Start()
        {
            Register();
            currentHealth = maxHealth;
            spawnPosition = transform.position;

            if (startsAsleep)
            {
                rb2d.simulated = false;
                CoroutineManager.HaltCoroutine(ref cr_enemyBehaviour, this);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerController p = collision.GetComponent<PlayerController>();

            if (p != null) {
                p.Damage(meleeDamage);
            }
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
            if(cr_enemyBehaviour == null){
                CoroutineManager.BeginCoroutine(EnemyBehaviour(), ref cr_enemyBehaviour, this);
            }
            rb2d.simulated = true;

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
        }

        public virtual void Heal(float amount)
        {
            Damage(-amount);
        }

        public virtual void Kill(bool withAnim = false)
        {
            if (withAnim) {
                animator.SetTrigger("death");
                rb2d.simulated = false;
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
            OnSpawnedIn();
        }

        private IEnumerator SafeToSpawnChecker()
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

        protected virtual IEnumerator EnemyBehaviour()
        {
            while (true) {   
                //enemy behaviour goes here.
                yield return null;
            }           
        }





        /******************************************************************************/
        /*********************************** EVENTS ***********************************/
        /******************************************************************************/

        public event EventHandler SpawnedIn;

        protected void OnSpawnedIn()
        {
            EventHandler handler = SpawnedIn;

            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}

