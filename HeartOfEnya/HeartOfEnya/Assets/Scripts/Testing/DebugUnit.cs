using UnityEngine;

public class DebugUnit : MonoBehaviour
{
    public int killDamage = 10;
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
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
            {
                var targets = BattleGrid.main.FindAll<Enemy>();
                foreach (var t in targets)
                    t.Damage(killDamage);
                return;
            }
            var targetPos = BattleGrid.main.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            var target = BattleGrid.main.Get<Combatant>(targetPos);
            if(target != null)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    target.Damage(killDamage);
                else if (Input.GetKey(KeyCode.RightShift))
                    target.Stunned = true;
                else
                    target.Damage(damage);
            }
        }
    }
}
