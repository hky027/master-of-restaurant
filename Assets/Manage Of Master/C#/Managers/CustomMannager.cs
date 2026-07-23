using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMannager : MonoBehaviour
{
    public static CustomMannager instance;

    [Header("elements")] 
    [SerializeField] private Customer CustomerPrefabs;
    [SerializeField] private Transform customerExitPoint;
    
    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Customer pop(Vector3 CustomerSpawnPosition)
    {
        return Instantiate(CustomerPrefabs, CustomerSpawnPosition, Quaternion.identity, transform);
    }

    public void HandleExitCustomer(Customer chairCustomer)
    {
        chairCustomer.getUpAndGo(customerExitPoint.position,()=>HandleCustomerToExit(chairCustomer));
    }

    private void HandleCustomerToExit(Customer chairCustomer)
    {
        Destroy(chairCustomer.gameObject);
    }
}
