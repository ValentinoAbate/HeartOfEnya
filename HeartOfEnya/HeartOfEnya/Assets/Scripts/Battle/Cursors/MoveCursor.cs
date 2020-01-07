using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A cursor class responisible for displaying and enacting movement for party members.
/// Enabled by the main party phase curor
/// When a square is selected, passes control to the party member's action menu.
/// </summary>
[RequireComponent(typeof(PartyMember))]
public class MoveCursor : GridCursor
{
    private Pos lastPosition;
    private PartyMember partyMember;
    private List<Pos> traversable;
    private readonly List<GameObject> squares = new List<GameObject>();

    private void Start()
    {
        partyMember = GetComponent<PartyMember>();
        Pos = partyMember.Pos;
        lastPosition = Pos;
        enabled = false;
    }

    public override void SetActive(bool value)
    {
        enabled = value;
        if(value)
        {
            CalculateTraversable();
        }
        DisplayTraversable(value);
    }

    /// <summary>
    /// Calculates which squares can be moved to by the cursor, based on the BattleGrid's reachability algorithm
    /// </summary>
    public void CalculateTraversable()
    {
        // Log the last position in case the move is canceled
        lastPosition = partyMember.Pos;
        // Calculate the reachable squares given position, move range, and traverability function
        traversable = BattleGrid.main.Reachable(partyMember.Pos, partyMember.Move, partyMember.CanMoveThrough).Keys.ToList();
        // Remove squares that were traversable but cannot be ended in (Squares with trasversable allies, etc.)
        traversable.RemoveAll((p) => !BattleGrid.main.IsEmpty(p));
        // Add the initial location back in (as it will be removed by the last line)
        traversable.Add(partyMember.Pos);
        // Make sure the cursor's position is correct
        Pos = partyMember.Pos;
    }

    /// <summary>
    /// Display or hide the moveable range UI depending on the "value" argument
    /// </summary>
    public void DisplayTraversable(bool value)
    {
        if(value)
        {
            foreach (var spot in traversable)
                squares.Add(BattleGrid.main.SpawnSquare(spot, BattleGrid.main.moveSquareMat));
        }
        else
        {
            foreach (var obj in squares)
                Destroy(obj);
            squares.Clear();
        }
    }

    public override void Highlight(Pos newPos)
    {
        if (newPos == Pos)
            return;
        // Code to allow movement cursor to teleport accross obstacles if there is a reachable space on the other side
        // Essentially checks if the next square in the movement direction is reachable, and recurring
        // Recursion ends if a traversable square is found or the difference between the new position and the start is
        // Know to be greater than the movement range
        if (!traversable.Contains(newPos))
        {
            Pos difference = newPos - Pos;
            // Return if different is breater than the movement range
            // Square values used as an optimization to avoid forcing a square root evaluation
            if (difference.SquareMagnitude > partyMember.Move * partyMember.Move)
                return;
            if (difference.col > 0)
                ++difference.col;
            else if (difference.col < 0)
                --difference.col;
            else if (difference.row > 0)
                ++difference.row;
            else if (difference.row < 0)
                --difference.row;
            Highlight(Pos + difference);
            return;
        }
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
    }

    // Finish the move and open the action menu
    public override void Select()
    {
        lastPosition = partyMember.Pos;
        BattleGrid.main.Move(partyMember, Pos);
        SetActive(false);
        partyMember.OpenActionMenu();
    }

    /// <summary>
    /// Reset to the last postion. Used when Cancelling moves. or cancelling the action menu.
    /// </summary>
    public void ResetToLastPosition()
    {
        Highlight(lastPosition);
        BattleGrid.main.Move(partyMember, Pos);
    }

    /// <summary>
    /// Base Grid Cursor functionality (see GridCursor.cs) with support for cancelling moves
    /// </summary>
    public override void ProcessInput()
    {
        base.ProcessInput();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetToLastPosition();
            SetActive(false);
            PhaseManager.main.PartyPhase.CancelAction(partyMember);
        }

    }
}
