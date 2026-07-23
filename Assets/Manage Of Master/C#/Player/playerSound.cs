using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip footStepSound;
    private JoystickPlayerController PC;

    private void Start()
    {
        PC = GetComponent<JoystickPlayerController>();
    }

    private void Update()
    {
        if (PC.IsMoving())
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(footStepSound);
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
