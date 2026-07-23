using System;
using UnityEngine;

public class FoodPosition : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private SpawnableFood food;
    
    [Header("Settings")] 
    private bool isEmpty;
    public bool IsEmpty => isEmpty;
    
    public bool IsFoodVisible => food != null && food.Isvisible; 

    private void Awake()
    {
        isEmpty = true;
    }

    void Start()
    {
        isEmpty = true;
    }

    public void push(SpawnableFood foodInstance)
    {
        food = foodInstance;
        
        foodInstance.transform.SetParent(transform);
        foodInstance.transform.localPosition = Vector3.zero;
        
        isEmpty = false;
    }

    public SpawnableFood pop()
    {
        isEmpty = true;

        SpawnableFood foodToReturn = food;
        food = null;

        return foodToReturn;
    }
    
    public void displayFood()
    {
        food?.display(); 
    }

    public void markAsDirty()
    {
        food?.MarkAsDirty(); 
    }

    public void HideFood()
    {
        food?.hide(); 
    }
}