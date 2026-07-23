using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Tabsil.Sijil;
using UnityEngine;

[RequireComponent(typeof(GuidGenerator))]
public class cash_pile : MonoBehaviour,IWantToBeSaved
{
    [Header("Elements")] 
    [SerializeField] private GameObject cashPrefab;
    [Header("components")] 
    private GuidGenerator guidGenerator;
    private bool isloaded;
    [Header("Settings")] 
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector3 gridSpacing;
    private Vector3[] basePositions;
    private int index;
    private void Awake()
    {
        guidGenerator = GetComponent<GuidGenerator>();
        StoreBasePosition();
    }
    private void Start()
    {
        if (!isloaded)
        {
            Load();
        }
    }
    [NaughtyAttributes.Button]
    private void GenerateCash()
    {
        GenerateCash(1);
    }
    public void GenerateCash(int amount,bool save = true)
    { 
        for (int i = 0; i < amount; i++)
        {
            Vector3 targetPosition = GetTargetGridPosition(transform.childCount);
            Instantiate(cashPrefab, targetPosition, Quaternion.identity,transform);
        }
        if (save)
        {
            Save();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _player))
        {
            return;
        }
        animateCashToPlayer(_player.transform);
        Save();
    }
    private void animateCashToPlayer(Transform playerTransform)
    {
        float duration = 2f;
        float delayStep = duration/transform.childCount;
        delayStep = Mathf.Max(delayStep, 0.01f);
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform cash = transform.GetChild(i);
            float delay = (transform.childCount - 1 - i) * delayStep;
            delay = Mathf.Max(delay, duration);
            ArcAnimator.Animate(cash, playerTransform, 0.2f, delay, 3f, () => 
            {
                if (cash != null)
                    handleCashMovedAlongArc(cash.gameObject);
            });
        }
    }
    private void handleCashMovedAlongArc(GameObject _cash)
    {
        CurrencyManager.instance.AddCurrency(1);
        Destroy(_cash.gameObject);
    }
    private void StoreBasePosition()
    {
        basePositions = new Vector3[gridSize.x * gridSize.y];
        Vector3 startPosition = transform.position 
                                - (Vector3.right * gridSpacing.x * gridSize.x / 2)
                                -(Vector3.forward * gridSpacing.z * gridSize.y / 2);
        startPosition += gridSpacing / 2;
        for (int z = 0; z < gridSize.y; z++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 targetPosition = startPosition + Vector3.right * x * gridSpacing.x+ Vector3.forward * z * gridSpacing.z;
                int i = x + z * gridSize.x;
                basePositions[i] = targetPosition;
            }
        }
    }
    private Vector3 GetTargetGridPosition(int index)
    {
        int elevationIndex = index / basePositions.Length;
        float y = elevationIndex * gridSpacing.y;
        int basePositionIndex = index % basePositions.Length;
        return basePositions[basePositionIndex]+Vector3.up*y;
    }
    public void Load()
    {
        isloaded = true;

        if (!Sijil.TryLoad(this,guidGenerator.GUID,out object _index))
        {
            return;
        }
        GenerateCash((int)_index , false);
    }
    public void Save()
    {
        Sijil.Save(this, guidGenerator.GUID, transform.childCount);
    }
}
