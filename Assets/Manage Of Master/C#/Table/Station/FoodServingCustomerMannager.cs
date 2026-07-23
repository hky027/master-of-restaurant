using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
    
public class FoodServingCustomerMannager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform SpawnPosition;
    [SerializeField] private Transform QueueStartPoint;

    [Header("settings")] 
    [SerializeField] private Vector3 QueueSpacing;
    [SerializeField] private int MaxCustomer;
    [SerializeField] private Vector2Int minMaxCustomerFoodCount;
    
    private Queue<Customer> customerQueue  = new Queue<Customer>();

    private void Start()
    {
        StartSpawningCustomers();
    }

    private void StartSpawningCustomers()
    {
        InvokeRepeating("SpawnNewCustomer",1,1);
    }
    
    private void SpawnNewCustomer()
    {
        if (customerQueue.Count >= MaxCustomer)
        {
            return;
        }
        
        Customer _customer = CustomMannager.instance.pop(SpawnPosition.position);
        _customer.name = "customer"+Random.Range(0,1000);
        
        customerQueue.Enqueue(_customer);
        _customer.StartWaiting();

        Vector3 targetpositon = GetLastCutomerPositon();

        int foodCount = Random.Range(minMaxCustomerFoodCount.x,minMaxCustomerFoodCount.y+1);
        
        _customer.initialize(foodCount,targetpositon,-QueueSpacing.normalized);
    }

    private Vector3 GetLastCutomerPositon()
    {
        return QueueStartPoint.position + QueueSpacing * (customerQueue.Count - 1);
    }

    public bool IsCustomReadyTakeFood()
    {
        if (customerQueue.Count<=0)
        {
            return false;
        }
        Customer _customer = customerQueue.Peek();
        float distance = Vector3.Distance(_customer.transform.position.With(y:0), QueueStartPoint.position);
        return distance <= 0.1f;
    }

    public Customer GetFirstCustomer()
    {
        return customerQueue.Peek();
    }

    public void Dequeu()
    {
        Customer dequeuedCustomer = customerQueue.Dequeue();
        dequeuedCustomer.StopWaiting();

        for (int i = 0; i < customerQueue.Count; i++)
        {
            customerQueue.ToArray()[i].Goto(GetTargetPosition(i));
        }
    }

    private Vector3 GetTargetPosition(int index)
    {
        return QueueStartPoint.position + QueueSpacing * index;
    }
}
