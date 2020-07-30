using HeatmapParticles.Utility;
using UnityEngine;

namespace HeatmapParticles
{
    [ExecuteInEditMode]
    public class ParticleManager : PersistableObject
    {
        public static ParticleManager Instance
        {
            get
            {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<ParticleManager>();
                return _instance;
            }
        }
        private static ParticleManager _instance = null;
        Saver saver;
        [SerializeField] public Logger logger;
        [SerializeField] public HeatmapParticleSystem system;

        private PointsList points;

#if UNITY_EDITOR
        [SerializeField] public GameObject particlePrefab;
#endif

        public float particleSize;


        void Awake()
        {
            saver = GetComponent<Saver>();
        }

        public void ClearAll()
        {
            system.Clear();
            logger.Clear();
        }

        public void Save()
        {
            saver.Save(this);
        }

        public void Load()
        {
            if(Application.isPlaying)
                system.Clear();
            saver.Load(this);
            
        }

        public void HideActiveParticles()
        {
            system.HideParticles();
        }

        public void UnhideActiveParticles()
        {
            system.UnhideParticles();
        }

        public void StartLogging_Realtime()
        {
            logger.Realtime = true;
            system.UnhideParticles();
            logger.Log = true;
        }
        

        public void StartLogging_NonRealtime()
        {
            logger.Realtime = false;
            system.HideParticles();
            logger.Log = true;
        }

        public void PlaybackParticles()
        {
            logger.Log = false;
            system.UnhideParticles();
            logger.Playback();
        }

        public void StopRecording()
        {
            logger.Log = false;
        }

        public override void Save(DataWriter writer)
        {
            logger.Save(writer);
        }

        public override void Load(DataReader reader)
        {
            logger.Load(reader);
        }
    }
}
