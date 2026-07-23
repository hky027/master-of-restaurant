using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    [Header("elements")] 
    [SerializeField] private Transform foodPositionParent;

    [Header("settings")] 
    [SerializeField] private int maxCapacity;
    private bool isFull;
    public bool IsFull => isFull;// =>是c#里的lambda运算符 ，“快速定义匿名法” ，相当于让IsFull读取isFull的值。实现外部只读isFull不能修改的特性。
    private bool isEmpty;
    public bool IsEmpty => GetFirstFullPosition()==null;
    private bool isDirty;
    public bool IsDirty=> isDirty;

    private float positionYoffset;

    private SpawnableFood lastFoodPush;

    void Start()
    {
        isFull = false;
    }
    
    public void push(SpawnableFood foodToGrab)
    {
        if (isDirty&&isFull)
        {
            createnNewFoodPosition();
        }
        
        if (foodToGrab.IsDirty)
        {
            isDirty = true;
        }
        
        FoodPosition foodPosition = GetFirstEmptyFoodPosition();
        foodPosition.push(foodToGrab);

        RearrangeFoodPosition(foodToGrab);
        if (GetFirstEmptyFoodPosition()==null)
        {
            if (foodPositionParent.childCount < maxCapacity)
            {
                createnNewFoodPosition();
            }
            else
            {
                isFull = true;
            }
        }
        else
        {
            int occupiedPosition = 0;
            for (int i = 0; i < foodPositionParent.childCount; i++)
            {
                FoodPosition _fp = foodPositionParent.GetChild(i).GetComponent<FoodPosition>();

                if (!_fp.IsEmpty)
                {
                    occupiedPosition++;
                }

                if (occupiedPosition >= maxCapacity)
                {
                    isFull = true;
                    break;
                }
            }
        }

        if (foodPositionParent.childCount<maxCapacity)
        {
            createnNewFoodPosition();
        }
        
        lastFoodPush = foodToGrab;
    }

    private void createnNewFoodPosition()
    {
        FoodPosition foodPositionInstance = new GameObject("foodPosition"+foodPositionParent.childCount).AddComponent<FoodPosition>();
        foodPositionInstance.transform.SetParent(foodPositionParent);

        int bottomChildIndex = foodPositionInstance.transform.GetSiblingIndex() - 1;
        
        foodPositionInstance.transform.localPosition = foodPositionParent.GetChild(bottomChildIndex).localPosition + Vector3.up * positionYoffset;
        
        foodPositionInstance.transform.localRotation = Quaternion.identity;

        isFull = false;
    }

    private void RearrangeFoodPosition(SpawnableFood foodInstance)
    {
        positionYoffset = foodInstance.IsDirty? foodInstance._dirtyYoffsetOnPlate : foodInstance._cleanYoffsetOnPlate;

        int hiddenFoodCount = 0;
        
        for (int i = 0; i < foodPositionParent.childCount; i++)
        {
            if (!foodPositionParent.GetChild(i).GetComponent<FoodPosition>().IsFoodVisible)
            {
                hiddenFoodCount++;
            }
            
            foodPositionParent.GetChild(i).localPosition = Vector3.up * (i-hiddenFoodCount) * positionYoffset;
        }
    }

    private FoodPosition GetFirstEmptyFoodPosition()
    {
        for (int i = 0; i < foodPositionParent.childCount; i++)
        {
            if (!foodPositionParent.GetChild(i).TryGetComponent(out FoodPosition foodPosition))
            {
                continue;
            }

            if (foodPosition.IsEmpty)
            {
                return foodPosition;
            }
        }
        //Debug.Log("plate:没有空闲位置");
        return null;
    }

    public FoodPosition GetFirstFullPosition()
    {
        for (int i = 0; i < foodPositionParent.childCount; i++)
        {
            if (!foodPositionParent.GetChild(i).TryGetComponent(out FoodPosition foodPosition))
            {
                continue;
            }

            if (!foodPosition.IsEmpty)  // 修复：改为找"有食物"的位置
            {
                return foodPosition;
            }
        }
        return null;
    }

    public SpawnableFood pop()
    {
        FoodPosition foodPosition = GetLastFullPosition();

        if (foodPosition == null)
        {
            return null;
        }

        isFull = false;
     
        
        return foodPosition.pop();
    }

    private FoodPosition GetLastFullPosition()
    {
        for (int i=foodPositionParent.childCount - 1;i>=0;i--)
        {
            if (!foodPositionParent.GetChild(i).TryGetComponent(out FoodPosition foodPosition))
            {
                continue;
            }

            if (!foodPosition.IsEmpty)
            {
                return foodPosition;
            }
        }

        return null;
    }

    public void markAsDirty()
    {
        for (int i = 0; i < foodPositionParent.childCount; i++)
        {
            FoodPosition foodPosition = foodPositionParent.GetChild(i).GetComponent<FoodPosition>();
            if (foodPosition.IsEmpty)
            {
                continue;
            }
            foodPosition.displayFood();
            foodPosition.markAsDirty();
        }

        if (lastFoodPush != null)
        {
            RearrangeFoodPosition(lastFoodPush);
        }
        //RearrangeFoodPosition(lastFoodPush);
    }

    public void HideFood()
    {
        for (int i = foodPositionParent.childCount-1; i >= 0 ; i--)
        {
            FoodPosition foodPosition = foodPositionParent.GetChild(i).GetComponent<FoodPosition>();
            if (foodPosition.IsEmpty)
            {
                continue;
            }

            if (!foodPosition.IsFoodVisible)//修改了代码
            {
                continue;
            }
            foodPosition.HideFood();
            break;
        }
        
    }

    public SpawnableFood[] PopAll()
    {
        List<SpawnableFood> foods = new List<SpawnableFood>();
        for (int i = 0; i < foodPositionParent.childCount; i++)
        { 
            FoodPosition foodPosition = foodPositionParent.GetChild(i).GetComponent<FoodPosition>();
            if (foodPosition.IsEmpty)
            {
                continue;
            }
            SpawnableFood food = foodPosition.pop();
            if (food != null)
            {
                foods.Add(food);
            }
            //foods.Add(foodPosition.pop());
        }
        isFull = false;
        isDirty = false;
        return foods.ToArray();
    }

    public float GetFoodCount()
    {
        int counter = 0;
        for (int i = 0; i < foodPositionParent.childCount; i++)
        {
            if (foodPositionParent.GetChild(i).GetComponent<FoodPosition>().IsEmpty)
            {
                continue;
            }
            counter++;
        }
        return counter;
    }
}
