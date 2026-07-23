using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class EdgeDetectionBuiltIn : MonoBehaviour
{
    public Shader edgeDetectionShader;
    [Range(0, 15)] public int outlineThickness = 3;
    public Color outlineColor = Color.black;

    private Material material;

    private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
    private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");

    private void OnEnable()
    {
        // 强制开启相机的深度和法线纹理图生成（边缘检测通常需要）
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
    }

    private void OnDisable()
    {
        if (material != null)
        {
            DestroyImmediate(material);
        }
    }

    // 内置管线经典的屏幕后处理回调
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (edgeDetectionShader == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (material == null || material.shader != edgeDetectionShader)
        {
            material = new Material(edgeDetectionShader);
            material.hideFlags = HideFlags.HideAndDontSave;
        }

        material.SetFloat(OutlineThicknessProperty, outlineThickness);
        material.SetColor(OutlineColorProperty, outlineColor);

        // 执行后处理 Blit
        Graphics.Blit(source, destination, material);
    }
}