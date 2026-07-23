using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItemAbility : MonoBehaviour
{
    [Header("elements")]
    [SerializeField] private Plate plate;

    [Header("grab timer")]
    private const float GrabFoodDelay = 0.1f;//设置拿取物品的时间间隔
    private float grabFoodTimer;//设置计时器。
    private float dropFoodTimer;//放置物品的计时器。
    public bool IsPlateEmpty=> plate.IsEmpty;
    public bool IsPlateFull=> plate.IsFull;
    public bool IsPlateDirty=> plate.IsDirty;

    private void Start()
    {
        grabFoodTimer = GrabFoodDelay;

    }

    public void SetPlate(Plate newPlate)
    {
        plate = newPlate;
    }

    public void handleFoodTrigger(FoodSpawnStation component)//这个是拿取逻辑，角色进入触发器时持续触发
    {
        if (plate.IsFull)
        {
            return;
        }

        if (grabFoodTimer<GrabFoodDelay)//检测计时器，如果小于设定的时间间隔，则返回，这个函数就跳出。第二次触发，计时器就会大于时间间隔，就会执行下面的逻辑。
        {
            grabFoodTimer+=Time.deltaTime;
            return;
        }

        SpawnableFood foodToGrab = component.pop();

        if (foodToGrab == null)
        {
            return;
        }
        
        plate.gameObject.SetActive(true);
        plate.push(foodToGrab);
        grabFoodTimer = 0;
    }

    public void handleFoodDropZone(Food_Drop_Zone dropZone)//放置逻辑
    {
        if (!plate.gameObject.activeInHierarchy)
        {
            return;
        }

        if (plate.IsDirty)
        {
            return;
        }

        if (dropZone.isFull)
        {
           return;
        }
        
        if (dropFoodTimer<GrabFoodDelay)
        {
            dropFoodTimer+=Time.deltaTime;
            return;
        }

        SpawnableFood food = plate.pop();

        if (food == null)
        {
            return;
        }

        dropZone.push(food);

        if (plate.IsEmpty)
        {
            plate.gameObject.SetActive(false);
        }
        
        dropFoodTimer = 0;
        
    }
    
}
