using System;
using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;

public class Customer : MonoBehaviour
{
    enum state
    {
        idle = 0,
        walking = 1,
        drinking = 2,
        eating = 3,
    }

    [Header("component")]
    [SerializeField] private CustomerAnimator _animator;
    [SerializeField] private NavigationAbility _navigationAbility;
    [SerializeField] private Plate _plate;
    
    [Header("settings")]
    private Vector3 finalFacePositon;
    
    private int foodNeededCount;
    public int FoodNeededCount=>foodNeededCount;
    
    private int foodTakenCount;
    public int FoodTakenCount=>foodTakenCount;

    private bool isLining;
    public bool IsLining=>isLining;

    public void StartWaiting()
    {
        isLining = true;
    }

    public void StopWaiting()
    {
        isLining = false;
    }
    
    [Header("Action")]
    private Action ReachedDestinationCallBack;
    
    private state _state;//使用枚举类型，需要声明一个枚举类型变量才能用。
    
    private void Update()
    {
        handleStateMachine();
    }

    private void handleStateMachine()
    {
        switch (_state)
        {
            case state.idle:
                handleIdelState();
                break;
            
            case state.walking:
                handleWalkState();
                break;
            
            case state.drinking:
                break;
        }
    }

    private void handleWalkState()
    {
        if (_navigationAbility.HasReachedDestination())
        {
            ReachDestination();
            return;
        }

        if (_navigationAbility.IsMoving())
        {
            _animator.ManageAnimations(_navigationAbility.velocity);
        }
        else
        {
            StartIdelState();
        }
    }

    private void ReachDestination()
    {
        StartIdelState();
        if (ReachedDestinationCallBack!=null)
        {
            ReachedDestinationCallBack?.Invoke();
            ReachedDestinationCallBack = null;
        }
    }
    
    private void handleIdelState()
    {
        if (_navigationAbility.IsMoving())
        {
            StartWalkingState();
        }
    }
    
    private void Awake()
    {
        _navigationAbility = GetComponent<NavigationAbility>();
    }

    public void initialize(int _foodNeedCount ,Vector3 targetpositon,Vector3 _FinalFacePosition)
    {
        this.foodNeededCount = _foodNeedCount;
        this.finalFacePositon = _FinalFacePosition;
        GotoThen(targetpositon,FaceFinalFacing);
    }
    
    private void FaceFinalFacing()
    {
        _animator.Face(finalFacePositon);
    }
    
    public void GotoThen(Vector3 targetpositon,Action callback)
    {
        ReachedDestinationCallBack = callback;
        Goto(targetpositon);
    }

    public void Goto(Vector3 targetpositon)
    {
        bool canReachDestination = _navigationAbility.TryGoToPosition(targetpositon);
        if (canReachDestination)
        {
            StartWalkingState();
        }
    }

    private void StartWalkingState()
    {
        _state = state.walking;
        _animator.startwalking();
    }

    private void StartIdelState()
    {
        _state = state.idle;
        _animator.stop();
    }

    public bool NeedsMoreFood()
    {
        return foodTakenCount < foodNeededCount;
    }

    public void CollectFood(SpawnableFood foodToServe)
    {
        _plate.gameObject.SetActive(true);
        _plate.push(foodToServe);
        foodTakenCount++;
    }

    public SpawnableFood pop()
    {
        SpawnableFood food = _plate.pop();

        if (food == null)
        {
            return null;
        }

        if (_plate.IsEmpty)
        {
            _plate.gameObject.SetActive(false);
        }

        return food;
    }

    public void HidePlate()
    {
        _plate.gameObject.SetActive(false);
    }

    public void SitDown(Vector3 sitPointPosition, Vector3 transformForward)
    {
        disAbleNavigation();
        
        transform.position = sitPointPosition.With(y:0);
        startDrikingState(transformForward);
    }

    private void startDrikingState(Vector3 forward)
    {
        _state = state.drinking;

        _animator.playsitDownAnimation(forward);
    }

    private void disAbleNavigation()
    {
        _navigationAbility.disable();
    }

    private void enabledNavigation()
    {
       _navigationAbility.enable();
    }
    
    public void getUpAndGo(Vector3 position, Action callBack)
    {
        enabledNavigation();
        GotoThen(position,callBack);
    }
    
}
