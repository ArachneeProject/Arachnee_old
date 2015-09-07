using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Artist : Entry
{

    public string FirstName
    {
        get;
        set;
    }

    public string LastName
    {
        get;
        set;
    }

    public int BirthYear
    {
        get;
        set;
    }

    public override string ToString()
    {
        return this.FirstName + " " + this.LastName;
    }
}
