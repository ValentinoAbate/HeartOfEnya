using UnityEngine;

/// <summary>
/// A cursor that can be moved around the grid by the mouse.
/// </summary>
public class MouseCursor : Cursor
{
    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!BattleGrid.main.IsLegal(newPos))
        {
            BattleGrid.main.Get<FieldObject>(Pos)?.UnHighlight();
            Pos = Pos.OutOfBounds;
            return;
        }

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
    private void HandleSelect(Vector3 mousePos)
    {
        if (PauseHandle.Paused || !BattleGrid.main.ContainsPoint(Camera.main.ScreenToWorldPoint(mousePos)))
            return;
        Select();          
    }

    private Vector3 oldMousePos;
    // Now handled by the new input system
    public override void ProcessInput()
    {
        if (Input.mousePosition != oldMousePos)
        {
            FollowMouse(Input.mousePosition);
            oldMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelect(Input.mousePosition);
        }
    }
}
