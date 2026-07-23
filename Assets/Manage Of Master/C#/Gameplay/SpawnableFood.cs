using System;
using UnityEngine;

public abstract class SpawnableFood : MonoBehaviour
{
    [Header("element")] 
    [SerializeField] private new Renderer _renderer;
    [SerializeField] private MeshFilter filter;
    [SerializeField] private Mesh dirtyMesh;
    
    [Header("settings")]
    [SerializeField] private float cleanYoffsetOnPlate;
    public float _cleanYoffsetOnPlate => cleanYoffsetOnPlate;//=>只读属性，外部脚本只能读取，无法更改。
    
    [SerializeField] private float dirtyYoffsetOnPlate;
    public float _dirtyYoffsetOnPlate => dirtyYoffsetOnPlate;
    private bool isDirty;
    public bool IsDirty=> isDirty;
    public bool Isvisible=>_renderer.enabled;

    public void MarkAsDirty()
    {
        isDirty = true;
        filter.mesh = dirtyMesh;
    }

    public void display()
    {
       _renderer.enabled = true;
    }

    public void hide()
    {
       _renderer.enabled = false;
    }
}