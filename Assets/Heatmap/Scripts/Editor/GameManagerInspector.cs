using UnityEditor;
using HeatmapParticles;
using UnityEngine;
using Logger = HeatmapParticles.Logger;
using System.IO;
using System.Collections.Generic;

[CustomEditor(typeof (GameManager)), CanEditMultipleObjects]
public class GameManagerInspector : Editor
{

    private Dictionary<SmallVector3, int> pointsDict;

    private SerializedProperty logger;
    private SerializedProperty system;
    private PointsList pointsList;

    public void OnEnable()
    {
        logger = serializedObject.FindProperty("logger");
        system = serializedObject.FindProperty("system");
    }

    public override void OnInspectorGUI()
    {

        serializedObject.Update();
        EditorGUILayout.ObjectField(logger, new GUIContent("Logger"));
        EditorGUILayout.ObjectField(system, new GUIContent("System"));
        //EditorGUILayout.ObjectField(pointsList, new GUIContent("Tracked Points"));
        

        GameManager gm = target as GameManager;

        if (GUILayout.Button(new GUIContent("Load Points"))){
            Load(gm);
        }
        if (pointsDict == null) GUI.enabled = false;

        if (GUILayout.Button(new GUIContent("Build Points"))){
            BuildPoints(gm);
        }
        GUI.enabled = true;

        if (GUILayout.Button(new GUIContent("Save Points")))
        {
            Save(gm);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void Load(GameManager gm)
    {
        if (logger.objectReferenceValue != null && system.objectReferenceValue != null)
        {
            gm.Load();
            Logger log = logger.objectReferenceValue as Logger;
            pointsList = log.points;

            pointsDict = new Dictionary<SmallVector3, int>();

            foreach (SmallVector3 point in pointsList.points)
            {
                if (pointsDict.ContainsKey(point)) pointsDict[point]++;
                else pointsDict.Add(point, 1);
            }
        }

    }

    private void BuildPoints(GameManager gm)
    {
        float heightMult = gm.system.incrementVal;
        foreach(KeyValuePair<SmallVector3, int> pair in pointsDict)
        {
            if (pair.Value == 1)
                gm.system.CreateParticleWithHeight(pair.Key.GetVector3());
            else
                gm.system.CreateParticleWithHeight(pair.Key.GetVector3(), pair.Value * heightMult);
        }
    }

    private void Save(GameManager gm)
    {
        gm.Save();
        gm.ClearAll();
    }

}
