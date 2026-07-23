using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskRequster : MonoBehaviour
{
    [SerializeReference] List<TaskRequst> _requsts = new List<TaskRequst>();

    public void createTaskRequest(TaskRequst _Request)
    {
        _Request.sender = this;
        
        foreach (TaskRequst item in _requsts)
        {
            if (_Request is CleanTableRequest)
            {
                // CleanTableRequest: 按GUID+类型去重，不同桌子视为不同请求
                if (item.GetType() == _Request.GetType() && item.Guid == _Request.Guid)
                    return;
            }
            else
            {
                // 其他请求: 按类型去重
                if (item.GetType() == _Request.GetType())
                    return;
            }
        }
        
        _requsts.Add(_Request);
        
        WorkerMannager.RegisterRequest(_Request);
    }
    
    public void ClearRequst(TaskRequst _Request)
    {
        for (int i = _requsts.Count-1; i >= 0; i--)
        {
            if (_requsts[i].Guid == _Request.Guid && _requsts[i].GetType() == _Request.GetType())
            {
                _requsts.RemoveAt(i);
                break;
            }
        }
    }
}
