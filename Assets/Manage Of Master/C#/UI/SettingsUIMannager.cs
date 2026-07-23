using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour
{
    [Header("ui面板")]
    public GameObject settingsPanel;

    [Header("ui按钮")]
    public Button openSettingsButton;
    public Button returnButton;
    
    [Header("背景音乐控制")]
    public Button bgmButton;
    public Button CloseButton;
    public AudioSource bgmAudioSource;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        //打开设置
        if (openSettingsButton != null)
        {
            openSettingsButton.onClick.AddListener(OpenSettings);
        }

        //关闭设置
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(CloseSettings);
        }

        //打开关闭背景音乐
        if (bgmButton != null)
        {
            bgmButton.onClick.AddListener(CloseBGM);
        }
        
        if (CloseButton != null)
        {
            CloseButton.onClick.AddListener(ToggleBGM);
        }
    }

    //打开设置按钮
    private void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    //关闭设置按钮
    private void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    //关BGM
    private void CloseBGM()
    {
        if (bgmAudioSource != null)
        {
            // 切换静音状态：如果现在是静音就取消静音，反之亦然
            bgmAudioSource.mute = !bgmAudioSource.mute; 
            bgmButton.gameObject.SetActive(false);
            CloseButton.gameObject.SetActive(true);
        }
    }
    //开BGM
    private void ToggleBGM()
    {
        if (bgmAudioSource != null)
        {
            //切换静音状态，如果现在是静音就取消静音，反之亦然
            bgmAudioSource.mute = !bgmAudioSource.mute; 
            CloseButton.gameObject.SetActive(false);
            bgmButton.gameObject.SetActive(true);
        }
    }
    
    private void OnDestroy()
    {
        // 退出或销毁时清理事件，防止报错
        if (openSettingsButton != null) openSettingsButton.onClick.RemoveAllListeners();
        if (returnButton != null) returnButton.onClick.RemoveAllListeners();
        if (bgmButton != null) bgmButton.onClick.RemoveAllListeners();
    }

    public void ChangeTheScene(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }
}