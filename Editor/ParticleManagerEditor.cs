using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using HeatmapParticles.Utility;

namespace HeatmapParticles
{
    [CustomEditor(typeof(ParticleManager)), CanEditMultipleObjects]
    public class ParticleManagerEditor : Editor
    {

        private Dictionary<SmallVector3, int> pointsDict;

        private SerializedProperty logger;
        private SerializedProperty system;
        private SerializedProperty size;
        private SerializedProperty particlePrefab;
        private PointsList pointsList;

        public void OnEnable()
        {
            logger = serializedObject.FindProperty("logger");
            system = serializedObject.FindProperty("system");
            size = serializedObject.FindProperty("particleSize");
            particlePrefab = serializedObject.FindProperty("particlePrefab");
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            EditorGUILayout.PropertyField(logger, new GUIContent("Logger"));
            EditorGUILayout.PropertyField(system, new GUIContent("System"));

            //EditorGUILayout.ObjectField(pointsList, new GUIContent("Tracked Points"));


            ParticleManager gm = target as ParticleManager;

            if (GUILayout.Button(new GUIContent("Load Points")))
            {
                Load(gm);
            }
            if (pointsDict == null) GUI.enabled = false;

            if (GUILayout.Button(new GUIContent("Build Points")))
            {
                BuildPoints(gm);
            }
            GUI.enabled = true;

            if (GUILayout.Button(new GUIContent("Save Points")))
            {
                Save(gm);
            }

            EditorGUILayout.PropertyField(size, new GUIContent("Particle Size"));
            EditorGUILayout.PropertyField(particlePrefab, new GUIContent("Particle Prefab"));
            serializedObject.ApplyModifiedProperties();
            if (particlePrefab.objectReferenceValue == null) GUI.enabled = false;
            if (GUILayout.Button(new GUIContent("Apply Particle Size")))
            {
                ApplyParticleSize(size.floatValue);
            }
            GUI.enabled = true;

        }

        private void ApplyParticleSize(float size)
        {
            GameObject particle = particlePrefab.objectReferenceValue as GameObject;
            particle.transform.localScale = Vector3.one * size;
            PrefabUtility.SavePrefabAsset(particle);
        }

        private void Load(ParticleManager gm)
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

        private void BuildPoints(ParticleManager gm)
        {
            float heightMult = gm.system.incrementVal;
            foreach (KeyValuePair<SmallVector3, int> pair in pointsDict)
            {
                if (pair.Value == 1)
                    gm.system.CreateParticleWithHeight(pair.Key.GetVector3());
                else
                    gm.system.CreateParticleWithHeight(pair.Key.GetVector3(), pair.Value * heightMult);
            }
        }

        private void Save(ParticleManager gm)
        {
            gm.Save();
            gm.ClearAll();
        }

    }
}
