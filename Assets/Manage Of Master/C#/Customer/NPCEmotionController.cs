using System;
using System.Collections;
using UnityEngine;

public class NPCEmotionController : MonoBehaviour
{
    [Header("气泡组件")]
    public GameObject emotionBubble;
    public Sprite happySprite;
    public Sprite angrySprite;
    public SpriteRenderer emotionIconRenderer;
    
    [Tooltip("这个是顾客进门多少秒才开始点好菜然后计时")]
    [SerializeField] private float OderTimedelay;
    private float OderTimer;
    
    private float gaptime = 4f;
    private float gaptimeTimer;
    
    [Tooltip("这个是顾客等待多少秒才会生气")]
    [SerializeField] private float waitingTimeDelay;
    private float waitingTimer = 0f;
    
    private Customer customer;

    private bool isAngry = false;
    
    // 当前正在运行的协程，用于打断旧的气泡显示
    private Coroutine currentEmotionCoroutine;

    private void Awake()
    {
        customer = GetComponent<Customer>();
    }
    
    public void ShowEmotion(Sprite emotionSprite, float displayTime = 2f)
    {
        // 如果当前已经有气泡在显示，先停止它
        if (currentEmotionCoroutine != null)
        {
            StopCoroutine(currentEmotionCoroutine);
        }

        // 启动新的气泡显示流程
        currentEmotionCoroutine = StartCoroutine(EmotionRoutine(emotionSprite, displayTime));
    }

    private IEnumerator EmotionRoutine(Sprite emotionSprite, float displayTime)
    {
        //替换情绪图片
        if (emotionIconRenderer != null)
        {
            emotionIconRenderer.sprite = emotionSprite;
        }

        //显示气泡
        if (emotionBubble != null)
        {
            emotionBubble.SetActive(true);
        }

        //等待指定的时间
        yield return new WaitForSeconds(displayTime);

        //隐藏气泡
        if (emotionBubble != null)
        {
            emotionBubble.SetActive(false);
        }
    }

    private void Update()
    {
        initailEmotionDisplay();
    }

    private void initailEmotionDisplay()
    {
        if (customer == null || !customer.IsLining)//实现如果顾客不在排队,就不再显示心情图标
            return;

        if (OderTimer < OderTimedelay)//实现让顾客刚走进店时先不显示心情
        {
            OderTimer += Time.deltaTime;
            return;
        }
        
        if (!isAngry)
        {
            waitingTimer += Time.deltaTime;
        }
        
        //实现判断是否是生气，不生气就循环播放笑脸，生气了循环播放生气脸
        if (!isAngry)
        {
            if (waitingTimer >= waitingTimeDelay)
            {
                Debug.Log("开始生气了");
                isAngry = true;
            }
            
            if (gaptimeTimer < gaptime)
            {
                gaptimeTimer += Time.deltaTime;
            }
            else
            {
                ShowEmotion(happySprite, 3f);
                gaptimeTimer = 0f;
            }
        }
        else
        {
            if (gaptimeTimer < gaptime)
            {
                gaptimeTimer += Time.deltaTime;
            }
            else
            {
                ShowEmotion(angrySprite, 3f);
                gaptimeTimer = 0f;
            }
        }
    }
    
}
