using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [Header("Element")]
    [SerializeField]private Transform workerTargetPoint;
    public Vector3 WorkerTargetPosition=> workerTargetPoint.position;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HoldDishesAbility _holdDishesAbility) && _holdDishesAbility.HasDishes())
        {
            SpawnableFood[] dishes = _holdDishesAbility.PopAll();
            DestroyDishes(dishes);
            return;
        }

        if (other.TryGetComponent(out HoldItemAbility _holdItemAbility) && !_holdItemAbility.IsPlateEmpty)
        {
            while (!_holdItemAbility.IsPlateEmpty)
            {
                SpawnableFood food = GetFoodFromPlate(_holdItemAbility);
                if (food != null)
                {
                    Destroy(food.gameObject);
                }
            }
        }
    }

    private void DestroyDishes(SpawnableFood[] dishes)
    {
        for (int i = dishes.Length - 1; i >= 0; i--)
        {
            Destroy(dishes[i].gameObject);
        }
    }

    private SpawnableFood GetFoodFromPlate(HoldItemAbility _holdItemAbility)
    {
        return _holdItemAbility.IsPlateEmpty ? null : null;
    }
}
