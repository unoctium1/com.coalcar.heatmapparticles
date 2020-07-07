using Sample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatmapParticleSystem : PersistableObject
{
    [SerializeField] HeatmapParticle particlePrefab;
    [SerializeField] float incrementVal = 0.05f;
    [SerializeField] ParticleFactory factory;

    [SerializeField] List<HeatmapParticle> particles;
    [SerializeField] Text debugText;
    [SerializeField] Vector3 currPoint;

    Collider[] collisions = new Collider[25];

    private float radius;

    private void Start()
    {
        particles = new List<HeatmapParticle>();
        radius = particlePrefab.transform.localScale.x/2;
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
        int hits = Physics.OverlapSphereNonAlloc(point, radius, collisions);
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
        BulkSpawn(count);
        for(int i = 0; i < count; i++)
        {
            HeatmapParticle p = factory.Get(this);
            p.Load(reader);
            particles.Add(p);
        }
    }

    public override void Save(DataWriter writer)
    {
        writer.Write(particles.Count);
        for(int i = 0; i < particles.Count; i++)
        {
            particles[i].Save(writer);
        }
    }

    public void Clear()
    {
        foreach(HeatmapParticle p in particles)
        {
            factory.Reclaim(p);
        }
        particles.Clear();
    }

}
