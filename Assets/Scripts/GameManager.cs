using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistableObject
{

    Saver saver;
    [SerializeField] Logger logger;
    [SerializeField] HeatmapParticleSystem system;

    // Start is called before the first frame update
    void Start()
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
        system.Save(writer);
    }

    public override void Load(DataReader reader)
    {
        logger.Load(reader);
        system.Load(reader);
    }
}
