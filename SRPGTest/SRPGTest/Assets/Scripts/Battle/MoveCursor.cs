using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PartyMember))]
public class MoveCursor : Cursor
{
    public GameObject squarePrefab;
    private BattleGrid.Pos lastPosition;
    private PartyMember partyMember;
    private HashSet<BattleGrid.Pos> traversible;
    private List<GameObject> squares = new List<GameObject>();
    private void Start()
    {
        partyMember = GetComponent<PartyMember>();
        Row = partyMember.Row;
        Col = partyMember.Col;
        lastPosition = new BattleGrid.Pos(Row, Col);
        enabled = false;
    }

    public override void SetActive(bool value)
    {
        enabled = value;
        if(value)
        {
            traversible = BattleGrid.main.Reachable(partyMember.Pos, partyMember.move, FieldObject.ObjType.Obstacle, FieldObject.ObjType.Enemy);
            foreach(var spot in traversible)
                squares.Add(Instantiate(squarePrefab, BattleGrid.main.GetSpace(spot.row, spot.col), Quaternion.identity));
        }           
    }

    public override void Highlight(int row, int col)
    {
        if (row == Row && col == Col)
            return;
        if (!traversible.Contains(new BattleGrid.Pos(row, col)))
            return;
        Row = row;
        Col = col;
        transform.position = BattleGrid.main.GetSpace(Row, Col);
    }

    public override void Select()
    {
        lastPosition = new BattleGrid.Pos(partyMember.Row, partyMember.Col);
        BattleGrid.main.Move(partyMember.Row, partyMember.Col, Row, Col);
        foreach (var obj in squares)
            Destroy(obj);
        squares.Clear();
        partyMember.OpenActionMenu();
        enabled = false;
    }

    public void ResetToLastPosition()
    {
        Highlight(lastPosition.row, lastPosition.col);
        BattleGrid.main.Move(partyMember.Row, partyMember.Col, Row, Col);
    }

    // Update is called once per frame
    private void Update()
    {
        ProecessInput();
    }
}
