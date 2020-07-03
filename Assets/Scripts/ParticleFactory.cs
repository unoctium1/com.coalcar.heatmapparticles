using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class ParticleFactory : ScriptableObject
{
    [SerializeField] HeatmapParticle prefab;

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

    public HeatmapParticle Get(HeatmapParticleSystem owningObject)
    {
        HeatmapParticle instance;
        if(pool == null)
        {
            CreatePool();
        }
        int lastIndex = pool.Count - 1;
        if(lastIndex < 50 && !isSpawningMany)
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
        if (pool == null)
        {
            CreatePool();
        }
        for (int i = 0; i < 50; i++)
        {
            HeatmapParticle instance;
            instance = Instantiate(prefab);
            SceneManager.MoveGameObjectToScene(
                    instance.gameObject, poolScene
                );
            instance.gameObject.SetActive(false);
            if (i % 3 == 0) yield return new WaitForEndOfFrame();
        }
        isSpawningMany = false;
    }
}
