using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Cursor : MonoBehaviour
{
    public BattleGrid.Pos Pos { get; set; }

    public virtual void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public abstract void Highlight(BattleGrid.Pos pos);

    public virtual void Select()
    {
        var highlighted = BattleGrid.main.GetObject(Pos);
        if (highlighted != null)
        {
            if (highlighted.Select())
                gameObject.SetActive(false);
        }
    }

    public abstract void ProcessInput();
    // Update is called once per frame
    private void Update()
    {
        ProcessInput();
    }
}
