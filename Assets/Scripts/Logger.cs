using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Logger : PersistableObject
{
    //[SerializeField] float waitTime = 0.03f;

    List<SmallVector3> points;

    [SerializeField] Text debugText = null;
    [SerializeField, Tooltip("Set true to log on start")] bool log = false;
    [SerializeField] Camera cam;
    [SerializeField, Tooltip("Controls speed of polling")] float waitTime;
    float timer = 0.0f;
    [SerializeField] InputSource input;
    [SerializeField] bool realtime;

    public bool Realtime { get => realtime; set => realtime = value; }
    public bool Log { get => log; set => log = value; }

    public VectorEvent onLogEvent;
    public IntEvent onPrepPlayback;

    delegate bool GetInputPoint(Camera cam, out Vector3 point);
    GetInputPoint getter;

    private void Awake()
    {
        if(onLogEvent == null) onLogEvent = new VectorEvent();
        if (onPrepPlayback == null) onPrepPlayback = new IntEvent();
        points = new List<SmallVector3>();

        if (input == InputSource.mousePos) getter = GetMousePos;
    }

    public void Playback()
    {
        StartCoroutine(PlaybackPoints());
    }

    private void FixedUpdate()
    {
        if (log)
        {
            timer += Time.deltaTime;
            if(timer > waitTime)
            {
                if(getter(cam, out Vector3 point))
                {
                    SmallVector3 toSave = new SmallVector3(point);
                    if (realtime)
                    {
                        onLogEvent.Invoke(toSave.GetVector3());
                    }
                    else
                    {
                        points.Add(toSave);
                    }
                    debugText.text = toSave.ToString();
                }
                timer -= waitTime;
            }
        }
    }

    private IEnumerator PlaybackPoints()
    {
        onPrepPlayback.Invoke(points.Count);
        bool resetLog = false;
        if (log)
        {
            log = false;
            resetLog = true;
        }
        for(int i = 0; i < points.Count; i++)
        {
            yield return new WaitForFixedUpdate(); 
            onLogEvent.Invoke(points[i].GetVector3());

        }
        points.Clear();
        if (resetLog) log = true;
    }

    private static bool GetMousePos(Camera cam, out Vector3 point)
    {
        point = Input.mousePosition;
        point.z = cam.nearClipPlane;
        Ray r = cam.ScreenPointToRay(point);

        if (Physics.Raycast(r, out RaycastHit hit))
        {
            point = hit.point;
            return true;
        }
        else { return false; }
    }

    public override void Load(DataReader reader)
    {
        int count = reader.ReadInt();
        points = new List<SmallVector3>();
        for(int i = 0; i < count; i++)
        {
            points.Add(SmallVector3.Load(reader));
        }
    }

    public override void Save(DataWriter writer)
    {
        writer.Write(points.Count);
        for (int i = 0; i < points.Count; i++)
        {
            points[i].Save(writer);
        }

        
    }

    public void Clear()
    {
        points.Clear();
    }

}

public enum InputSource
{
    mousePos
}

[System.Serializable]
public class VectorEvent : UnityEvent<Vector3>
{

}

[System.Serializable]
public class IntEvent : UnityEvent<int>
{

}
