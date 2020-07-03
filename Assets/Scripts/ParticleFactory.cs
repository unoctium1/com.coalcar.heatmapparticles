using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CreateAssetMenu]
public class ParticleFactory : ScriptableObject
{
    [SerializeField] HeatmapParticle prefab;
    [SerializeField, Tooltip("When pooled objects drop below this, spawn in bulk in a coroutine. Set negative to disable bulk spawning")] int minCount = 50;

    [System.NonSerialized] List<HeatmapParticle> pool;
    private Scene poolScene;
    [System.NonSerialized] bool isSpawningMany = false;

    void CreatePool()
    {
        pool = new List<HeatmapParticle>();
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
        poolScene = SceneManager.CreateScene(name);
    }



    public HeatmapParticle Get(MonoBehaviour owningObject)
    {
        HeatmapParticle instance;
        if(pool == null)
        {
            CreatePool();
        }
        int lastIndex = pool.Count - 1;
        if(lastIndex < minCount && !isSpawningMany)
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
        Debug.Log("Batch spawning, pool count: " + pool.Count);
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
}
