using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapParticle : PersistableObject
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

    public override void Save(DataWriter writer)
    {
        base.Save(writer);
        writer.Write(height);
    }

    public override void Load(DataReader read)
    {
        base.Load(read);
        height = read.ReadFloat();
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
