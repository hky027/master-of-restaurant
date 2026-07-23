using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TaskRequst
{
    public TaskRequster sender;
    protected string guid;
    public string Guid=> guid;
    protected int priority;
    public int Priority=> priority;
    
    public TaskRequst()
    {
        
    }
    
    public TaskRequst(TaskRequster sender) => this.sender = sender;
}

public class FillStationPlateRequest : TaskRequst
{
    private Food_Drop_Zone _dropZone;
    public Food_Drop_Zone DropZone => _dropZone;
    private SpawnableFood _food;
    public SpawnableFood Food => _food;
    
    public FillStationPlateRequest(string guid, SpawnableFood food, Food_Drop_Zone dropZone)
    {
        this.guid = guid;
        this._food = food;
        this._dropZone = dropZone;
        priority = 70;
    }
}

public class ServeCustomersRequest: TaskRequst
{
    private Vector3 _workerTargetPosition;
    public Vector3 WorkerTargetPosition => _workerTargetPosition;
    private Food_Drop_Zone _dropZone;
    public Food_Drop_Zone DropZone => _dropZone;
    
    public ServeCustomersRequest(string guid, Vector3 workerTargetPosition, Food_Drop_Zone dropZone)
    {
        this.guid = guid;
        this._workerTargetPosition = workerTargetPosition;
        this._dropZone = dropZone;
        priority = 40;
    }
}

public class CleanTableRequest : TaskRequst
{
    private TableSet tableSet;
    public TableSet TableSet => tableSet;
    
    public CleanTableRequest(string guid, TableSet tableSet)
    {
        this.guid = guid;
        this.tableSet = tableSet;
        priority = 50;
    }
}

public class IdleRequest: TaskRequst
{
    private Vector3 targetPosition;
    public Vector3 TargetPosition => targetPosition;

    public IdleRequest(string guid, Vector3 _targetPosition)//构造函数
    {
        this.guid = guid;
        this.targetPosition = _targetPosition;

        priority = -1;
    }
    
}
