using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// <summary>
    /// Consturct a new PauseHandle with the spefied action to call on pause.
    /// Pass null to have nothing called on pause.
    /// </summary>
    public PauseHandle(System.Action<bool> onPause = null)
    {
        this.onPause = onPause;
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
            onPause?.Invoke(true);
    }
    /// <summary>
    /// UnPause the handle from specific sources (flags).
    /// Calls the onPause delegate with false if previously paused.
    /// </summary>
    public void UnPause(PauseSource sources)
    {
        bool wasPaused = Paused;
        PauseSources &= (~sources);
        if(wasPaused && !Paused)
            onPause?.Invoke(false);
    }
    /// <summary>
    /// Pause the handle from all sources.
    /// Calls the onPause delegate with true if previously unpaused.
    /// </summary>
    public void PauseAll()
    {
        if (!Paused)
            onPause?.Invoke(true);
        PauseSources = PauseSource.All;
    }
    /// <summary>
    /// UnPause the handle from all sources.
    /// Calls the onPause delegate with false if previously paused.
    /// </summary>
    public void UnPauseAll()
    {
        if(Paused)
            onPause?.Invoke(false);
        PauseSources = PauseSource.None;
    }
}
