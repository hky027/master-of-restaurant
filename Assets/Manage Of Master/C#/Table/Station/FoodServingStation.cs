using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FoodServingCustomerMannager))]
[RequireComponent(typeof(GuidGenerator))]
public class FoodServingStation : MonoBehaviour
{

  [Header("component")] 
  private FoodServingCustomerMannager _customerMannager;
  private GuidGenerator _guidGenerator;
  [Header("Elements")] 
  [SerializeField] private Food_Drop_Zone _dropZone;
  [SerializeField] private Table_mannager _tableMannager;
  [SerializeField] private TaskRequster _taskRequster;
  [SerializeField] AudioSource _audioSource;//必须在目标物体上挂载AudioSource组件，并且拖转AudioSource赋值给这个变量_audioSource;
  [SerializeField] AudioClip _buySound;//这里赋值想播放的音效
  [SerializeField] private Transform workerTargetPoint;
  [SerializeField] private cash_pile cashPile;

  [Header("Settings")] 
  [SerializeField] private SpawnableFood _foodServedPrefab;
  private int workerCount;
  [SerializeField] private float servingDelay;
  private float servingTimer;
  
  [Header("Request Timer")]
  private float requestCheckTimer;
  [SerializeField] private const float requestCheckDelay = 1f;

  private void Awake()
  {
    _customerMannager = GetComponent<FoodServingCustomerMannager>();
    _guidGenerator = GetComponent<GuidGenerator>();
  }

  private void Update()
  {
    HandleRequestTimer();
  }

  private void HandleRequestTimer()
  {
    if (requestCheckTimer<requestCheckDelay)
    {
      requestCheckTimer += Time.deltaTime;
      return;
    }
    checkRequst();
    requestCheckTimer = 0;
  }

  private void checkRequst()
  {
    if (!HasEnoughFood())
    {
      _taskRequster.createTaskRequest(new FillStationPlateRequest(_guidGenerator.GUID,_foodServedPrefab,_dropZone));
    }

    if (CanSendServerCustomersRequest())
    {
      _taskRequster.createTaskRequest(new ServeCustomersRequest(_guidGenerator.GUID,workerTargetPoint.position,_dropZone));
    }
  }

  private bool CanSendServerCustomersRequest()
  {
    return _customerMannager.IsCustomReadyTakeFood() && HasEnoughFood();
  }

  private bool HasEnoughFood()
  {
    return _dropZone.isFull;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!other.TryGetComponent(out PlayerDtector _playerdetector))
    {
      return;
    }
    workerCount++;
  }
  
  private void OnTriggerStay(Collider other)
  {
    if (workerCount>0)
    {
      handleFoodServing();
    }
  }
  
  private void OnTriggerExit(Collider other)
  {
    if (!other.TryGetComponent(out PlayerDtector _playerdetector))
    {
      return;
    }
    workerCount--;
    workerCount = Mathf.Max(0,workerCount);
  }
  
  private void handleFoodServing()
  {
    if (servingTimer<servingDelay)
    {
      servingTimer += Time.deltaTime;
      return;
    }

    if (!_customerMannager.IsCustomReadyTakeFood())
    {
        return;
    }

    if (GetFirstFullPosition()==null)
    {
        return;
    }

    if (!_customerMannager.GetFirstCustomer().NeedsMoreFood())
    {
      DequeueCustomer(_customerMannager.GetFirstCustomer());
      servingTimer = 0;
      return;
    }
    
    ActuallyServeFood();
    
  }

  private void ActuallyServeFood()
  {
    _audioSource.PlayOneShot(_buySound);//播放收银音效
    cashPile.GenerateCash(1);//生成金钱
    servingTimer = 0;
    Customer _customerToServe = _customerMannager.GetFirstCustomer();
    SpawnableFood FoodToServe = pop();
    _customerToServe.CollectFood(FoodToServe);
    if (_customerToServe.NeedsMoreFood())
    {
      return;
    }
    DequeueCustomer(_customerToServe);
  }
  
  private void DequeueCustomer(Customer customer)
  {
    if (_tableMannager == null)
    {
      Debug.LogError("Table_mannager is not assigned in the Inspector on " + gameObject.name, this);
      return;
    }

    if (!_tableMannager.IsAnyActive())
    {
      return;
    }
    _customerMannager.Dequeu();
    _tableMannager.handleCustomerServed(customer);
  }
  
  private SpawnableFood pop()
  {
    return _dropZone.pop();
  }

  private FoodPosition GetFirstFullPosition()
  {
    return _dropZone.GetFirstFullPosition();
  }
  
}
