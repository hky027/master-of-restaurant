using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food_Drop_Zone : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private Plate plate;
    
    public bool isFull=>plate.IsFull;
    public float FoodCount => plate.GetFoodCount();
    
    [Header("Settings")]
    [SerializeField]private Transform workerTargetPoint;
    public Vector3 WorkerTargetPoint => workerTargetPoint.position;

    public void push(SpawnableFood food)
    {
        plate.push(food);
    }

    public FoodPosition GetFirstFullPosition()
    {
        return plate.GetFirstFullPosition();
    }

    public SpawnableFood pop()
    {
        return plate.pop();
    }
}
