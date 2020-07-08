using UnityEngine;
using System.Collections;

public class ShaderHeatmap : MonoBehaviour
{

    public Vector4[] positions;
    public Vector4[] properties;

    public Material material;

    public int count = 50;
    private int index = 0;

    void Awake()
    {
        positions = new Vector4[count];
        properties = new Vector4[count];

        for (int i = 0; i < positions.Length; i++)
        {
            //positions[i] = new Vector4(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), 0, 0);
            positions[i] = Vector4.zero;
            properties[i] = new Vector4(Random.Range(0f, 0.25f), Random.Range(-0.25f, 1f), 0, 0);
        }
    }

    void Update()
    {
        //for (int i = 0; i < positions.Length; i++)
           // positions[i] += new Vector4(Random.Range(-0.1f, +0.1f), Random.Range(-0.1f, +0.1f), 0, 0) * Time.deltaTime;

        material.SetInt("_Points_Length", count);
        material.SetVectorArray("_Points", positions);
        material.SetVectorArray("_Properties", properties);
    }

    public void AddNewPoint(Vector4 point)
    {
        //Debug.Log(index);
        positions[index] = point;
        index++;
        if (index >= count) index = 0;
    }

    public void ProcessMousePoint(Vector3 point)
    {
        Debug.Log(point);
        if (point.x < 0 || point.x > Screen.width || point.y < 0 || point.y > Screen.height) return;
        Vector4 vec4 = new Vector3(
            Map(point.x, Screen.width, 0, 1f, -1f),
            Map(point.y, Screen.height, 0, 1f, -1f),
            0);
        AddNewPoint(vec4);
    }

    private static float Map(float val, float oldMax, float oldMin, float newMax, float newMin)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(oldMin, oldMax, val));
    }
}