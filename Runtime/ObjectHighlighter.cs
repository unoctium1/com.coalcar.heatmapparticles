using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeatmapParticles
{
    public class ObjectHighlighter : MonoBehaviour
    {
        [SerializeField] private Material m_material;
        [SerializeField] private Outline m_outline;

        private bool m_isGlowing = false;

        void OnEnable()
        {
            m_outline.enabled = false;
        }

        public void ToggleHighlite(bool val)
        {
            if (val)
            {
                //ActivateHighlight();
            }
            else
            {
                //DeactivateHighlight();
            }
        }
        
        public void ActivateHighlight()
        {
            if (!m_isGlowing)
                m_outline.enabled = true;
        }

        public void DeactivateHighlight()
        {
            m_outline.enabled = false;
        }
    }
}

