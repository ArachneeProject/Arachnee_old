using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Movie : Entry
{

    public string Title
    {
        get;
        set;
    }

    public int Year
    {
        get;
        set;
    }

    public int Runtime
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
        }
    }

    public int Plot
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
        }
    }

    public int Poster
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
        }
    }

    public int Rating
    {
        get
        {
            throw new System.NotImplementedException();
        }
        set
        {
        }
    }

    public override string ToString()
    {
        return this.Title;
    }
}
