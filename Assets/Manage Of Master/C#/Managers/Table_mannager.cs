using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Table_mannager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private TaskRequster taskRequster;
    private TableSet[] _tableSet;
    [Header("Settings")] 
    private List<TableSet> dirtyTable = new List<TableSet>();

    [Header("Actions")] 
    public static Action<TableSet, HoldDishesAbility> tableCleaned;
    
    private void Awake()
    {
        _tableSet = GetComponentsInChildren<TableSet>(true);
    }

    public bool IsAnyActive()
    {
        return GetFirstCleanEmptyTable()!=null;
    }

    private TableSet GetFirstCleanEmptyTable()
    {
        for (int i = 0; i < _tableSet.Length; i++)
        {
            if (!_tableSet[i].gameObject.activeInHierarchy)
            {
                continue;
            }
            
            if (_tableSet[i].IsDirty)
            {
                continue;
            }
            
            if (_tableSet[i].IsFull)
            {
                continue;
            }
            
            return _tableSet[i];
        }
        
        Debug.LogWarning("没有找到空闲的桌子！");
        return null;
    }

    public void handleCustomerServed(Customer customer)
    {
        TableSet table = GetFirstCleanEmptyTable();
        if (table==null)
        {
            Debug.Log("没有空闲的桌子了");
            return;
        }
        
        table.AcceptCustomer(customer,this);
    }

    public void pushDirtyTable(TableSet _table)
    {
        dirtyTable.Add(_table);
        
        taskRequster.createTaskRequest(new CleanTableRequest(_table.guid, _table));
    }

    public void RemoveDirtyTable(TableSet _table, HoldDishesAbility holdDishesAbility)
    {
        dirtyTable.Remove(_table);
        
        taskRequster.ClearRequst(new CleanTableRequest(_table.guid,_table));
        tableCleaned?.Invoke(_table, holdDishesAbility);
    }
}
