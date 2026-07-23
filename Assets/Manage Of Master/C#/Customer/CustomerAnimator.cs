using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAnimator : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject plateau;
    [Header("settings")] 
    private bool issitting;
    private Vector3 lastvelocity;

    private void Update()
    {
        if (!issitting)
        {
            handleAnimations();
        }
    }
    
    private void handleAnimations()
    {
        if (lastvelocity.magnitude > 0)
        {
            animator.SetFloat("moveSpeed", lastvelocity.magnitude / 1.5f);
            PlayWalkAnimation();
            animator.transform.forward = Vector3.Lerp(animator.transform.forward, lastvelocity.normalized,Time.deltaTime*60*0.2f);
        }
        else
        {
            PlayIdleAnimation();
        }
    }
    
    private void PlayWalkAnimation()
    {
        if (plateau == null)
            animator.Play("Walk");
        else
        {
            if (plateau.gameObject.activeInHierarchy)
                animator.Play("WalkWithPlateau");
            else
                animator.Play("Walk");
        }
    }

    private void PlayIdleAnimation()
    {
        if (plateau == null)
            animator.Play("Idle");
        else
        {
            if (plateau.gameObject.activeInHierarchy)
                animator.Play("IdleWithPlateau");
            else
                animator.Play("Idle");
        }
    }

    public void startwalking()
    {
        issitting = false;
    }
    
    public void stop()
    {
        lastvelocity = Vector3.zero;
    }

    public void ManageAnimations(Vector3 navigationAbilityVelocity)
    {
        lastvelocity = navigationAbilityVelocity;
    }

    public void Face(Vector3 facing)
    {
        animator.transform.forward = facing;
    }

    public void playsitDownAnimation(Vector3 forward)
    {
        issitting = true;
        animator.Play("Sit");
        Face(forward);
    }
}