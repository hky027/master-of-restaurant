using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TaskRequster))]
[RequireComponent(typeof(GuidGenerator))]
public class WorkerIdelPosition : MonoBehaviour
{
    [Header("component")]
    private TaskRequster taskRequster;
    private GuidGenerator guidGenerator;

    [Header("settings")] 
    private float timer;

    private void Awake()
    {
        taskRequster = GetComponent<TaskRequster>();
        guidGenerator = GetComponent<GuidGenerator>();
    }
    
    void Update()
    {
        timer+=Time.deltaTime;
        if (timer>=2)
        {
            timer = 0;
            taskRequster.createTaskRequest(new IdleRequest(guidGenerator.GUID,transform.position));
        }
    }
}
