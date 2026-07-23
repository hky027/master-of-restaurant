using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PayAbility : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject cashPrefab;
    
    [Header("Component")] 
    private CharacterController characterController;
    
    public bool isMoving=>
        characterController.velocity.magnitude>0.1f;

    private void Awake()
    {
     characterController = GetComponent<CharacterController>();   
    }

    public void pay(LockedElement lockedElement, int cashPerFrame)
    {
        int ToPay = Mathf.Min(cashPerFrame,CurrencyManager.instance.Currency);

        if (ToPay<=0)
        {
            return;
        }
        
        CurrencyManager.instance.AddCurrency(-ToPay);
        lockedElement.collectCash(ToPay);
        AnimateCashToTarget(lockedElement.transform);
    }

    private void AnimateCashToTarget(Transform _target)
    {
        GameObject cashObject = Instantiate(cashPrefab, transform.position, Quaternion.identity);
        ArcAnimator.Animate(cashObject.transform, _target, 0.2f, 0f, 2f,()=>Destroy(cashObject));
    }
}
