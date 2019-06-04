using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PartyMember))]
public class MoveCursor : GridCursor
{
    public GameObject squarePrefab;
    private BattleGrid.Pos lastPosition;
    private PartyMember partyMember;
    private HashSet<BattleGrid.Pos> traversible;
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
            traversible = BattleGrid.main.Reachable(partyMember.Pos, partyMember.move, FieldObject.ObjType.Obstacle, FieldObject.ObjType.Enemy);
            foreach(var spot in traversible)
                squares.Add(Instantiate(squarePrefab, BattleGrid.main.GetSpace(spot.row, spot.col), Quaternion.identity));
        }           
    }

    public override void Highlight(BattleGrid.Pos newPos)
    {
        if (newPos == Pos)
            return;
        if (!traversible.Contains(newPos))
            return;
        Pos = newPos;
        transform.position = BattleGrid.main.GetSpace(Pos);
    }

    public override void Select()
    {
        lastPosition = new BattleGrid.Pos(partyMember.Row, partyMember.Col);
        BattleGrid.main.Move(partyMember.Pos, Pos);
        foreach (var obj in squares)
            Destroy(obj);
        squares.Clear();
        partyMember.OpenActionMenu();
        enabled = false;
    }

    public void ResetToLastPosition()
    {
        Highlight(lastPosition);
        BattleGrid.main.Move(partyMember.Pos, Pos);
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessInput();
    }
}
