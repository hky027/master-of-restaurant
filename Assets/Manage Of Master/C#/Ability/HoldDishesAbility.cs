using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldDishesAbility : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField]
    private Plate _plate;

    public void SetPlate(Plate newPlate)
    {
        _plate = newPlate;
    }

    public bool CanCollectDishes()
    {
        
        if (!_plate.gameObject.activeInHierarchy)
        {
            return true;
        }

        if (!_plate.IsEmpty && !_plate.IsDirty)
        {
            return false;
        }

        return true;

    }
    
    public void CollectDishes(SpawnableFood[] dishes)
    {
        for (int i = 0; i < dishes.Length; i++)
        {
            if (dishes[i] == null) continue;
            _plate.gameObject.SetActive(true);
            _plate.push(dishes[i]);
        }
    }

    public bool HasDishes()
    {
        return !_plate.IsEmpty;
    }

    public SpawnableFood[] PopAll()
    {
        SpawnableFood[] Dishes = _plate.PopAll();
        _plate.gameObject.SetActive(false);
        return Dishes;
    }
}
