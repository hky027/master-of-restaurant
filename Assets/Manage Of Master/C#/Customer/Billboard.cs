using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 获取主相机
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 让气泡的朝向与相机的朝向保持一致
            transform.forward = mainCamera.transform.forward;
        }
    }
}