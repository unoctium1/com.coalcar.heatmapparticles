using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HeatmapParticleSystem : MonoBehaviour
{
    [SerializeField] HeatmapParticle particlePrefab;
    [SerializeField] float incrementVal = 0.05f;
    [SerializeField] Camera cam;
    [SerializeField] ParticleFactory factory;

    [SerializeField] List<HeatmapParticle> particles;
    [SerializeField] Text debugText;

    Collider[] collisions = new Collider[25];

    private float radius;

    private void Start()
    {
        particles = new List<HeatmapParticle>();
        radius = particlePrefab.transform.localScale.x;
    }

    public void SpawnAtPoint(Vector3 mousePos)
    {
        mousePos.z = cam.nearClipPlane;

        Ray r = cam.ScreenPointToRay(mousePos);

        if(Physics.Raycast(r, out RaycastHit hit))
        {
            StartCoroutine(ProcessHit(hit));
        }
#if UNITY_EDITOR
        if (debugText) debugText.text = particles.Count.ToString();
#endif

    }

    private IEnumerator ProcessHit(RaycastHit hit)
    {
        int hits = Physics.OverlapSphereNonAlloc(hit.point, radius, collisions);
        bool hitParticle = false;
        for (int i = 0; i < hits; i++)
        {
            if (collisions[i].TryGetComponent(out HeatmapParticle particle))
            {
                hitParticle = true;
                particle.Height += incrementVal;
            }
            yield return new WaitForEndOfFrame();
        }

        if (!hitParticle)
        {
            HeatmapParticle particle = factory.Get(this);
            particle.transform.position = hit.point;
            particles.Add(particle);
        }
    }


}
