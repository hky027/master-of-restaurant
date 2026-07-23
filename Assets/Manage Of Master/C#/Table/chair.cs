using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class chair : MonoBehaviour
{
    [SerializeField] private new Transform _renderer;
    [SerializeField]private Transform sitPoint;
    [SerializeField]private Transform standPoint;
    
    public Vector3 StandPointPosition=> standPoint.position;
    public Vector3 SitPointPosition=> sitPoint.position;
    private Customer _customer;
    
    [Header("Settings")]
    [SerializeField] private bool isEmpty;
    public bool IsEmpty=> isEmpty;

    private Quaternion _originalRendererRotation;//存储了椅子初始旋转

    private void Awake()
    {
        isEmpty = true;
        if (_renderer == null)
            _renderer = transform;
        _originalRendererRotation = _renderer.localRotation;//存储了椅子初始旋转
    }

    public void MarkAsOccupied()
    {
        isEmpty = false;
    }

    public void push(Customer customer)
    {
        this._customer = customer;
        customer.transform.SetParent(transform);
        customer.SitDown(sitPoint.position, transform.forward);

    }

    public Customer pop()
    {
        isEmpty = true;
        _customer.transform.SetParent(null);
        Customer customerToReturn = _customer;
        _customer = null;
        return customerToReturn;
    }

    public void MessUp()
    {
       
        _renderer.localRotation = Quaternion.Euler(0,60*Mathf.Sign(Random.Range(-1f,1f)),0);
    }

    public void Fix()
    {
        _renderer.localRotation = _originalRendererRotation;//恢复椅子初始旋转
    }
}
