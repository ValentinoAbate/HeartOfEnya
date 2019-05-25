using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public int Row { get; set; }
    public int Col { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        transform.position = BattleGrid.main.GetSpace(Row, Col);
    }

    public void Select(FieldObject f)
    {
        Row = f.Row;
        Col = f.Col;
        transform.position = BattleGrid.main.GetSpace(Row, Col);
    }

    // Update is called once per frame
    public void OnPhaseUpdate()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            if (Row - 1 >= 0)
                transform.position = BattleGrid.main.GetSpace(--Row, Col);
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            if (Row + 1 < BattleGrid.main.Rows)
                transform.position = BattleGrid.main.GetSpace(++Row, Col);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Col - 1 >= 0)
                transform.position = BattleGrid.main.GetSpace(Row, --Col);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (Col + 1 < BattleGrid.main.Cols)
                transform.position = BattleGrid.main.GetSpace(Row, ++Col);
        }
    }
}
