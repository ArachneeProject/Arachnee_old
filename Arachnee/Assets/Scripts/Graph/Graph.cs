using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Graph
{
    
    /// <summary>
    /// Dictionary of Entries
    /// </summary>
    public Dictionary<uint, Entry> Vertices
    {
        get;
        set;
    }

    public Graph()
    {
        this.Vertices = new Dictionary<uint, Entry>();
    }

}
