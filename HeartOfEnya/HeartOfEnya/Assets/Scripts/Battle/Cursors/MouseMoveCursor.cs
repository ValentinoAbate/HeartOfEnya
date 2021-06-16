using UnityEngine;

/// <summary>
/// Cursor that moves selected units (now with mouse control)
/// </summary>
public class MouseMoveCursor : MoveCursor
{
	public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;      
        if (!traversable.Contains(newPos))
            return;
        var moveRestrictions = BattleUI.main.MoveableTiles;
        if (moveRestrictions.Count > 0 && !moveRestrictions.Contains(newPos))
            return;
            
        sfxHighlight.Play();
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
        partyMember.UpdateChargeTileUI(Pos);
    }

    //highlight whichever square is currently moused over
    private void FollowMouse(Vector3 mousePos)
    {
        if (PauseHandle.Paused)
            return;
    	//convert mouse coords from screenspace to worldspace to BattleGrid coords
    	Pos newPos = BattleGrid.main.GetPos(Camera.main.ScreenToWorldPoint(mousePos));
    	Highlight(newPos);
    	// Debug.Log(Pos);
    }

    //gotta make a wrapper for Select so the input system can see it
    private void HandleSelect()
    {
        if (PauseHandle.Paused)
            return;
        Select();
    }

    private void HandleDeselect()
    {
        if (PauseHandle.Paused || BonusMode || !BattleUI.main.CancelingEnabled)
            return;
        sfxCancel.Play();
    	ResetToLastPosition();
        SetActive(false);
        PhaseManager.main.PartyPhase.CancelAction(partyMember);
    }

    private Vector3 oldMousePos;

    public override void ProcessInput()
	{
        if (Input.mousePosition != oldMousePos)
        {
            FollowMouse(Input.mousePosition);
            oldMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelect();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleDeselect();
        }
    }
}
