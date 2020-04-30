using System.Collections.Generic;

public class PauseHandle
{
    [System.Flags]
    public enum PauseSource
    {
        None = 0,
        PauseMenu = 1,
        BattleInterrupt = 2,
        All = None | PauseMenu | BattleInterrupt,
    }
    /// <summary>
    /// The sources the pausehandle is currently paused by (if any)
    /// </summary>
    public PauseSource PauseSources { get; private set; }
    /// <summary>
    /// Is the PauseHandle Paused from any source
    /// </summary>
    public bool Paused => PauseSources != PauseSource.None;
    private System.Action<bool> onPause;
    public List<IPausable> Dependents { get; }
    /// <summary>
    /// Consturct a new PauseHandle with the spefied action to call on pause.
    /// Pass null to have nothing called on pause.
    /// </summary>
    public PauseHandle(System.Action<bool> onPause = null)
    {
        Dependents = new List<IPausable>();
        this.onPause = onPause;
    }
    /// <summary>
    /// Consturct a new PauseHandle with the spefied action to call on pause.
    /// Pass null to have nothing called on pause.
    /// Pass IPuasables in to have them be dependents of this PauseHandle
    /// </summary>
    public PauseHandle(System.Action<bool> onPause, params IPausable[] dependents)
    {
        Dependents = new List<IPausable>(dependents);
        this.onPause = onPause;
    }

    private void InternalPause(bool pause)
    {
        onPause?.Invoke(pause);
        foreach (var pauseable in Dependents)
        {
            if (pauseable.PauseHandle == null)
                continue;
            pauseable.PauseHandle.SetPauseAll(pause);
        }
            
    }
    /// <summary>
    /// Pause / Unpauses the handle from specific sources (flags).
    /// Calls the onPause delegate if applicable.
    /// </summary>
    public void SetPause(bool pause, PauseSource sources)
    {
        if (pause)
            Pause(sources);
        else
            Unpause(sources);
    }
    /// <summary>
    /// Pause the handle from specific sources (flags).
    /// Calls the onPause delegate with true if previously unpaused.
    /// </summary>
    public void Pause(PauseSource sources)
    {
        bool wasPaused = Paused;
        PauseSources |= sources;
        if (!wasPaused && Paused)
            InternalPause(true);
    }
    /// <summary>
    /// Unpause the handle from specific sources (flags).
    /// Calls the onPause delegate with false if previously paused.
    /// </summary>
    public void Unpause(PauseSource sources)
    {
        bool wasPaused = Paused;
        PauseSources &= (~sources);
        if(wasPaused && !Paused)
            InternalPause(false);
    }
    /// <summary>
    /// Pause / Unpauses the handle from all sources.
    /// Calls the onPause delegate if applicable.
    /// </summary>
    public void SetPauseAll(bool pause)
    {
        if (pause)
            PauseAll();
        else
            UnpauseAll();
    }
    /// <summary>
    /// Pause the handle from all sources.
    /// Calls the onPause delegate with true if previously unpaused.
    /// </summary>
    public void PauseAll()
    {
        if (!Paused)
            InternalPause(true);
        PauseSources = PauseSource.All;
    }
    /// <summary>
    /// UnPause the handle from all sources.
    /// Calls the onPause delegate with false if previously paused.
    /// </summary>
    public void UnpauseAll()
    {
        if(Paused)
            InternalPause(false);
        PauseSources = PauseSource.None;
    }
}
