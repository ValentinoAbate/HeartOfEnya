using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A top-level class for objects that take player input and control battle flow.
/// Different cursors have different movement logic, diaply and functionality and are responsiple for different ares of battle.
/// </summary>
public abstract class Cursor : MonoBehaviour, IPausable
{
    public PauseHandle PauseHandle { get; set; } = new PauseHandle();
    public Pos Pos { get; set; }

    public virtual void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    /// <summary>
    /// Move the cursor to a new square (if the new square is valid, and modify any UI necessary.
    /// </summary>
    /// <param name="pos"> The position to move to </param>
    public abstract void Highlight(Pos pos);

    /// <summary>
    /// Apply the cursor's functionality at the current square.
    /// Should almost always be triggered by the standard confirm butting in ProcessInput()
    /// By default, uses the selction method defined in the highlighted field object if one exists.
    /// </summary>
    public virtual void Select()
    {
        var highlighted = BattleGrid.main.GetObject(Pos);
        if (highlighted != null)
        {
            if (highlighted.Select())
                gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Process any input. Called in update if the cursor is unpaused.
    /// </summary>
    public abstract void ProcessInput();
    // Update is called once per frame
    private void Update()
    {
        if (PauseHandle.Paused)
            return;
        ProcessInput();
    }
}
