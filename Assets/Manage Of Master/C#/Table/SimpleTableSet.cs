using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GuidGenerator))]
public class TableSet : MonoBehaviour
{
    [Header("Components")] private GuidGenerator guidGenerator;
    
    [Header("Element")]
    [SerializeField]private Plate _plate;
    private Table_mannager _tableMannager;
    private chair[] _chairs;
    [SerializeField]private AudioSource _audioSource;
    [SerializeField]private AudioClip _cleanSound;
    
    [Header("Settings")]
    private bool isFull;
    public bool IsFull=> isFull;
    private bool isDirty;
    public bool IsDirty=> isDirty;
    [SerializeField]private Transform workerTargetPoint;
    public Vector3 WorkerTargetPosition=>workerTargetPoint.position;

    private int inComingCustomer = 0;

    private List<Customer> customers = new List<Customer>();
    
    [Header("timer")]
    private float timer;
    private float foodTimer;
    private int foodConcumed;

    public string guid=>guidGenerator.GUID;

    private void Awake()
    {
        isFull = false;
        _chairs = GetComponentsInChildren<chair>();
        guidGenerator = GetComponent<GuidGenerator>();
    }

    private void Update()
    {

        if (inComingCustomer>0)
        {
            return;
        }
        
        if (customers.Count>0)
        {
            handleFoodTimer();
            handleTimer();
        }
    }

    private void handleFoodTimer()
    {
        foodTimer += Time.deltaTime;
        if (foodTimer > (foodConcumed+1) * Constants.TIME_TO_CONSUME_FOOD)
        {
            HideNextFood();
        }
    }

    private void HideNextFood()
    {
        foodConcumed++;
        _plate.HideFood();
    }

    private void handleTimer()
    {
        timer -= Time.deltaTime;
        if (timer<0)
        {
            exitCustomer();//alt+enter快捷生成函数。
        }
    }

    private void exitCustomer()
    {
        customers.Clear();
        foodTimer = 0;
        foodConcumed = 0;
        for (int i = 0; i < _chairs.Length; i++)
        {
            if (_chairs[i].IsEmpty)
            {
                continue;
            }
            
            Customer chairCustomer = _chairs[i].pop();
            CustomMannager.instance.HandleExitCustomer(chairCustomer);
            _chairs[i].MessUp();
        }

         isDirty = true;
         
         _tableMannager.pushDirtyTable(this);
         
        _plate.markAsDirty();
        isFull = false;
    }


    public void AcceptCustomer(Customer customer, Table_mannager tableMannager)
    {
        this._tableMannager = tableMannager;
        chair TargetChair = GetFirstEmptyChair();
        if (TargetChair == null)
        {
            Debug.Log("没有空闲的椅子了");
            return;
        }

        inComingCustomer++;
        
        TargetChair.MarkAsOccupied();

        customer.GotoThen(TargetChair.StandPointPosition,() =>HandleCustomerReachedChair(customer,TargetChair));

        CheckIfTableIsFull();
    }

    private void CheckIfTableIsFull()
    {
        for (int i = 0; i < _chairs.Length; i++)
        {
            if (_chairs[i].IsEmpty)
            {
                isFull = false;
                return;
            }
        }
        isFull = true;
    }

    private void HandleCustomerReachedChair(Customer customer, chair targetChair)
    {
        customers.Add(customer);

        targetChair.push(customer);

        //TODO:新加代码部分
        //如果盘子上有未清理的脏食物（上一波客人留下的），先清空盘子
        if (_plate.IsDirty)
        {
            _plate.PopAll();
        }

        for (int i = 0; i < customer.FoodTakenCount; i++)
        {
            SpawnableFood food = customer.pop();
            
            if (food == null) continue;
            
            //TODO:有bug，两位顾客拼桌时，Plate会显示两个，而且显示的Food自动默认更多的那个数量，而不会相加。
            
            _plate.gameObject.SetActive(true);
            _plate.push(food);
            timer += Constants.TIME_TO_CONSUME_FOOD;
        }

        inComingCustomer--;
        inComingCustomer = Mathf.Max(0,inComingCustomer);
    }

    private chair GetFirstEmptyChair()
    {
        for (int i = 0; i < _chairs.Length; i++)
        {
            if (_chairs[i].IsEmpty)
            {
               return _chairs[i];
            }
        }
        return null;
    }

    public void GetCleanedBy(HoldDishesAbility holdDishesAbility)
    {
        SpawnableFood[] dishes = _plate.PopAll();
        _plate.gameObject.SetActive(false);
        _audioSource.PlayOneShot(_cleanSound);
        
        holdDishesAbility.CollectDishes(dishes);

        for (int i = 0; i < _chairs.Length; i++)
        {
            _chairs[i].Fix();

            isDirty = false;
            isFull = false;
        }

        _tableMannager.RemoveDirtyTable(this,holdDishesAbility);

    }
    
}
