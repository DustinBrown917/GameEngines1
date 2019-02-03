﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public class PlayerController : MonoBehaviour
    {
        private static PlayerController instance_ = null;
        public static PlayerController Instance { get { return instance_; } }

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D groundCheckCollider;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Vector2 groundCheckDimensions;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float jumpSpeed;

        private Animator animator;
        private Rigidbody2D rb2d;
        

        private bool canRecieveInput_ = true;
        private Vector2 cachedVelocity;

        private void Awake()
        {
            //Open singleton pattern.
            if(instance_ == null) {
                instance_ = this;

                animator = GetComponent<Animator>();
                rb2d = GetComponent<Rigidbody2D>();

            } else {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            cachedVelocity.y = rb2d.velocity.y;
            if (canRecieveInput_)
            {
                cachedVelocity.x = Input.GetAxis("Horizontal") * movementSpeed * Time.fixedDeltaTime;

                if (Mathf.Abs(cachedVelocity.x) > 3.0f && IsGrounded()) { animator.SetBool("isRunning", true); }
                else { animator.SetBool("isRunning", false); }

                if (Input.GetButtonDown("Jump")) { Jump(); }
            }
        }

        private void FixedUpdate()
        { 
            rb2d.velocity = cachedVelocity;
            SetFacing();
        }

        private void OnDrawGizmos()
        {
        }

        private void OnDestroy()
        {
            //Close singleton pattern.
            if(instance_ == this) {
                instance_ = null; 
            }
        }

        /// <summary>
        /// Check if the player is grounded.
        /// </summary>
        /// <returns>True if the player is grounded.</returns>
        public bool IsGrounded()
        {
            return groundCheckCollider.IsTouchingLayers(whatIsGround);           
        }

        /// <summary>
        /// Adds to the player's y velocity, only if the player is grounded.
        /// </summary>
        private void Jump()
        {
            if (IsGrounded()) {
                cachedVelocity.y += jumpSpeed;
            }
        }

        /// <summary>
        /// Sets which direction the player is facing based on their velocity.
        /// </summary>
        private void SetFacing()
        {
            if(rb2d.velocity.x > 0) { spriteRenderer.flipX = true; }
            else if(rb2d.velocity.x < 0) { spriteRenderer.flipX = false; }
        }
    }
}

