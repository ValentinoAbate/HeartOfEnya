using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxOrderRowColumn : VfxOrder
{
    public enum Mode
    { 
        Rows,
        Columns,
        Directional,
    }

    public Mode mode;
    public int numPerBatch = 1;

    public override bool UseBatches => true;

    public override List<List<Pos>> GetPositions(List<Pos> targets)
    {
        var output = new List<List<Pos>>();
        if (targets.Count < 1)
            return output;
        var modMode = mode;
        if(mode == Mode.Directional)
        {
            modMode = Mode.Rows;
            //choose columns or rows
        }
        if (modMode == Mode.Columns)
        {
            var targetsSorted = new List<Pos>(targets);
            targetsSorted.Sort(Pos.CompareLeftToRightTopToBottom);
            int currCol = targetsSorted[0].col;
            var currBatch = new List<Pos>();
            foreach(var position in targetsSorted)
            {
                if(position.col >= currCol + numPerBatch)
                {
                    output.Add(currBatch);
                    currBatch = new List<Pos>();
                    currBatch.Add(position);
                    currCol += numPerBatch;
                }
                else
                {
                    currBatch.Add(position);
                }
            }
            output.Add(currBatch);
        }
        else // modMode == Mode.Rows
        {
            var targetsSorted = new List<Pos>(targets);
            targetsSorted.Sort(Pos.CompareTopToBottomLeftToRight);
            int currRow = targetsSorted[0].row;
            var currBatch = new List<Pos>();
            foreach (var position in targetsSorted)
            {
                if (position.row >= currRow + numPerBatch)
                {
                    output.Add(currBatch);
                    currBatch = new List<Pos>();
                    currBatch.Add(position);
                    currRow += numPerBatch;
                }
                else
                {
                    currBatch.Add(position);
                }
            }
            output.Add(currBatch);
        }
        return output;
    }
}
