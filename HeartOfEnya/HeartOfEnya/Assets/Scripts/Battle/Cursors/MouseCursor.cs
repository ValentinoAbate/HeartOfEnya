using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

/// <summary>
/// A cursor that can be moved around the grid by the mouse.
/// </summary>
public class MouseCursor : Cursor
{
	//get a Controls object to access the input system
	public Controls controlSys;

	//set up controlSys
	void Awake()
	{
		controlSys = new Controls();
	}

	//set up & enable the actions we'll be using
	private void OnEnable()
	{
		controlSys.BattleUI.MousePos.performed += FollowMouse;
		controlSys.BattleUI.MousePos.Enable();
		controlSys.BattleUI.Select.performed += HandleSelect;
        controlSys.BattleUI.Select.Enable();
    }

	//clean up the actions once we're done (to avoid memory leaks)
	private void OnDisable()
	{
		controlSys.BattleUI.MousePos.performed -= FollowMouse;
		controlSys.BattleUI.MousePos.Disable();
		controlSys.BattleUI.Select.performed -= HandleSelect;
        controlSys.BattleUI.Select.Disable();
    }

    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!BattleGrid.main.IsLegal(newPos))
            return;

        sfxHighlight.Play();
        BattleGrid.main.Get<FieldObject>(Pos)?.UnHighlight();
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
        FieldObject newObject = BattleGrid.main.Get<FieldObject>(Pos);
        newObject?.Highlight();

        // check if the object is an enemy
        if(newObject != null && newObject.Team == FieldEntity.Teams.Enemy)
        {
            // invoke tutorial event explain enemy stats
            BattleEvents.main.tutEnemyInfo._event.Invoke();
        }

    }

    //highlight whichever square is currently moused over
    private void FollowMouse(InputAction.CallbackContext context)
    {
        if (PauseHandle.Paused)
            return;
        //convert mouse coords from screenspace to worldspace to BattleGrid coords
        Pos newPos = BattleGrid.main.GetPos(Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()));
    	Highlight(newPos);
    	// Debug.Log(Pos);
    }

    //gotta make a wrapper for Select so the input system can see it
    private void HandleSelect(InputAction.CallbackContext context)
    {
        if (PauseHandle.Paused || !BattleGrid.main.ContainsPoint(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())))
            return;
        Select();          
    }


    // Now handled by the new input system
    public override void ProcessInput()
    {

    }
}
