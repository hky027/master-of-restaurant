using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavigationAbility : MonoBehaviour
{
    [Header("component")] 
    private NavMeshAgent _agent;

    public Vector3 velocity => _agent.velocity;

    public bool TryGoToPosition(Vector3 targetpositon)
    {
        targetpositon.y = 0;
        NavMeshPath path = new NavMeshPath();
        bool reachAble = _agent.CalculatePath(targetpositon, path);
        if (!reachAble)
        {
            return false;
        }
        else
        {
            _agent.SetPath(path);
            _agent.isStopped = false;
            return true;
        }
        
    }
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        
        // 禁用工人之间的避让，让他们可以互相穿过
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    public bool IsMoving()
    {
        if (_agent.pathPending)
        {
            return true;
        }

        if (_agent.hasPath && _agent.remainingDistance>_agent.stoppingDistance)
        {
            return true;
        }

        return _agent.velocity.sqrMagnitude > 0f;
    }

    public bool HasReachedDestination()
    {
        if (_agent.pathPending)
        {
            return false;
        }

        if (_agent.remainingDistance>_agent.stoppingDistance)
        {
            return false;
        }

        if (_agent.hasPath && _agent.velocity.sqrMagnitude!=0)
        {
            return false;
        }

        return true;
    }

    public void Stop()
    {
        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }

    public void disable()
    {
        _agent.enabled = false;
    }

    public void enable()
    {
        _agent.enabled = true;
    }
}
