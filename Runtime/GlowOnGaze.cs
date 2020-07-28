using HeatmapParticles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeatmapParticles
{
    public class GlowOnGaze : MonoBehaviour
    {
        [SerializeField] private InputSource input;
        [SerializeField] Camera cam;
        [SerializeField] private bool m_glowIsActive = false;
        delegate bool GetInputPoint(Camera cam, out Vector3 point, int layerMask);
        private GetInputPoint getter;
        private int layerMask;
        private ObjectHighlighter previousObjectGazed;


        private void Awake()
        {
            switch (input)
            {
                case InputSource.mousePos:
                    getter = GetMousePos;
                    break;
                case InputSource.vrCameraGaze:
                    getter = GetGazePos;
                    break;
            }
            layerMask = 1 << Physics.IgnoreRaycastLayer;
            layerMask = ~layerMask;
        }

        private void FixedUpdate()
        {
            if (m_glowIsActive)
                getter(cam, out Vector3 point, layerMask);
        }

        private bool GetMousePos(Camera cam, out Vector3 point, int layerMask)
        {
            point = Input.mousePosition;
            point.z = cam.nearClipPlane;
            Ray r = cam.ScreenPointToRay(point);

            if (Physics.Raycast(r, out RaycastHit hit, 50f, layerMask))
            {
                ObjectHit(hit, out point);
                return true;
            }
            else
            {
                ObjectMiss();
                return false;
            }
        }

        // Replace this with eye tracking 
        private bool GetGazePos(Camera cam, out Vector3 point, int layerMask)
        {
            point = new Vector3(0.5f, 0.5f, 0f);

            // Haven't really tested Mono vs left here
            Ray r = cam.ViewportPointToRay(point, Camera.MonoOrStereoscopicEye.Mono);

            if (Physics.Raycast(r, out RaycastHit hit, 50f, layerMask))
            {
                ObjectHit(hit, out point);
                return true;
            }
            else
            {
                ObjectMiss();
                return false;
            }
        }


        private void ObjectHit(RaycastHit hit, out Vector3 point)
        {
            ObjectHighlighter currObject = hit.transform.GetComponent<ObjectHighlighter>();
            if (currObject == null)
            {
                if (previousObjectGazed != null)
                    previousObjectGazed.DeactivateHighlight();
            }
            else
            {
                if (currObject != previousObjectGazed)
                {
                    if (previousObjectGazed != null)
                        previousObjectGazed.DeactivateHighlight();
                }
            }

            previousObjectGazed = currObject;
            if (previousObjectGazed != null) previousObjectGazed.ActivateHighlight();
            point = hit.point;
        }

        private void ObjectMiss()
        {
            if (previousObjectGazed != null) previousObjectGazed.DeactivateHighlight();
            previousObjectGazed = null;
        }
    }
}
