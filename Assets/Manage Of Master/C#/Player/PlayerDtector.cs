using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HoldItemAbility))]//强制挂载组件，把PlayerDtector挂载到物体上时，
                                           //unity会检测物体上是否有HoldItemAbility组件
                                           //如果没有就自动添加，如果有则不做任何操作。
//[RequireComponent(typeof(类名))]强制挂载组件要在所有类前面

public class PlayerDtector : MonoBehaviour
{
    [Header("components")]
    private HoldItemAbility holdItemAbility;
    [SerializeField] private NavigationAbility navigationAbility;
    
    
    private void Awake()
    {
        holdItemAbility = GetComponent<HoldItemAbility>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (navigationAbility != null && navigationAbility.IsMoving())
        {
            return;
        }
        if (other.TryGetComponent(out FoodSpawnStation station))
        {
            addFoodTrigger(station);
        }

        else if (other.TryGetComponent(out Food_Drop_Zone dropZone))
        {
            handleInDropZone(dropZone);
        }
        
        else if (other.TryGetComponent(out TableSet table))
        {
            cleanTalbeTrriger(table);
        }
    }
    
    private void handleInDropZone(Food_Drop_Zone dropZone)
    {
        holdItemAbility.handleFoodDropZone(dropZone);
    }

    private void addFoodTrigger(FoodSpawnStation component)
    {
        holdItemAbility.handleFoodTrigger(component);
    }
    
    private void cleanTalbeTrriger(TableSet table)
    {
        if (!table.IsDirty)
        {
            return;
        }

        if (!TryGetComponent(out HoldDishesAbility _holdDishesAbility))
        {
             return;
        }

        if (!_holdDishesAbility.CanCollectDishes())
        {
            return;
        }

        table.GetCleanedBy(_holdDishesAbility);
    }
    
}
