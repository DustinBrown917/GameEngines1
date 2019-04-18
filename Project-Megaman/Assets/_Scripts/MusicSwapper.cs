using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwapper : MonoBehaviour
{
    [SerializeField] private AudioClip enterClip;
    [SerializeField] private AudioClip exitClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject triggerObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == triggerObject)
        {
            audioSource.clip = enterClip;
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == triggerObject)
        {
            audioSource.clip = exitClip;
            audioSource.Play();
        }
    }
}
