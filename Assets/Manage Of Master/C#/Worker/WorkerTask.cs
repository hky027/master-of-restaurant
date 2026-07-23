using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class WorkerTask
{
    protected List<Subtask> _subtasks = new List<Subtask>();
    protected int _currentSubtaskIndex;
    protected Worker _worker;
    protected TaskRequst _request;
    
    public WorkerTask(Worker worker, TaskRequst request)
    {
        this._worker = worker;
        this._request = request;
    }
    
    public void start()
    {
        _currentSubtaskIndex = 0;
        _subtasks[0].start(_worker);
    }
    
    public void update()
    {
        if (_currentSubtaskIndex>=_subtasks.Count)
        {
            return;
        }
        Subtask currentSubtask = _subtasks[_currentSubtaskIndex];
        currentSubtask.update(_worker);

        if (currentSubtask.IsComplete)
        {
            _currentSubtaskIndex++;
            if (_currentSubtaskIndex<_subtasks.Count)
            {
                _subtasks[_currentSubtaskIndex].start(_worker);
            }
            else
            {
                complete();
            }
        }
    }
    
    private void complete()
    {
        if (_request != null && _request.sender!=null)
        {
            _request.sender.ClearRequst(_request);
        }
        
        _worker.CompleteTask();
    }

    public void cancel()
    {
        if (_request != null && _request.sender!=null)
        {
            _request.sender.ClearRequst(_request);
        }
    }
}

public class FillStationPlateTask:WorkerTask
{
    public FoodSpawnStation TargetStation { get; private set; }

    public FillStationPlateTask(Worker _worker, FoodSpawnStation _foodSpawnStation, Food_Drop_Zone _dropZone, TaskRequst _request)
        :base(_worker,_request)
    {
        TargetStation = _foodSpawnStation;

        HoldItemAbility holdItemAbility = _worker.GetComponent<HoldItemAbility>();
        
        _subtasks.Add(new MoveToSubtask(_foodSpawnStation.WorkerTargetPosition));
        _subtasks.Add(new GrabFoodSubtask(holdItemAbility, _foodSpawnStation));
        _subtasks.Add(new MoveToSubtask(_dropZone.WorkerTargetPoint));
        _subtasks.Add(new DropFoodSubtask(holdItemAbility, _dropZone));
    }
}

public class ServeCustomersTask : WorkerTask
{
    public Food_Drop_Zone TargetDropZone => (_request as ServeCustomersRequest)?.DropZone;

    public ServeCustomersTask(Worker _worker, TaskRequst _request, Vector3 _targetPosition):base(_worker,_request)
    {
        _subtasks.Add(new MoveToSubtask(_targetPosition));
        _subtasks.Add(new RotateSubtask(-90f));
        _subtasks.Add(new WaitSubtask(1f));
        _subtasks.Add(new waitForConditionSubTask(()=>(_request as ServeCustomersRequest).DropZone.FoodCount<=0));
        
    }
}

public class idleTask:WorkerTask
{
    public idleTask(Worker _worker,Vector3 _targetposition,TaskRequst _request):base(_worker,_request)
    {
        _subtasks.Add(new MoveToSubtask(_targetposition));
        _subtasks.Add(new waitForConditionSubTask(()=>false));
    }
}

public abstract class Subtask
{
    public bool IsComplete;

    public abstract void start(Worker _worker);
    public abstract void update(Worker _worker);
}

public class MoveToSubtask : Subtask
{
    private Vector3 destination;
    public MoveToSubtask(Vector3 _destination)=> destination = _destination;
    
    public override void start(Worker _worker)
    {
        _worker.GoTo(destination);
    }

    public override void update(Worker _worker)
    {
        if (_worker.HasReachedDestination)
        {
            IsComplete = true;
        }
    }
}

public class waitForConditionSubTask : Subtask
{
    private Func<bool> condition;
    public waitForConditionSubTask(Func<bool> _condition)
    {
        condition = _condition;
    }
    
    public override void start(Worker _worker)
    {
        _worker.markAsBusy();
    }
    public override void update(Worker _worker)
    {
        if (condition())
        {
            IsComplete = true;
        }
    }
}

public class GrabFoodSubtask : Subtask
{
    private HoldItemAbility _holdItemAbility;
    private FoodSpawnStation _station;
    
    public GrabFoodSubtask(HoldItemAbility holdItemAbility, FoodSpawnStation station)
    {
        _holdItemAbility = holdItemAbility;
        _station = station;
    }
    
    public override void start(Worker _worker)
    {
        _worker.markAsBusy();
    }
    
    public override void update(Worker _worker)
    {
        _holdItemAbility.handleFoodTrigger(_station);
        if (_worker.IsPlateFull && !_worker.IsPlateDirty)
        {
            IsComplete = true;
        }
    }
}

public class DropFoodSubtask : Subtask
{
    private HoldItemAbility _holdItemAbility;
    private Food_Drop_Zone _dropZone;
    
    public DropFoodSubtask(HoldItemAbility holdItemAbility, Food_Drop_Zone dropZone)
    {
        _holdItemAbility = holdItemAbility;
        _dropZone = dropZone;
    }
    
    public override void start(Worker _worker)
    {
        _worker.markAsBusy();
    }
    
    public override void update(Worker _worker)
    {
        _holdItemAbility.handleFoodDropZone(_dropZone);
        if (_worker.IsPlateEmpty || _dropZone.isFull)
        {
            IsComplete = true;
        }
    }
}

public class WaitSubtask : Subtask
{
    private float _duration;
    private float _timer;
    
    public WaitSubtask(float duration)
    {
        _duration = duration;
    }
    
    public override void start(Worker _worker)
    {
        _timer = 0;
        _worker.markAsBusy();
    }
    
    public override void update(Worker _worker)
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            IsComplete = true;
        }
    }
}

public class RotateSubtask : Subtask
{
    private Quaternion _targetRotation;
    private float _rotationSpeed;
    
    public RotateSubtask(float angleDegrees, float speed = 360f)
    {
        _rotationSpeed = speed;
    }
    
    public override void start(Worker _worker)
    {
        _targetRotation = _worker.transform.rotation * Quaternion.Euler(0, -90, 0);
        _worker.markAsBusy();
    }
    
    public override void update(Worker _worker)
    {
        _worker.transform.rotation = Quaternion.RotateTowards(
            _worker.transform.rotation, 
            _targetRotation, 
            _rotationSpeed * Time.deltaTime
        );
        
        if (Quaternion.Angle(_worker.transform.rotation, _targetRotation) < 0.5f)
        {
            _worker.transform.rotation = _targetRotation;
            IsComplete = true;
        }
    }
}

public class CleanTableTask : WorkerTask
{ 
    public TableSet table => (_request as CleanTableRequest).TableSet;
    public CleanTableTask(Worker _worker, TableSet _table,Trash _trash,TaskRequst _request):base(_worker,_request)
    {
        _subtasks.Add(new MoveToSubtask(_table.WorkerTargetPosition));
        _subtasks.Add(new waitForConditionSubTask(()=>!_table.IsDirty));  
        _subtasks.Add(new MoveToSubtask(_trash.WorkerTargetPosition));
    }
}
