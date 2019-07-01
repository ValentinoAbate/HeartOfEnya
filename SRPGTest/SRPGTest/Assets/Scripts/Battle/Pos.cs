using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Pos : IEquatable<Pos>
{
    public static Pos Origin { get; } = new Pos(0, 0);
    public Vector2 AsVector2 { get => new Vector2(col, row); }
    public int row;
    public int col;
    /// <summary>
    /// Returns the magnitude of this position considered as a Vector2Int squared
    /// </summary>
    public int SquareMagnitude { get => row * row + col * col; }

    public static int Distance(Pos p, Pos p2)
    {
        return Math.Abs(p2.row - p.row) + Math.Abs(p2.col - p.col);
    }

    public Pos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
    public Pos Offset(int rowOff, int colOff)
    {
        return new Pos(row + rowOff, col + colOff);
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
}
