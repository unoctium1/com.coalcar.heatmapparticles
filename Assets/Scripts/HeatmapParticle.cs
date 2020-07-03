using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapParticle : MonoBehaviour
{

    static int heightProperty = Shader.PropertyToID("_Height");

    private new MeshRenderer renderer;
    private static MaterialPropertyBlock propertyBlock;
    [SerializeField, Range(0.01f, 1.0f)] float height;

    public float Height
    {
        get => height;
        set
        {
            height = Mathf.Clamp(value, 0.01f, 1.0f);
            SetHeightVal(height);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetHeightVal(height);
    }
#endif

    private void SetHeightVal(float val)
    {
        if (renderer == null) renderer = GetComponent<MeshRenderer>();
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat(heightProperty, val);
        renderer.SetPropertyBlock(propertyBlock);
    }
}
