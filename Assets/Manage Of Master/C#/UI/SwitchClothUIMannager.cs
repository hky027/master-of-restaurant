using UnityEngine;
using UnityEngine.UI;

public class DressUpManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera mainCamera;
    public Camera dressUpCamera;

    [Header("UI Panels")]
    public GameObject dressUpUIPanel;

    [Header("UI Buttons (防连点保护)")]
    public Button enterDressUpButton;
    public Button returnButton;

    [Header("Character Teleportation")]
    public Transform playerCharacter;
    public Transform dressUpStage;
    
    [Header("Character System References")]
    public CharacterSwitcher characterSwitcher; 

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private CharacterController characterController;

    private bool isDressUpZone = false;

    private void Start()
    {
        if (dressUpCamera != null) dressUpCamera.gameObject.SetActive(false);
        if (dressUpUIPanel != null) dressUpUIPanel.SetActive(false);
        if (playerCharacter != null) characterController = playerCharacter.GetComponent<CharacterController>();
        
        if (characterSwitcher == null && playerCharacter != null)
        {
            characterSwitcher = playerCharacter.GetComponent<CharacterSwitcher>();
        }

        // 【修改】游戏开始时，确保按钮一直显示在屏幕上，但处于不可点击（失活）状态
        if (enterDressUpButton != null) 
        {
            enterDressUpButton.gameObject.SetActive(true);
            enterDressUpButton.interactable = false;
        }
    }
    
    // ---------- 触发器检测逻辑 ----------
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == playerCharacter)
        {
            isDressUpZone = true;
            if (enterDressUpButton != null)
            {
                enterDressUpButton.interactable = true; // 【修改】角色进入区域，按钮恢复可点击
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == playerCharacter)
        {
            isDressUpZone = false;
            if (enterDressUpButton != null)
            {
                enterDressUpButton.interactable = false; // 【修改】角色离开区域，按钮变为不可点击（通常变灰）
            }
        }
    }

    public void OpenDressUpUI()
    {
        // 进入换装界面时，让进入按钮不可按（防连点/逻辑保护）
        if (enterDressUpButton != null) enterDressUpButton.interactable = false;

        if (playerCharacter != null)
        {
            originalPosition = playerCharacter.position;
            originalRotation = playerCharacter.rotation;
            
            Vector3 forwardDir = playerCharacter.forward;
            Vector3 targetDir = Vector3.left;
            float rotationOffset = Vector3.SignedAngle(forwardDir, targetDir, Vector3.up);
            
            if (characterController != null) 
                characterController.enabled = false;
            
            playerCharacter.position = dressUpStage.position;
            Quaternion offsetRotate = Quaternion.Euler(0, rotationOffset, 0);
            playerCharacter.rotation = originalRotation * offsetRotate;
        }

        mainCamera.gameObject.SetActive(false);
        dressUpCamera.gameObject.SetActive(true);
        dressUpUIPanel.SetActive(true);

        if (returnButton != null) returnButton.interactable = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseDressUpUI()
    {
        if (returnButton != null) returnButton.interactable = false;

        if (playerCharacter != null)
        {
            if (characterController != null) characterController.enabled = false;
            playerCharacter.position = originalPosition;
            playerCharacter.rotation = originalRotation;
            if (characterController != null) characterController.enabled = true;
        }

        dressUpCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        dressUpUIPanel.SetActive(false);

        // 【修改】关闭换装UI后，根据玩家当前是否还在检测框内，决定按钮是否可点击
        if (enterDressUpButton != null)
        {
            enterDressUpButton.interactable = isDressUpZone;
        }

    }
    
    public void OnClothesButtonClicked(int index)
    {
        if (characterSwitcher != null)
        {
            characterSwitcher.SwitchCharacterToIndex(index);
            characterSwitcher.SwitchPlateToIndex(index);
            // 切换完装扮和盘子后，先暂时禁用手中的盘子
            characterSwitcher.DisableCurrentPlate();
        }
        else
        {
            Debug.LogError("DressUpManager找不到角色装扮");
        }
    }
}