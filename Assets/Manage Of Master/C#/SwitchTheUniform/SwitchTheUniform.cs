using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("角色/服装配置")]
    [SerializeField] private GameObject[] characterModels;
    [SerializeField] private PlayerAnimator playerAnimator;
    private int currentCharacterIndex = 0;

    [Header("盘子/道具配置")]
    [SerializeField] private GameObject[] plateModels;
    private int currentPlateIndex = 0;

    [Header("盘子功能引用(换装时同步更新)")]
    [SerializeField] private HoldItemAbility holdItemAbility;
    [SerializeField] private HoldDishesAbility holdDishesAbility;

    private void Start()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponent<PlayerAnimator>();

        // 游戏开始时，确保只有第一个角色模型和第一个盘子是显示状态
        UpdateModelVisibility();
        UpdatePlateVisibility();
    }

    /// <summary>
    /// 切换到指定索引的角色（适合UI按钮直接指定某套衣服）
    /// </summary>
    public void SwitchCharacterToIndex(int index)
    {
        if (index < 0 || index >= characterModels.Length || index == currentCharacterIndex) return;

        characterModels[currentCharacterIndex].SetActive(false);
        currentCharacterIndex = index;
        characterModels[currentCharacterIndex].SetActive(true);
        UpdateAnimatorReference();
    }

    /// <summary>
    /// 切换到指定索引的盘子/道具（适合UI盘子按钮直接调用）
    /// </summary>
    public void SwitchPlateToIndex(int index)
    {
        if (plateModels == null || plateModels.Length == 0) return;
        if (index < 0 || index >= plateModels.Length || index == currentPlateIndex) return;

        plateModels[currentPlateIndex].SetActive(false);
        currentPlateIndex = index;
        plateModels[currentPlateIndex].SetActive(true);

        // 同步更新各系统的盘子引用到新盘子
        UpdatePlateReferences();
    }

    /// <summary>
    /// 切换盘子后，同步更新 PlayerAnimator、HoldItemAbility、HoldDishesAbility 中的盘子引用
    /// </summary>
    private void UpdatePlateReferences()
    {
        GameObject currentPlate = plateModels[currentPlateIndex];
        Plate plateComponent = currentPlate != null ? currentPlate.GetComponent<Plate>() : null;

        if (playerAnimator != null)
            playerAnimator.SetPlateau(currentPlate);

        if (holdItemAbility != null && plateComponent != null)
            holdItemAbility.SetPlate(plateComponent);

        if (holdDishesAbility != null && plateComponent != null)
            holdDishesAbility.SetPlate(plateComponent);
    }

    // 根据索引更新角色模型的显隐
    private void UpdateModelVisibility()
    {
        if (characterModels == null || characterModels.Length == 0) return;

        for (int i = 0; i < characterModels.Length; i++)
        {
            if (characterModels[i] != null)
                characterModels[i].SetActive(i == currentCharacterIndex);
        }
        UpdateAnimatorReference();
    }

    // 根据索引更新盘子模型的显隐
    private void UpdatePlateVisibility()
    {
        if (plateModels == null || plateModels.Length == 0) return;

        for (int i = 0; i < plateModels.Length; i++)
        {
            if (plateModels[i] != null)
                plateModels[i].SetActive(i == currentPlateIndex);
        }
    }

    /// <summary>
    /// 暂时禁用当前手中的盘子（换装时调用）
    /// </summary>
    public void DisableCurrentPlate()
    {
        if (plateModels == null || plateModels.Length == 0) return;
        if (currentPlateIndex >= 0 && currentPlateIndex < plateModels.Length && plateModels[currentPlateIndex] != null)
        {
            plateModels[currentPlateIndex].SetActive(false);
        }
    }

    /// <summary>
    /// 重新启用当前盘子（退出换装时调用）
    /// </summary>
    public void EnableCurrentPlate()
    {
        if (plateModels == null || plateModels.Length == 0) return;
        if (currentPlateIndex >= 0 && currentPlateIndex < plateModels.Length && plateModels[currentPlateIndex] != null)
        {
            plateModels[currentPlateIndex].SetActive(true);
        }
    }

    // 抓取当前激活模型的Animator，并传递给PlayerAnimator
    private void UpdateAnimatorReference()
    {
        if (characterModels == null || characterModels.Length == 0) return;

        Animator activeAnimator = characterModels[currentCharacterIndex].GetComponent<Animator>();
        
        if (activeAnimator != null && playerAnimator != null)
        {
            playerAnimator.SetAnimator(activeAnimator);
        }
        else
        {
            Debug.LogWarning("换装系统：在新模型上没有找到Animator组件，或者PlayerAnimator未赋值！");
        }
    }
}