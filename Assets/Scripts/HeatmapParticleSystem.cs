using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HeatmapParticleSystem : MonoBehaviour
{
    [SerializeField] HeatmapParticle particlePrefab;
    [SerializeField] float incrementVal = 0.05f;
    [SerializeField] Camera cam;
    [SerializeField] ParticleFactory factory;

    Collider[] collisions = new Collider[25];

    private float radius;

    private void Start()
    {
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
            factory.Get(this).transform.position = hit.point;
        }
    }


}
