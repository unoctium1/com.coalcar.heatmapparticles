using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct SmallVector3
{
    const string f = "({0},{1},{2})";
    //Set precision based on how large the world is. For 10f, it cannot be larger than 3000 units in any direction
    const float precision = 10f;
    public float _x;
    public float _y;
    public float _z;

    public SmallVector3(float x, float y, float z)
    {
        (_x, _y, _z) = (Round(x), Round(y), Round(z));
    }

    public SmallVector3(Vector3 vec3)
    {
        _x = Round(vec3.x);
        _y = Round(vec3.y);
        _z = Round(vec3.z);
    }

    public Vector3 GetVector3()
    {
        return new Vector3(_x, _y, _z);
    }

    private static float Round(float f)
    {
        return Mathf.Round(f * precision) / precision;
    }

    public override string ToString()
    {
        return string.Format(f, _x, _y, _z);
    }

    public void Save(DataWriter writer)
    {
        writer.Write((short)(_x * precision));
        writer.Write((short)(_y * precision));
        writer.Write((short)(_z * precision));
    }

    public static SmallVector3 Load(DataReader reader)
    {
        return new SmallVector3
        {
            _x = reader.ReadShort() / precision,
            _y = reader.ReadShort() / precision,
            _z = reader.ReadShort() / precision
        };
    }
}
