using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A struct that signifies a position value in a BattleGrid
/// </summary>
[Serializable]
public struct Pos : IEquatable<Pos>
{
    #region Static Symbolic Constants

    public static Pos Zero { get; } = new Pos(0, 0);
    public static Pos One { get; } = new Pos(1, 1);
    public static Pos Right { get; } = new Pos(0, 1);
    public static Pos Left { get; } = new Pos(0, -1);
    public static Pos Up { get; } = new Pos(-1, 0);
    public static Pos Down { get; } = new Pos(1, 0);
    public static Pos OutOfBounds { get; } = new Pos(-100, -100);

    #endregion

    public Vector2 AsVector2 { get => new Vector2(col, row); }
    public int row;
    public int col;
    /// <summary>
    /// Returns the magnitude of this position considered as a Vector2Int squared
    /// </summary>
    public int SquareMagnitude { get => row * row + col * col; }

    #region Static Utility Methods

    public static int Distance(Pos p1, Pos p2)
    {
        return Math.Abs(p2.row - p1.row) + Math.Abs(p2.col - p1.col);
    }
    /// <summary>
    /// If the two points are on the same column, returns the vertical direction from "from" to "to",
    /// else returns the horizontal direction between the two points.
    /// </summary>
    /// <returns> Pos.Up, Pos.Down, Pos.Left, Pos.Right, or Pos.Zero (if points are the same) </returns>
    public static Pos DirectionBasic(Pos from, Pos to)
    {
        if (from.col == to.col)
        {
            if (from.row == to.row)
                return Zero;
            return from.row > to.row ? Up : Down;
        }          
        else
            return from.col > to.col ? Left : Right;
    }
    /// <summary>
    /// Compares two positions by their column and then by their row if their columns are equal
    /// </summary>
    public static int CompareLeftToRightTopToBottom(Pos p1, Pos p2)
    {
        int colComp = p1.col.CompareTo(p2.col);
        if (colComp != 0)
            return colComp;
        return p1.row.CompareTo(p2.row);
    }
    /// <summary>
    /// Compares two positions by their row and then by their column if their rows are equal
    /// </summary>
    public static int CompareTopToBottomLeftToRight(Pos p1, Pos p2)
    {
        int rowComp = p1.row.CompareTo(p2.row);
        if (rowComp != 0)
            return rowComp;
        return p1.col.CompareTo(p2.col);
    }
    /// <summary>
    /// Rotates a point around another point another point.
    /// startDirection and goalDirection should either be Pos.Up, Pos.Down, Pos.Left, or Pos.Right
    /// </summary>
    public static Pos Rotated(Pos center, Pos point, Pos startDirection, Pos goalDirection)
    {
        // Already in the proper direction
        if (startDirection == goalDirection)
            return point;
        Pos difference = point - center;
        Pos sumDirection = startDirection + goalDirection;
        // Directions are colinear
        if (sumDirection == Zero)
            return center - difference;
        // Directions are perpendicular
        return center + PointwiseProduct(difference.AxesSwapped(), sumDirection);
    }
    /// <summary>
    /// Return the Pointwise product of two Pos (considering them as 2D int vectors).
    /// Returns new Pos(p1.row * p2.row, p1.col * p2.col);
    /// </summary>
    public static Pos PointwiseProduct(Pos p1, Pos p2)
    {
        return new Pos(p1.row * p2.row, p1.col * p2.col);
    }

    public static Pos Average(IEnumerable<Pos> positions)
    {
        Pos sum = Pos.Zero;
        int count = 0;
        foreach(var pos in positions)
        {
            sum += pos;
            ++count;
        }
        return new Pos(sum.row / count, sum.col / count);
    }

    #endregion

    public Pos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public Pos Offset(int rowOff, int colOff)
    {
        return new Pos(row + rowOff, col + colOff);
    }
    public Pos AxesSwapped()
    {
        return new Pos(col, row);
    }

    public override bool Equals(object obj)
    {
        return obj is Pos && Equals((Pos)obj);
    }

    public bool Equals(Pos other)
    {
        return row == other.row && col == other.col;
    }

    public override int GetHashCode()
    {
        var hashCode = -1720622044;
        hashCode = hashCode * -1521134295 + row.GetHashCode();
        hashCode = hashCode * -1521134295 + col.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return "row: " + row + " col: " + col;
    }

    #region Operator Overloads

    public static Pos operator *(Pos vector, int scalar)
    {
        return new Pos(vector.row * scalar, vector.col * scalar);
    }

    public static Pos operator -(Pos pos1, Pos pos2)
    {
        return new Pos(pos1.row - pos2.row, pos1.col - pos2.col);
    }

    public static Pos operator +(Pos pos1, Pos pos2)
    {
        return new Pos(pos1.row + pos2.row, pos1.col + pos2.col);
    }

    public static bool operator ==(Pos pos1, Pos pos2)
    {
        return pos1.Equals(pos2);
    }

    public static bool operator !=(Pos pos1, Pos pos2)
    {
        return !(pos1 == pos2);
    }

    #endregion
}
