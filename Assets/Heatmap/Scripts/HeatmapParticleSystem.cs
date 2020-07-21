using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HeatmapParticles
{
    public class HeatmapParticleSystem : PersistableObject
    {
        [SerializeField] HeatmapParticle particlePrefab;
        [SerializeField] public float incrementVal = 0.05f;
        [SerializeField] ParticleFactory factory;

        [SerializeField] List<HeatmapParticle> particles;
        [SerializeField] Text debugText;
        [SerializeField] Vector3 currPoint;

        Collider[] collisions = new Collider[25];

        private float radius;
        private int layerMask;

        private void Start()
        {
            particles = new List<HeatmapParticle>();
            radius = GameManager.Instance.particleSize / 2;
            layerMask = 1 << Physics.IgnoreRaycastLayer;
            layerMask = ~layerMask;
        }

        public void BulkSpawn(int toSpawn)
        {
            StartCoroutine(factory.SpawnMany(toSpawn));
        }

        public void SpawnAtPoint(Vector3 point)
        {

            ProcessPoint(point);
            //StartCoroutine(ProcessPoint(point));
#if UNITY_EDITOR
            if (debugText) debugText.text = particles.Count.ToString();
#endif

        }

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


#if UNITY_EDITOR
        public void CreateParticleWithHeight(Vector3 point, float height = 0.01f)
        {
            if (particles == null) particles = new List<HeatmapParticle>();
            HeatmapParticle p = factory.GetInEditor();
            p.transform.position = point;
            p.Height = height;
            particles.Add(p);
        }
#endif

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
            foreach (HeatmapParticle p in particles)
            {
                p.gameObject.SetActive(false);
            }
        }

        public void UnhideParticles()
        {
            foreach (HeatmapParticle p in particles)
            {
                p.gameObject.SetActive(true);
            }
        }

        public override void Load(DataReader reader)
        {
            int count = reader.ReadInt();

            
            particles = new List<HeatmapParticle>();
            if (Application.isPlaying)
                BulkSpawn(count);
            for (int i = 0; i < count; i++)
            {
                HeatmapParticle p = Application.isPlaying ? factory.Get(this) : factory.GetInEditor();
                p.Load(reader);
                particles.Add(p);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(this);
#endif

        }

        public override void Save(DataWriter writer)
        {
            writer.Write(particles.Count);
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Save(writer);
            }
        }

        public void Clear()
        {
            foreach (HeatmapParticle p in particles)
            {
                factory.Reclaim(p);
            }
            particles.Clear();
        }

    }
}
