using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HeatmapParticles
{

    [CreateAssetMenu]
    public class ParticleFactory : ScriptableObject
    {
        [SerializeField] HeatmapParticle prefab;
        [SerializeField, Tooltip("When pooled objects drop below this, spawn in bulk in a coroutine. Set negative to disable bulk spawning")] int minCount = 50;

        [System.NonSerialized] List<HeatmapParticle> pool;
        public Scene poolScene;
        [System.NonSerialized] bool isSpawningMany = false;

        void CreatePool()
        {
            pool = new List<HeatmapParticle>();
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                poolScene = SceneManager.GetSceneByName(name);
                if (poolScene.isLoaded)
                {
                    GameObject[] rootObjects = poolScene.GetRootGameObjects();
                    for (int i = 0; i < rootObjects.Length; i++)
                    {
                        HeatmapParticle pooledShape = rootObjects[i].GetComponent<HeatmapParticle>();
                        if (!pooledShape.gameObject.activeSelf)
                        {
                            pool.Add(pooledShape);
                        }
                    }
                    return;
                }
            }
#endif
            poolScene = SceneManager.CreateScene(name);
        }

#if UNITY_EDITOR
        public HeatmapParticle GetInEditor()
        {
            HeatmapParticle instance;
            if (pool == null || !poolScene.IsValid())
            {
                CreatePoolInEditor();
            }
            instance = Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(
                    instance.gameObject, poolScene
                );
            return instance;
        }

        private void CreatePoolInEditor()
        {
            pool = new List<HeatmapParticle>();
            if (!Application.isEditor)
            {
                poolScene = SceneManager.GetSceneByName(name);
                if (poolScene.isLoaded)
                {
                    GameObject[] rootObjects = poolScene.GetRootGameObjects();
                    for (int i = 0; i < rootObjects.Length; i++)
                    {
                        HeatmapParticle pooledShape = rootObjects[i].GetComponent<HeatmapParticle>();
                        if (!pooledShape.gameObject.activeSelf)
                        {
                            pool.Add(pooledShape);
                        }
                    }
                    return;
                }
            }
            poolScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            poolScene.name = name;
        }
#endif
        public HeatmapParticle Get(MonoBehaviour owningObject)
        {
            HeatmapParticle instance;
            if (pool == null)
            {
                CreatePool();
            }
            int lastIndex = pool.Count - 1;
            if (minCount > -1 && lastIndex < minCount && !isSpawningMany)
            {
                isSpawningMany = true;
                owningObject.StartCoroutine(SpawnMany());
            }
            if (lastIndex >= 0)
            {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
            else
            {
                instance = Instantiate(prefab);
                SceneManager.MoveGameObjectToScene(
                        instance.gameObject, poolScene
                    );
            }
            return instance;
        }

        public void Reclaim(HeatmapParticle particle)
        {
            if (pool == null)
            {
                CreatePool();
            }

            pool.Add(particle);
            particle.Height = 0.01f;
            particle.gameObject.SetActive(false);
        }

        private IEnumerator SpawnMany()
        {
            //Debug.Log("Batch spawning, pool count: " + pool.Count);
            if (pool == null)
            {
                CreatePool();
            }
            for (int i = 0; i < minCount * 2; i++)
            {
                HeatmapParticle instance;
                instance = Instantiate(prefab);
                SceneManager.MoveGameObjectToScene(
                        instance.gameObject, poolScene
                    );
                Reclaim(instance);
                if (i % 3 == 0) yield return new WaitForEndOfFrame();
            }
            isSpawningMany = false;
        }

        public IEnumerator SpawnMany(int targetPoolSize)
        {
            //Debug.Log("Batch spawning, pool count: " + pool.Count);
            if (pool == null)
            {
                CreatePool();
            }
            int toSpawn = targetPoolSize - pool.Count + minCount;
            for (int i = 0; i < toSpawn; i++)
            {
                HeatmapParticle instance;
                instance = Instantiate(prefab);
                SceneManager.MoveGameObjectToScene(
                        instance.gameObject, poolScene
                    );
                Reclaim(instance);
                if (i % 3 == 0) yield return new WaitForEndOfFrame();
            }
            isSpawningMany = false;
        }
    }
}
