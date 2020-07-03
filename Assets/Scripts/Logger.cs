using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    [SerializeField] float waitTime = 0.03f;

    //List<Vector3> points;

    [SerializeField] Text debugText = null;
    [SerializeField] bool log = true;

    [SerializeField] InputSource input;

    public LogEvent onLogEvent;

    delegate Vector3 GetInputPoint();
    GetInputPoint getter;

    private void Awake()
    {
        if(onLogEvent == null) onLogEvent = new LogEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (input == InputSource.mousePos) getter = GetMousePos;

        //points = new List<Vector3>();
        StartCoroutine(LogPos());
    }

    IEnumerator LogPos()
    {
        while (log)
        {
            Vector3 point = getter();

            onLogEvent.Invoke(point);
            if (debugText) debugText.text = point.ToString();
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    private static Vector3 GetMousePos()
    {
        return Input.mousePosition;
    }

}

public enum InputSource
{
    mousePos
}

[System.Serializable]
public class LogEvent : UnityEvent<Vector3>
{

}
