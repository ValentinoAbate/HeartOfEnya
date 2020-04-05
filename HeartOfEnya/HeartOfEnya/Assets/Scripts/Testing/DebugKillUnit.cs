using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugKillUnit : MonoBehaviour
{
    public int damage = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {
            var targetPos = BattleGrid.main.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            var target = BattleGrid.main.GetObject(targetPos) as Combatant;
            if(target != null)
            {
                target.Damage(damage);
            }
        }
    }
}
