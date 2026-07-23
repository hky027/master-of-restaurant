using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMannager : MonoBehaviour
{
    public static WorkerMannager Instance;
    
    [Header("Elements")]
    [SerializeField] private List<Worker> Workers = new List<Worker>();
    [SerializeField] private List<TaskRequst> pendingRequests = new List<TaskRequst>();
    [SerializeField] private Trash _trash;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Table_mannager.tableCleaned += OnTableCleaned;
    }

    private void OnTableCleaned(TableSet _table, HoldDishesAbility _holdDishesAbility)
    {
        for (int i = 0; i < pendingRequests.Count; i++)
        {
            if (pendingRequests[i] is CleanTableRequest&&pendingRequests[i].Guid == _table.guid)
            {
             pendingRequests.RemoveAt(i);
             break;
            }
        }

        for (int i = 0; i < Workers.Count; i++)
        {
            if (Workers[i].CurrentTask==null)
            {
                return;
            }

            if (!(Workers[i].CurrentTask is CleanTableTask))
            {
                continue;
            }
            
            CleanTableTask cleanTableTask =Workers[i].CurrentTask as CleanTableTask;

            if (cleanTableTask.table!=_table)
            {
                continue;
            }

            if (_holdDishesAbility.gameObject==Workers[i].gameObject)
            {
                continue;
            }

            Workers[i].cancelTask();
        }
    }

    private void OnDestroy()
    {
        Table_mannager.tableCleaned -= OnTableCleaned;
    }

    private void Update()
    {
        HandleRequests();
    }

    private void HandleRequests()
    {
        if (pendingRequests.Count <= 0) return;

        bool hasRealWork = pendingRequests.Exists(r => !(r is IdleRequest));

        for (int i = 0; i < Workers.Count; i++)
        {
            Worker worker = Workers[i];

            if (worker.CurrentTask != null && !(worker.CurrentTask is idleTask))
                continue;

            if (worker.CurrentTask == null && !worker.IsIdle)
                continue;

            // 閬嶅巻鎵€鏈夎姹傦紝涓鸿宸ヤ汉鎵惧埌鍚堥€傜殑浠诲姟
            for (int j = 0; j < pendingRequests.Count; j++)
            {
                if (hasRealWork && pendingRequests[j] is IdleRequest)
                    continue;

                if (TryHandleRequest(pendingRequests[j], worker))
                {
                    pendingRequests.RemoveAt(j);
                    break;
                }
            }
        }
    }

    private bool TryHandleRequest(TaskRequst requst, Worker worker)
    {
        if (requst is ServeCustomersRequest serveRequest)
        {
            if (IsWorkerServingDropZone(serveRequest.DropZone))
                return false;

            HandleRequest(requst, worker);
            return true;
        }

        if (requst is FillStationPlateRequest)
        {
            return TryHandleFillStationPlateRequest(requst, worker);
        }

        if (requst is CleanTableRequest cleanRequest)
        {
            // 鎵嬩笂鏈夊共鍑€鐩樺瓙鏃朵笉鍒嗛厤娓呯悊浠诲姟
            if (!worker.IsPlateEmpty && !worker.IsPlateDirty)
                return false;

            if (IsWorkerCleaningTable(cleanRequest.TableSet))
                return false;

            HandleRequest(requst, worker);
            return true;
        }

        HandleRequest(requst, worker);
        return true;
    }

    private bool IsWorkerServingDropZone(Food_Drop_Zone dropZone)
    {
        for (int i = 0; i < Workers.Count; i++)
        {
            if (Workers[i].CurrentTask is ServeCustomersTask serveTask &&
                serveTask.TargetDropZone == dropZone)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsWorkerCleaningTable(TableSet table)
    {
        for (int i = 0; i < Workers.Count; i++)
        {
            if (Workers[i].CurrentTask is CleanTableTask cleanTask &&
                cleanTask.table == table)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsFoodSpawnStationOccupied(FoodSpawnStation station)
    {
        for (int i = 0; i < Workers.Count; i++)
        {
            if (Workers[i].CurrentTask is FillStationPlateTask fillTask && fillTask.TargetStation == station)
            {
                return true;
            }
        }
        return false;
    }

    private bool TryHandleFillStationPlateRequest(TaskRequst requst, Worker worker)
    {
        FillStationPlateRequest filledRequest = requst as FillStationPlateRequest;
        FoodSpawnStation[] foodSpawnStations = FindObjectsByType<FoodSpawnStation>(FindObjectsSortMode.None);
        if (foodSpawnStations.Length <= 0)
        {
            
            return false;
        }

        List<FoodSpawnStation> PotentialFoodSpawnStations = new List<FoodSpawnStation>();
        for (int i = 0; i < foodSpawnStations.Length; i++)
        {
            if (foodSpawnStations[i].FoodType == filledRequest.Food.GetType())
            {
                PotentialFoodSpawnStations.Add(foodSpawnStations[i]);
            }
        }

        if (PotentialFoodSpawnStations.Count <= 0)
        {
            
            return false;
        }

        // 杩囨护鎺夋鍦ㄨ鍏朵粬宸ヤ汉浣跨敤鐨勭敓鎴愮珯
        List<FoodSpawnStation> availableStations = new List<FoodSpawnStation>();
        for (int i = 0; i < PotentialFoodSpawnStations.Count; i++)
        {
            if (!IsFoodSpawnStationOccupied(PotentialFoodSpawnStations[i]))
            {
                availableStations.Add(PotentialFoodSpawnStations[i]);
            }
        }

        if (availableStations.Count <= 0)
        {
            return false;
        }

        FoodSpawnStation randomFoodSpawnStation = availableStations.ToArray().GetRandom();

        FillStationPlateTask fillTask = new FillStationPlateTask(
            worker,
            randomFoodSpawnStation,
            filledRequest.DropZone,
            requst
        );

        worker.AssignTask(fillTask);
        return true;
    }

    private void HandleRequest(TaskRequst _requst, Worker _worker)
    {
        if (_requst is ServeCustomersRequest)
        {
            handleServeCustomersRequest(_requst,_worker);
        }

        else if (_requst is CleanTableRequest)
        {
            HandleCleanTableRequest(_requst,_worker);
        }

        else if (_requst is IdleRequest)
        {
            HandleIdleRequest(_requst, _worker);
        }
      
    }

    private void HandleIdleRequest(TaskRequst _requst, Worker _worker)
    {
        idleTask task = new idleTask(_worker, (_requst as IdleRequest).TargetPosition, _requst);
        _worker.AssignTask(task);
    }

    private void HandleCleanTableRequest(TaskRequst _requst, Worker _worker)
    {
        CleanTableTask task = new CleanTableTask(_worker,(_requst as CleanTableRequest).TableSet,_trash,_requst);
        _worker.AssignTask(task);
    }

    private void handleServeCustomersRequest(TaskRequst _requst, Worker _worker)
    {
        ServeCustomersTask task = new ServeCustomersTask(_worker,_requst,(_requst as ServeCustomersRequest).WorkerTargetPosition);
        _worker.AssignTask(task);
    }

    public static void RegisterRequest(TaskRequst _request)
    {
        Instance.pendingRequests.Add(_request);
    }
    
}
