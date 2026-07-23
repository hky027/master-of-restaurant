using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject plateau;
    
    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
    }

    public void SetPlateau(GameObject newPlateau)
    {
        plateau = newPlateau;
    }
    
    public void ManageAnimations(Vector3 moveVector, float moveSpeed)
    {
        if (moveVector.magnitude > 0)
        {
            animator.SetFloat("moveSpeed", moveSpeed / 1.5f);
            PlayWalkAnimation();
            animator.transform.forward = moveVector.normalized; 
        }
        else
        {
            PlayIdleAnimation();
        }
    }
    
    private void PlayWalkAnimation()
    {
        if (plateau == null)
            animator.Play("walking_scound");
        else
        {
            if (plateau.gameObject.activeInHierarchy)
                animator.Play("WalkWithPlateau");
            else
                animator.Play("walking_scound");
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
}