using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorAbs0 : TargetPatternGenerator
{
    public TargetPattern pattern1;
    public TargetPattern pattern2;
    public TargetPattern pattern3;
    public float[] weights = new float[3];
    public int numChoices;
    public bool noDuplicateTargets = true;
    public override TargetPattern Generate()
    {
        var choices = new TargetPattern[] { pattern1, pattern2, pattern3 };
        var targets = new List<PartyMember>(PhaseManager.main.PartyPhase.Party);
        TargetPattern output = new TargetPattern() { type = TargetPattern.Type.Absolute };
        for(int i = 0; i < numChoices; ++i)
        {
            var pattern = RandomU.instance.Choice(choices, weights);
            var target = RandomU.instance.Choice(targets);
            if (noDuplicateTargets && targets.Count > 1)
                targets.Remove(target);
            if(pattern == pattern3)
            {
                pattern.Target(Pos.Zero, target.Pos);
            }
            else
            {
                var posChoices = new List<Pos>
                {
                    target.Pos,
                    target.Pos,
                    target.Pos + Pos.Up,
                    target.Pos + Pos.Down,
                    target.Pos + Pos.Left,
                    target.Pos + Pos.Right,
                };
                posChoices.RemoveAll((p) => !BattleGrid.main.IsLegal(p));
                pattern.Target(Pos.Zero, RandomU.instance.Choice(posChoices));
            }
            output.Add(pattern);
        }
        return output;
    }
}
