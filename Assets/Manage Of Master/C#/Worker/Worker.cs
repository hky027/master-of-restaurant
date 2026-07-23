using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavigationAbility))]
public class Worker : MonoBehaviour
{
    enum State
    {
        Idle = 0,
        performingTask = 1
    }

    [Header("component")] 
    [SerializeField] private CustomerAnimator _animator;
    private NavigationAbility _navigationAbility;
    private HoldItemAbility _holdItemAbility;
    
    private static bool _collisionSetupDone;
    private const int WORKER_LAYER = 8;
    
    [Header("Task")]
    private WorkerTask _currentTask;
    public WorkerTask CurrentTask=>_currentTask;
    private State _state;
    
    public bool HasReachedDestination=> _navigationAbility.HasReachedDestination();
    
    public bool IsIdle => _state == State.Idle;

    public bool IsPlateEmpty=> _holdItemAbility.IsPlateEmpty;
    
    public bool IsPlateDirty=> _holdItemAbility.IsPlateDirty;
    public bool IsPlateFull => _holdItemAbility.IsPlateFull;

    private void Awake()
    {
        _navigationAbility = GetComponent<NavigationAbility>();
        _holdItemAbility = GetComponent<HoldItemAbility>();
        
        _state = State.Idle;
        
        SetupWorkerCollision();
    }

    private void SetupWorkerCollision()
    {
        // 将工人分配到固定层级
        gameObject.layer = WORKER_LAYER;
        
        // 只在第一次运行时禁用工人层级之间的碰撞
        if (!_collisionSetupDone)
        {
            _collisionSetupDone = true;
            Physics.IgnoreLayerCollision(WORKER_LAYER, WORKER_LAYER, true);
        }
    }

    private void StartWalkingState()
    {
        markAsBusy();
    }
    
    private void Update()
    {
        HandleStateMachine();
        
        if (_currentTask!=null)
        {
            _currentTask.update();
        }
    }

    private void HandleStateMachine()
    {
        switch (_state)
        {
         case State.Idle:
             handleIdleState();
             break;
         case State.performingTask:
             handlePerformingTaskState();
             break;
        }
    }

    private void handlePerformingTaskState()
    {
        if (_navigationAbility.HasReachedDestination())
        {
            _animator.stop();
            return;
        }

        if (_navigationAbility.IsMoving())
        {
            _animator.ManageAnimations(_navigationAbility.velocity);
        }
        else
        {
            startIdleState();
        }
    }

    private void ReachDestination()
    { 
        startIdleState();
    }

    private void handleIdleState()
    {
        if (_navigationAbility.IsMoving())
        {
            StartWalkingState();
        }
    }

    public void AssignTask(WorkerTask task)
    {
        _currentTask = task;
        _currentTask.start();
    }

    public void CompleteTask()
    {
        //Debug.Log("瀹屾垚浠诲姟");
        _currentTask = null;
        //_state = State.Idle;
        startIdleState();
    }

    private void startIdleState()
    {
        _state = State.Idle;
        _animator.stop();
    }

    public void GoTo(Vector3 destination)
    {
        bool canReachDestination = _navigationAbility.TryGoToPosition(destination);
        if (canReachDestination)
        {
            StartWalkingState();
        }
    }

    public void markAsBusy()
    {
        _state = State.performingTask;
    }

    public void cancelTask()
    {
        _currentTask.cancel();
        _navigationAbility.Stop();
        CompleteTask();
    }
}
