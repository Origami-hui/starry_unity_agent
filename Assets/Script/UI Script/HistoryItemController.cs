using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryItemController : MonoBehaviour
{
    public AudioClip clip;

    public void PlaySound()
    {
        if(clip == null)
        {
            return;
        }
        AudioSource audioSource = GameObject.FindGameObjectWithTag("AudioSource").GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
