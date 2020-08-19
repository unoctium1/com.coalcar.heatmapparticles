using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

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
        private bool configureParticle;
        private bool builtPoints;

        private Scene poolScene;

        public void OnEnable()
        {
            logger = serializedObject.FindProperty("logger");
            system = serializedObject.FindProperty("system");
            size = serializedObject.FindProperty("particleSize");
            particlePrefab = serializedObject.FindProperty("particlePrefab");
            foldout = false;
            configureParticle = false;
            builtPoints = false;
            EditorApplication.playModeStateChanged += ClearParticleFactoryScene;
        }

        private void ClearParticleFactoryScene(PlayModeStateChange obj)
        {
            if (poolScene.IsValid())
                {
                    EditorSceneManager.UnloadSceneAsync(poolScene);
                }
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

            if (!builtPoints) GUI.enabled = false;
            if (GUILayout.Button(new GUIContent("Remove Points")))
            {
                RemovePoints(gm);
            }
            GUI.enabled = true;


            if (GUILayout.Button(new GUIContent("Save Points")))
            {
                Save(gm);
            }

            configureParticle = EditorGUILayout.BeginFoldoutHeaderGroup(configureParticle, new GUIContent("Particle Configuration Settings"));
            if (configureParticle)
            {
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
            EditorGUILayout.EndFoldoutHeaderGroup();

            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, new GUIContent("Currently Tracked Points"));
            if (foldout)
            {
                if (pointsList != null)
                {
                    EditorGUILayout.LabelField(pointsList.GetInfo());
                    EditorGUILayout.LabelField(pointsList.GetDateTime());
                    EditorGUILayout.LabelField(pointsList.CountCurrent.ToString() + " points tracked");

                    if (GUILayout.Button("Clear/Reinitialize"))
                    {
                        pointsList.InitializeCurrElement();
                    }

                    if (GUILayout.Button("Reset Time"))
                    {
                        pointsList.ResetTime();
                    }

                    if (GUILayout.Button("Add new/Move up"))
                    {
                        pointsList.MoveUp();
                    }

                    if (GUILayout.Button("Move down"))
                    {
                        pointsList.MoveDown();
                    }

                }
                else
                {
                    EditorGUILayout.LabelField(new GUIContent("Hit 'Load Points' first"));
                }
            }

            
            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();
        }

        private void RemovePoints(ParticleManager gm)
        {
            if (poolScene.IsValid())
            {
                gm.system.Clear();
                EditorSceneManager.UnloadSceneAsync(poolScene);
                
                builtPoints = false;
            }
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
            poolScene = gm.system.factory.poolScene;
            builtPoints = true;
        }

        private void Save(ParticleManager gm)
        {
            gm.Save();
            gm.ClearAll();
        }

    }
}
