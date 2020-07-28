using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeatmapParticles
{
    public class ObjectHighlighter : MonoBehaviour
    {
        [SerializeField] private Material m_material;
        private bool m_isGlowing = false;

        void OnEnable()
        {
            m_material.color = Color.black;
        }

        public void ToggleHighlite(bool val)
        {
            if (val)
            {
                ActivateHighlight();
            }
            else
            {
                DeactivateHighlight();
            }
        }
        
        public void ActivateHighlight()
        {
            if (!m_isGlowing)
                m_material.color = Color.blue;
        }

        public void DeactivateHighlight()
        {
            m_material.color = Color.black;
        }
    }
}

