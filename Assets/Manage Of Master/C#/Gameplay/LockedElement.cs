using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tabsil.Sijil;
using Unity.Collections;

[RequireComponent(typeof(GuidGenerator))]

public class LockedElement : MonoBehaviour, IWantToBeSaved
{
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI PriceText;
    [SerializeField] private Transform anim;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject unlockedElement;

    [Header("Element")] 
    private GuidGenerator guidGenerator;
    
    [Header("Settings")]
    [SerializeField] private int initialPrice;
    private int currentPrice;
    private const string currentPriceKey = "locked_element_current_price";
    private bool Loaded;
    
    [Header("Actions")]
    public static Action<LockedElement> unlocked;
    private void Awake()
    {
        guidGenerator = GetComponent<GuidGenerator>();
        
        currentPrice = initialPrice;
        PriceText.text = currentPrice.ToString();
        if (unlockedElement!=null)
        {
            unlockedElement.SetActive(false);
        }
    }

    private void Start()
    {
        if (!Loaded)
        {
            Load();
        }
    }

    public void collectCash(int amount)
    {
        currentPrice -= amount;
        UpdateVisuals();
        if (currentPrice<=0)
        {
            unlock();
        }
        Save();
    }

    private void UpdateVisuals()
    {
        float fillAmount = 1-((float)currentPrice / initialPrice); 
        fillImage.fillAmount = fillAmount;
        PriceText.text = currentPrice.ToString();
    }
    
    private void unlock(bool instance =false )
    {
        Debug.Log("解锁");
        currentPrice = 0;
        anim.gameObject.SetActive(false);
        if (unlockedElement!=null)
        {
            unlockedElement.SetActive(true);
        }

        if (!instance)
        { 
            unlocked?.Invoke(this);
            Save();
        }
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PayAbility _payAbility))
        {
            return;
        }
        ScaleUp();
    }
    private void ScaleUp()
    {
        LeanTween.cancel(anim.gameObject);
        LeanTween.scale(anim.gameObject, Vector3.one * 1.2f, 0.2f)
            .setEase(LeanTweenType.easeOutCubic);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out PayAbility _payAbility))
        {
            return;
        }

        if (_payAbility.isMoving)
        {
            return;
        }
        
        float wholeAnimationDuration = 1f;
        float animationDuration = wholeAnimationDuration * ((float)currentPrice / initialPrice);

        int frameDuration = Mathf.CeilToInt(animationDuration * Application.targetFrameRate);
        frameDuration = Mathf.Max(frameDuration,1);
        
        int cashPerFrame = Mathf.CeilToInt((float)currentPrice/frameDuration);
       
        _payAbility.pay(this, cashPerFrame);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PayAbility _payAbility))
        {
            return;
        }
        ScaleDown();
    }
    private void ScaleDown()
    {
        LeanTween.cancel(anim.gameObject);
        LeanTween.scale(anim.gameObject, Vector3.one, 0.2f)
            .setEase(LeanTweenType.easeOutCubic);  
    }

    public void Load()
    {
        Loaded = true;
        
        string guid =guidGenerator.GUID;

        if (Sijil.TryLoad(this, guid ,out object unlocked))
        {
            unlock(true);
        }
        else
        {
            if (Sijil.TryLoad(this,guid+currentPriceKey,out object _currentPrice))
            {
             currentPrice = (int)_currentPrice;
             UpdateVisuals();
            }
        }
    }

    public void Save()
    {
        string guid =guidGenerator.GUID;

        if (currentPrice>0)
        {
            Sijil.Save(this,guid+currentPriceKey, currentPrice);
        }
        else
        {
            Sijil.Save(this,guid, true);
        }
    }
}
