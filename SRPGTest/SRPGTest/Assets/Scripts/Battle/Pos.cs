using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Pos : IEquatable<Pos>
{
    public int row;
    public int col;
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

    public static bool operator ==(Pos pos1, Pos pos2)
    {
        return pos1.Equals(pos2);
    }

    public static bool operator !=(Pos pos1, Pos pos2)
    {
        return !(pos1 == pos2);
    }
}
