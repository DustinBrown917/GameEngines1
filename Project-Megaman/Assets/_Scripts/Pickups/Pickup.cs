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
        private bool canBePickedUp = true;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!canBePickedUp) { return; }
            OnPickup(collision.gameObject);

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

