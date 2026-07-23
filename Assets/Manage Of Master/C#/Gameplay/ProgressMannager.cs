using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Tabsil.Sijil;

public class ProgressMannager : MonoBehaviour, IWantToBeSaved
{
    [Header("Elements")]
    [SerializeField] private GameObject _progressPanel;
    [SerializeField] private CinemachineVirtualCamera _progressCamera;
    
    [Header("Data")]
    [SerializeField] private ProgressionStep[] progressionSteps;
    private int progressionIndex;
    private const string ProgressStepIndexKey = "Progress_Index";

    private void Awake()
    {
        LockedElement.unlocked += OnLockedElementUnlocked;
    }

    private void OnLockedElementUnlocked(LockedElement Element)
    {
        if (progressionIndex >= progressionSteps.Length)
        {
            return;
        }

        if (!progressionSteps[progressionIndex].Contains( Element))
        {
            Debug.Log("无锁定元素");
            return;
        }
        progressionSteps[progressionIndex].Unlock(Element);
        if (!progressionSteps[progressionIndex].IsComplete())
        {
            return;
        }
        
        progressionIndex++;
        Save();

        if (progressionIndex >= progressionSteps.Length)
        {
            return;
        }

        StartNextStep();
    }

    private void StartNextStep()
    {
        _progressPanel.SetActive(true);
        
        ProgressionStep step = progressionSteps[progressionIndex];
        step.Show();
        StartCoroutine(StartStepCoroutine(step));
    }

    IEnumerator StartStepCoroutine(ProgressionStep _step)
    {
        for (int i = 0; i < _step.lockedElements.Count; i++)
        {
            Transform targetTransform = _step.lockedElements[i].transform;
            _progressCamera.Follow = targetTransform;
            _progressCamera.LookAt = targetTransform;
            _progressCamera.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
        }
        _progressCamera.gameObject.SetActive(false);
        _progressPanel.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        LockedElement.unlocked -= OnLockedElementUnlocked;
    }

    public void Load()
    {
        if (Sijil.TryLoad(this,ProgressStepIndexKey,out object _progressionIndex))
        {
            progressionIndex = (int)_progressionIndex;
        }

        if (progressionIndex >= progressionSteps.Length)
        {
            return;
        }

        for (int i = progressionIndex; i < progressionSteps.Length; i++)
        {
            progressionSteps[i].Hide();
        } 
        ProgressionStep step = progressionSteps[progressionIndex];
        step.Show();
    }

    public void Save()
    {
        Sijil.Save(this,ProgressStepIndexKey, progressionIndex);
    }
}

[System.Serializable]
public struct ProgressionStep
{
    public string Name;
    public List<LockedElement> lockedElements;
    private List<LockedElement> unlockedElements;
    
    public void Show()
    {
        for (int i = 0; i < lockedElements.Count; i++)
        {
            lockedElements[i].gameObject.SetActive(true);
        }
        
    }

    public void Hide()
    {
        for (int i = 0; i < lockedElements.Count; i++)
        {
            lockedElements[i].gameObject.SetActive(false); 
        }
    }

    public bool Contains(LockedElement element) => lockedElements.Contains(element);

    public void Unlock(LockedElement element)
    {
        if (unlockedElements == null)
        {
            unlockedElements = new List<LockedElement>();
        }
        
        unlockedElements.Add(element);
    }

    public bool IsComplete()=> unlockedElements.Count >= lockedElements.Count;

}
