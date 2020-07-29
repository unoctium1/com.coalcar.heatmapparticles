using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using HeatmapParticles.Utility;
using UnityEngine.UIElements;

namespace HeatmapParticles
{
    [CustomEditor(typeof(ParticleManager)), CanEditMultipleObjects]
    public class ParticleManagerEditor : Editor
    {

        private SerializedProperty logger;
        private SerializedProperty system;
        private SerializedProperty size;
        private SerializedProperty particlePrefab;
        private PointsList pointsList;
        private bool foldout;

        public void OnEnable()
        {
            logger = serializedObject.FindProperty("logger");
            system = serializedObject.FindProperty("system");
            size = serializedObject.FindProperty("particleSize");
            particlePrefab = serializedObject.FindProperty("particlePrefab");
            foldout = false;
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
            if (pointsList == null) GUI.enabled = false;

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
            
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, new GUIContent("Currently Tracked Points"));
            if (pointsList)
            {
                EditorGUILayout.LabelField(pointsList.CountCurrent.ToString() + " points tracked");
            }
            else
            {
                if(GUILayout.Button(new GUIContent("Fetch Points"))){
                    FetchPoints();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

        }

        private void FetchPoints()
        {
            pointsList = PointsList.Instance;
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
                FetchPoints();
            }

        }

        private void BuildPoints(ParticleManager gm)
        {
            gm.system.CreateFromDictionary(pointsList.CurrDict);
        }

        private void Save(ParticleManager gm)
        {
            gm.Save();
            gm.ClearAll();
        }

    }
}
