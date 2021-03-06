﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class PlayerController : MonoBehaviour, IHealthyObject, IResettable, IEnergyObject
    {
        private static PlayerController instance_ = null;
        public static PlayerController Instance { get { return instance_; } }

        [SerializeField] private SpriteRenderer spriteRenderer;
        private Vector3 startPosition;
        private float startGravity;
        [SerializeField] private Collider2D groundCheckCollider;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Collider2D highLadderCollider;
        [SerializeField] private Collider2D lowLadderCollider;
        [SerializeField] private ContactFilter2D filter;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float climbSpeed;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Vector3 idleFireOffset;
        [SerializeField] private Vector3 runningFireOffset;
        [SerializeField] private Vector3 jumpingFireOffset;
        [SerializeField] private Vector3 climbingFireOffset;
        [SerializeField] private float fireCoolDownTime;
        [SerializeField] private float damageInvulnerabilityTime = 2.0f;
        [SerializeField] private float damageFlashRate = 1;
        private bool canFire = true;
        private bool isClimbing = false;
        private int livesRemaining_ = 3;
        public int LivesRemaining { get { return livesRemaining_; } }
        private bool isAlive_ = true;
        public bool IsAlive { get { return isAlive_; } }

        [SerializeField] private float maxHP_ = 100.0f;
        public float MaxHP { get { return maxHP_; } }
        [SerializeField] private float maxEnergy = 100.0f;
        [SerializeField] private Vector2 damageVelocity;
        private float currentHP_;
        public float CurrentHP { get { return currentHP_; } }
        private float currentEnergy;

        private Vector3 currentFirePosition;

        private bool isGrounded = true;
        private bool shouldJump = false;

        private Animator animator;
        private Rigidbody2D rb2d;
        private bool canReceiveDamage_ = true;
        public bool CanReceiveDamage { get { return canReceiveDamage_; } }
        private bool canRecieveInput_ = true;
        private Vector2 cachedVelocity;

        private Coroutine cr_FireCooldown;
        private WaitForSeconds wfs_FireCooldown;
        private Coroutine cr_DamageReceivedSequence;
        private Coroutine cr_PickupFlash;
        [SerializeField] private float pickupFlashTime = 1.0f;
        [SerializeField] private float pickupFlahRate = 1.0f;

        private AudioSource audioSource;

        [SerializeField] private AudioClip damagedSound;
        [SerializeField] private AudioClip defeatSound;
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip shootSound;

        private void Awake()
        {
            //Open singleton pattern.
            if (instance_ == null) {
                instance_ = this;

                startPosition = transform.position;

                animator = GetComponent<Animator>();
                rb2d = GetComponent<Rigidbody2D>();
                wfs_FireCooldown = new WaitForSeconds(fireCoolDownTime);
                audioSource = GetComponent<AudioSource>();

                //For the sake of the assignment. This could could be avoided by prefacing the class with "RequireComponent" pattern.
                if(animator == null) { throw new UnityException("No animator found on Player Controller"); }
                if(rb2d == null) { throw new UnityException("No Rigid Body 2D found on Player Controller"); }
                if(audioSource == null) { throw new UnityException("No AudioSource found on Player Controller"); }

                startGravity = rb2d.gravityScale;
                Register();

            } else {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            RestoreToMaxHP();
        }

        void Update()
        {
            if (canRecieveInput_ && !GameManager.Instance.Paused) 
            {
                if (Input.GetButtonDown("Fire2")) { animator.SetTrigger("isSecondaryShooting"); }
                if (Input.GetButtonDown("Jump")) {
                    if (isClimbing) { StopClimbing(); }
                    if (isGrounded) { shouldJump = true; }
                }
                cachedVelocity.x = Input.GetAxis("Horizontal") * movementSpeed * Time.fixedDeltaTime;

                if (Input.GetButton("Horizontal") && isGrounded) { animator.SetBool("isRunning", true); }
                else { animator.SetBool("isRunning", false); }

                if (Input.GetButtonDown("Vertical") && GetClimbOverlapState() != ClimbCheckOverlapState.NONE)
                {
                    float v = Input.GetAxis("Vertical");
                    switch (GetClimbOverlapState())
                    {
                        case ClimbCheckOverlapState.TOP:
                            if (v > 0.0f) { StartClimbing(); }
                            break;
                        case ClimbCheckOverlapState.BOTTOM:
                            if (v < 0.0f) { StartClimbing(); }
                            else if(v > 0.0f) { StopClimbing(); }
                            break;
                        case ClimbCheckOverlapState.BOTH:
                            if (v < 0.0f && GetIsGrounded()) { StopClimbing(); }
                            else { StartClimbing(); }
                            break;
                        case ClimbCheckOverlapState.NONE: //Never happens
                        default:
                            break;
                    }
                }

                if (Input.GetButtonDown("Fire1")) {
                    animator.SetBool("isShooting", true);
                }
                if (Input.GetButtonUp("Fire1")) { animator.SetBool("isShooting", false); }
            }
        }

        private void FixedUpdate()
        {
            if (canRecieveInput_ && !GameManager.Instance.Paused)
            {
                if (isClimbing) {
                    
                    cachedVelocity.y = Input.GetAxis("Vertical") * climbSpeed * Time.fixedDeltaTime;
                    if (Mathf.Abs(cachedVelocity.y) > 0) {
                        animator.SetBool("isMovingWhileClimbing", true);
                        ClimbCheckOverlapState ccos = GetClimbOverlapState();
                        if(cachedVelocity.y > 0) {
                            if(ccos == ClimbCheckOverlapState.BOTTOM) {
                                canRecieveInput_ = false;
                                animator.Play("Player_Climb_Transition_Exit", -1);
                            }
                        }
                    }
                    else { animator.SetBool("isMovingWhileClimbing", false); }

                    if (cachedVelocity.x > 0) {
                        ManualSetFacing(Directions.RIGHT);
                        if (isGrounded)
                        {
                            //StopClimbing();
                        }
                    }
                    else if(cachedVelocity.x < 0) {
                        ManualSetFacing(Directions.LEFT);
                        if (isGrounded)
                        {
                            //StopClimbing();
                        }
                    }
                    cachedVelocity.x = 0;
                } else {
                    if(Input.GetAxis("Vertical") < 0.0f && GetClimbOverlapState() == ClimbCheckOverlapState.BOTTOM)
                    {
                        canRecieveInput_ = false;
                        animator.Play("Player_Climb_Transition_Enter", -1);
                    }
                    cachedVelocity.y = rb2d.velocity.y;
                }


                if (shouldJump) { Jump(); }
                rb2d.velocity = cachedVelocity;
                SetFacing();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            GetIsGrounded();

            Pickup pu = collision.gameObject.GetComponent<Pickup>();

            if (pu != null) {
                CoroutineManager.BeginCoroutine(PickupFlash(pu.EffectColor), ref cr_PickupFlash, this);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            GetIsGrounded();
        }

        private void OnDestroy()
        {
            //Close singleton pattern.
            if (instance_ == this) {
                ResetManager.RemoveResettable(this);
                instance_ = null;
            }
        }

        public void AddLives(int lives)
        {
            livesRemaining_ += lives;

            OnLivesChanged(new LivesChangedArgs(lives, livesRemaining_));
        }

        /// <summary>
        /// Check if the player is grounded.
        /// </summary>
        /// <returns>True if the player is grounded.</returns>
        public bool GetIsGrounded()
        {
            isGrounded = groundCheckCollider.IsTouchingLayers(whatIsGround);
            animator.SetBool("isGrounded", isGrounded);
            return isGrounded;
        }

        public ClimbCheckOverlapState GetClimbOverlapState()
        {
            if (highLadderCollider.IsTouchingLayers(filter.layerMask) && lowLadderCollider.IsTouchingLayers(filter.layerMask)) {
                return ClimbCheckOverlapState.BOTH;
            } else if (highLadderCollider.IsTouchingLayers(filter.layerMask)) {
                return ClimbCheckOverlapState.TOP;
            } else if (lowLadderCollider.IsTouchingLayers(filter.layerMask)) {
                return ClimbCheckOverlapState.BOTTOM;
            } else {
                return ClimbCheckOverlapState.NONE;
            }
        }

        public void AddYOffset(float offset)
        {
            Vector3 pos = transform.position;
            pos.y += offset;
            transform.position = pos;
        }

        private void StartClimbing() //Would make this a method that takes a bool, but the animator doesn't like that.
        {
            Collider2D[] results = new Collider2D[1];

            highLadderCollider.OverlapCollider(filter, results);

            if (results[0] == null)
            {
                lowLadderCollider.OverlapCollider(filter, results);
            }

            if (results[0] != null)
            {
                Vector3 newPos = transform.position;
                newPos.x = results[0].transform.position.x + results[0].offset.x;

                transform.position = newPos;

                rb2d.gravityScale = 0;
                cachedVelocity = new Vector2();
                
                isClimbing = true;
                animator.SetBool("isClimbing", isClimbing);
            }

        }

        private void StopClimbing()
        {
            rb2d.gravityScale = startGravity;
            isClimbing = false;
            animator.SetBool("isClimbing", isClimbing);
        }

        /// <summary>
        /// Adds to the player's y velocity, only if the player is grounded.
        /// </summary>
        private void Jump()
        {
            cachedVelocity.y += jumpSpeed;
            if(jumpSound == null) { throw new UnityException("Jump sound is missing from PlayerController"); }
            audioSource.clip = jumpSound;
            audioSource.Play();
            shouldJump = false;
        }

        /// <summary>
        /// Sets which direction the player is facing based on their velocity.
        /// </summary>
        private void SetFacing()
        {
            if (rb2d.velocity.x > 0) {
                ManualSetFacing(Directions.RIGHT);
            }
            else if (rb2d.velocity.x < 0) {
                ManualSetFacing(Directions.LEFT);
            }
        }

        private void ManualSetFacing(Directions dir)
        {
            switch (dir)
            {
                case Directions.LEFT:
                    spriteRenderer.flipX = false;
                    if (currentFirePosition.x > 0) { currentFirePosition.x *= -1; }
                    break;
                case Directions.RIGHT:
                    spriteRenderer.flipX = true;
                    if (currentFirePosition.x < 0) { currentFirePosition.x *= -1; }
                    break;
                default:
                    break;
            }
        }

        public void EnableInput()
        {
            canRecieveInput_ = true;
        }

        public void DisableInput()
        {
            canRecieveInput_ = false;
        }

        public void Shoot()
        {
            if (!canFire) { return; }
            Projectile p = Instantiate(projectilePrefab, transform.position + currentFirePosition, Quaternion.identity).GetComponent<Projectile>();

            if (shootSound == null) { throw new UnityException("Shoot sound is missing from PlayerController"); }
            audioSource.clip = shootSound;
            audioSource.Play();

            if (p != null && spriteRenderer.flipX)
            {
                p.SetLaunchVelocity(new Vector2(p.LaunchVelocity.x * -1, p.LaunchVelocity.y));
            }

            canFire = false;

            CoroutineManager.BeginCoroutine(FireCooldownTimer(), ref cr_FireCooldown, this);
        }

        public void SetCurrentFirePosition(CharacterStates s)
        {
            switch (s)
            {
                case CharacterStates.IDLE:
                    currentFirePosition = idleFireOffset;
                    break;
                case CharacterStates.RUNNING:
                    currentFirePosition = runningFireOffset;
                    break;
                case CharacterStates.JUMPING:
                    currentFirePosition = jumpingFireOffset;
                    break;
                case CharacterStates.CLIMBING:
                    currentFirePosition = climbingFireOffset;
                    break;
                default:
                    break;
            }

            if (spriteRenderer.flipX) { currentFirePosition.x *= -1.0f; }
        }


        private IEnumerator FireCooldownTimer()
        {
            yield return wfs_FireCooldown;
            canFire = true;
            cr_FireCooldown = null;
        }

        private IEnumerator DamageReceivedSequence()
        {
            if (isClimbing) { StopClimbing(); }
            gameObject.layer = LayerMask.NameToLayer("PlayerInjured");
            Color c1 = Color.white;
            Color c2 = new Color(1.0f, 1.0f, 1.0f, 0.25f);

            bool colorFlipFlop = true;

            canReceiveDamage_ = false;

            float flipRateTimer = 0;
            float t = 0;
            while(t < damageInvulnerabilityTime)
            {
                
                if(flipRateTimer >= damageFlashRate)
                {
                    spriteRenderer.color = (colorFlipFlop) ? c1 : c2;
                    colorFlipFlop = !colorFlipFlop;
                    flipRateTimer = 0;
                }

                flipRateTimer += Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = c1;
            if(currentHP_ > 0)
            {
                canReceiveDamage_ = true;
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
            
        }

        private IEnumerator PickupFlash(Color effectColour)
        {
            bool colorFlipFlop = true;

            float flipRateTimer = 0;
            float t = 0;
            while(t < pickupFlashTime)
            {
                if(flipRateTimer >= pickupFlahRate) {
                    spriteRenderer.color = (colorFlipFlop) ? Color.white : effectColour;
                    colorFlipFlop = !colorFlipFlop;
                    flipRateTimer = 0;
                }

                flipRateTimer += Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = Color.white;
        }

        public void RestoreToMaxHP()
        {
            currentHP_ = maxHP_;
            OnDamageReceived(new DamageReceivedArgs(1.0f, 0.0f));
        }

        public void Heal(float amount)
        {
            Damage(-amount);
        }

        public void Damage(float amount)
        {
            if (!canReceiveDamage_) { return; }
            if(amount >= 0) {
                CoroutineManager.BeginCoroutine(DamageReceivedSequence(), ref cr_DamageReceivedSequence, this);
                ApplyDamageForce();
                animator.SetTrigger("takeDamage");

                if (damagedSound == null) { throw new UnityException("Damaged sound is missing from PlayerController"); }
                audioSource.clip = damagedSound;
                audioSource.Play();
            }
            currentHP_ = Mathf.Clamp(currentHP_ - amount, 0, maxHP_);

            OnDamageReceived(new DamageReceivedArgs(currentHP_ / maxHP_, amount));

            if(currentHP_ == 0) { Kill(true); }
        }

        public void ApplyDamageForce() {
            canRecieveInput_ = false;
            Vector2 vel = damageVelocity;
            if (spriteRenderer.flipX) { vel.x *= -1; }
            rb2d.velocity = vel;           
        }

        public void Kill(bool withAnim = false) {
            if (!isAlive_) { return; }
            canRecieveInput_ = false;
            canReceiveDamage_ = false;
            isAlive_ = false;

            if (defeatSound == null) { throw new UnityException("Defeat sound is missing from PlayerController"); }
            audioSource.clip = defeatSound;
            audioSource.Play();

            rb2d.velocity = new Vector2(0.0f, rb2d.velocity.y);

            AddLives(-1);

            OnDeath();
            if (withAnim) {
                animator.SetTrigger("death");
            }
            
        }

        /// <summary>
        /// To interface with the Animator for animation events. Why cant we pass bools!? Come on, Unity!
        /// </summary>
        public void KillWrapper() {
            Kill();
        }

        public void Register() {
            ResetManager.AddResettable(this);
        }

        public void IReset()
        {
            RestoreToMaxHP();
            transform.position = startPosition;
            rb2d.velocity = new Vector2();
            canRecieveInput_ = true;
            canReceiveDamage_ = true;
            StopClimbing();
            animator.SetBool("isShooting", false);
            animator.Play("Player_Idle", -1);
            gameObject.layer = LayerMask.NameToLayer("Player");
            isAlive_ = true;
        }

        public void RestoreToMaxEnergy()
        {
            currentEnergy = maxEnergy;
        }

        public void GainEnergy(float amount)
        {
            currentEnergy += amount;
            Debug.Log(currentEnergy.ToString() + " Energy Left.");
        }

        public void LoseEnergy(float amount) {
            GainEnergy(-amount);
        }


        /******************************************************************************/
        /*********************************** EVENTS ***********************************/
        /******************************************************************************/

        public event EventHandler Death;

        public void OnDeath()
        {
            EventHandler handler = Death;

            if(handler != null) { handler(this, EventArgs.Empty); }
        }


        public event EventHandler<DamageReceivedArgs> DamageReceived;

        public class DamageReceivedArgs : EventArgs
        {
            public float percentHealthRemaining;
            public float damageAmountReceived;

            public DamageReceivedArgs(float percentRemaining, float amountReceived)
            {
                percentHealthRemaining = percentRemaining;
                damageAmountReceived = amountReceived;
            }
        }

        public void OnDamageReceived(DamageReceivedArgs e)
        {
            EventHandler<DamageReceivedArgs> handler = DamageReceived;

            if (handler != null) { handler(this, e); }
        }


        public event EventHandler<LivesChangedArgs> LivesChanged;

        public class LivesChangedArgs : EventArgs
        {
            public int livesChangedBy;
            public int newLives;

            public LivesChangedArgs(int changedBy, int newLives)
            {
                livesChangedBy = changedBy;
                this.newLives = newLives;
            }
        }

        private void OnLivesChanged(LivesChangedArgs e)
        {
            EventHandler<LivesChangedArgs> handler = LivesChanged;

            if(handler != null) { handler(this, e); }
        }

        /******************************************************************************/
        /************************************ ENUMS ***********************************/
        /******************************************************************************/

        public enum CharacterStates
        {
            IDLE,
            RUNNING,
            JUMPING,
            CLIMBING
        }

        public enum ClimbCheckOverlapState
        {
            NONE,
            TOP,
            BOTTOM,
            BOTH
        }

        public enum Directions
        {
            LEFT,
            RIGHT
        }
    }
}

