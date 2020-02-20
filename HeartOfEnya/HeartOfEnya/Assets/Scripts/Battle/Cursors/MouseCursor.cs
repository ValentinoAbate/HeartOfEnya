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

    // Start is called before the first frame update
    void Start()
    {
        //Highlight(Pos);
    }

    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!BattleGrid.main.IsLegal(newPos))
            return;

        BattleGrid.main.GetObject(Pos)?.UnHighlight();
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
        BattleGrid.main.GetObject(Pos)?.Highlight();
    }

    //highlight whichever square is currently moused over
    private void FollowMouse(InputAction.CallbackContext context)
    {
    	//convert mouse coords from screenspace to worldspace to BattleGrid coords
    	Pos newPos = BattleGrid.main.GetPos(Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()));
    	Highlight(newPos);
    	// Debug.Log(Pos);
    }

    //gotta make a wrapper for Select so the input system can see it
    private void HandleSelect(InputAction.CallbackContext context)
    {
        Select();
    }


    // Now handled by the new input system
    public override void ProcessInput()
    {

    }
}
