using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2i {

    public int X;

    public int Z;

    public Vector2i()
    {
        X = 0;
        Z = 0;
    }

    public Vector2i(int x, int z)
    {
        X = x;
        Z = z;
    }

    private Vector2 ToVector2 ()
    {
        return new Vector2(X, Z);
    }

    public override bool Equals(object obj)
    {
        var other = obj as Vector2i;
        if (other == null)
            return false;

        return this.X == other.X && this.Z == other.Z;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode()*10000 + Z.GetHashCode();
    }
}
