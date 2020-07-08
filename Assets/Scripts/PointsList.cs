using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PointsList : ScriptableObject
{
    public List<SmallVector3> points;

    public void Add(SmallVector3 point)
    {
        if (points == null) points = new List<SmallVector3>();
        points.Add(point);
    }

    public void Clear()
    {
        if (points == null) return;
        else points.Clear();
    }

    public int Count
    {
        get
        {
            if (points == null) points = new List<SmallVector3>();
            return points.Count;
        }
    }

    public SmallVector3 this[int val]
    {
        get
        {
            return points[val];
        }
        set
        {
            points[val] = value;
        }
    }

}
