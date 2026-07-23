using System;
using UnityEngine;


public class FoodSpawnStation : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private SpawnableFood spwanableFoodPrefab;
    [SerializeField] private Plate plate;
    [SerializeField] private Transform workerTargetPoint;
    public Vector3 WorkerTargetPosition => workerTargetPoint.position;

    [Header("Settings")] 
    [SerializeField] private float spawnDelay;
    private float spawnTimer;
    public Type FoodType => spwanableFoodPrefab.GetType();
    private bool isMaking;//想实现制作饮料的逻辑而添加的，有待实现
    
    void Update()
    {
        handleSpawnTimer();
    }

    private void handleSpawnTimer()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer>spawnDelay)
        {
            TrySpawnFood();
            spawnTimer = 0;
        }
    }

    private void TrySpawnFood()
    {
        if (plate.IsFull)
        {
            return;
        }
        else
        {
            SpawnFood();
        }
    }
    
    private void SpawnFood()
    {
        //TODO:添加box collider检测，实现角色制作饮料的逻辑功能
        /*if (isMaking)
        {
            
        }*/
        SpawnableFood foodInstance = Instantiate(spwanableFoodPrefab, transform);
        plate.push(foodInstance);
    }

    public SpawnableFood pop()
    {
        SpawnableFood food  = plate.pop();
        if (food == null)
        {
            return null;
        }
        return food;
    }
}