using HeatmapParticles.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HeatmapParticles
{
    public class HeatmapParticleSystem : MonoBehaviour
    {
        [SerializeField] HeatmapParticle particlePrefab;
        [SerializeField] public float incrementVal = 0.05f;
        [SerializeField] ParticleFactory factory;

        [SerializeField] Dictionary<SmallVector3, HeatmapParticle> particles;
        [SerializeField] Text debugText;
        [SerializeField] Vector3 currPoint;
        PointsList points;

        //Collider[] collisions = new Collider[25];

        private float radius;
        //private int layerMask;

        private void Start()
        {
            particles = new Dictionary<SmallVector3, HeatmapParticle>();
            radius = ParticleManager.Instance.particleSize / 2;
            points = PointsList.Instance;
            //layerMask = 1 << Physics.IgnoreRaycastLayer;
            //layerMask = ~layerMask;
        }

        public void BulkSpawn(int toSpawn)
        {
            StartCoroutine(factory.SpawnMany(toSpawn));
        }

        
        public void SpawnAtPoint(SmallVector3 point)
        {
            int height = points.CurrDict[point].height;
            if (particles.TryGetValue(point, out HeatmapParticle p))
            {
                p.Height = height * incrementVal;
            }
            else
            {
                if (height == 1)
                    CreateParticleWithHeight(point);
                else
                    CreateParticleWithHeight(point, height * incrementVal);
            }
            
            //StartCoroutine(ProcessPoint(point));
#if UNITY_EDITOR
            if (debugText) debugText.text = particles.Count.ToString();
#endif

        }
        

        public void CreateFromDictionary(Dictionary<SmallVector3, HeatmapInfo> dict)
        {
            Clear();
            foreach (KeyValuePair<SmallVector3, HeatmapInfo> pair in dict)
            {
                if (pair.Value.height == 1)
                    CreateParticleWithHeight(pair.Key);
                else
                    CreateParticleWithHeight(pair.Key, pair.Value.height * incrementVal);
            }
        }

        /* Temporarily removing realtime capability
        private void ProcessPoint(Vector3 point)
        {
            currPoint = point;
            int hits = Physics.OverlapSphereNonAlloc(point, radius, collisions, layerMask);
            bool hitParticle = false;
            for (int i = 0; i < hits; i++)
            {
                if (collisions[i].TryGetComponent(out HeatmapParticle particle))
                {
                    hitParticle = true;
                    particle.Height += incrementVal;
                }
                //yield return new WaitForEndOfFrame();
            }

            if (!hitParticle)
            {
                HeatmapParticle particle = factory.Get(this);
                particle.transform.position = point;
                particles.Add(particle);
            }
        }
        */



        public void CreateParticleWithHeight(SmallVector3 point, float height = 0.01f)
        {
            if (particles == null) particles = new Dictionary<SmallVector3, HeatmapParticle>();
            HeatmapParticle p;
            if (particles.TryGetValue(point, out  p))
            {
                Debug.Log("particle already exists!");
            }
            else
            {
                p =
#if UNITY_EDITOR
                Application.isPlaying ? factory.Get(this) : factory.GetInEditor();
#else
                factory.Get(this);
#endif
                p.transform.position = point.GetVector3();
                p.Height = height;
                particles.Add(point, p);
            }
        }


        private void OnDrawGizmosSelected()
        {
            if (currPoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(currPoint, radius);
            }
        }

        public void HideParticles()
        {
            foreach (HeatmapParticle p in particles.Values)
            {
                p.gameObject.SetActive(false);
            }
        }

        public void UnhideParticles()
        {
            foreach (HeatmapParticle p in particles.Values)
            {
                p.gameObject.SetActive(true);
            }
        }


        public void Clear()
        {
            foreach (HeatmapParticle p in particles.Values)
            {
                factory.Reclaim(p);
            }
            particles.Clear();
        }

    }
}
