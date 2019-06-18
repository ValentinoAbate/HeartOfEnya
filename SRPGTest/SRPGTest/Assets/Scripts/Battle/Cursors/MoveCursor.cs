using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PartyMember))]
public class MoveCursor : GridCursor
{
    public GameObject squarePrefab;
    private Pos lastPosition;
    private PartyMember partyMember;
    private HashSet<Pos> traversible;
    private List<GameObject> squares = new List<GameObject>();
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
            lastPosition = Pos;
            traversible = BattleGrid.main.Reachable(partyMember.Pos, partyMember.move, FieldObject.ObjType.Obstacle, FieldObject.ObjType.Party, FieldObject.ObjType.Enemy);
            traversible.RemoveWhere((pos) => (!BattleGrid.main.IsEmpty(pos) && !BattleGrid.main.GetObject(pos).CanShareSquare));
            traversible.Add(Pos);
        }
        DisplayTraversible(value);
    }

    public void DisplayTraversible(bool value)
    {
        if(value)
        {
            foreach (var spot in traversible)
                squares.Add(Instantiate(squarePrefab, BattleGrid.main.GetSpace(spot), Quaternion.identity));
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
        if (!traversible.Contains(newPos))
        {
            Pos difference = newPos - Pos;
            if (difference.SquareMagnitude > partyMember.move * partyMember.move)
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

    public override void Select()
    {
        lastPosition = partyMember.Pos;
        BattleGrid.main.Move(partyMember.Pos, Pos);
        SetActive(false);
        partyMember.OpenActionMenu();
    }

    public void ResetToLastPosition()
    {
        Highlight(lastPosition);
        BattleGrid.main.Move(partyMember.Pos, Pos);
    }

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
