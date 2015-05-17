using UnityEngine;
using System.Collections;

public class Pair
{
    public int a
    {
        get;
        set;
    }

    public int b
    {
        get;
        set;
    }

    public Pair(int x, int y)
    {
        this.a = x;
        this.b = y;
    }

    public override string ToString()
    {
        return "(" + a + "," + b + ")";
    }
}
