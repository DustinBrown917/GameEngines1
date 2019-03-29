using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MEGA
{
    public abstract class Pickup : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite graphic;
        [SerializeField] private Behaviour[] behavioursToDisable;

        private AudioSource audioSource;

        private bool canBePickedUp = true;

        [SerializeField] private Color effectColour_;
        public Color EffectColor { get { return effectColour_; } }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!canBePickedUp) { return; }
            OnPickup(collision.gameObject);

            audioSource.Play();

            for(int i = 0; i < behavioursToDisable.Length; i++) {
                behavioursToDisable[i].enabled = false;
            }
            canBePickedUp = false;
            spriteRenderer.sprite = null;
        }

        private void OnBecameVisible()
        {
            for (int i = 0; i < behavioursToDisable.Length; i++) {
                behavioursToDisable[i].enabled = true;
                canBePickedUp = true;
            }

            spriteRenderer.sprite = graphic;
        }

        public abstract void OnPickup(GameObject whoPickedMeUp);

    }
}

