using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GraphBuilder
{
    private uint idCounter = 1;

    public Graph Graph
    {
        get;
        private set;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="coulombRepulsion"></param>
    /// <param name="hookAttraction"></param>
    public GraphBuilder(Graph g)
    {
        this.Graph = g;
    }


    /// <summary>
    /// Assign a new unique id to the movie
    /// </summary>
    /// <param name="entry"></param>
    public void AddEntryToGraph(Entry entry)
    {
        entry.GraphId = idCounter++;
        this.Graph.Vertices.Add(entry);
    }


    /// <summary>
    /// Update the matrix of the graph
    /// </summary>
    /// <param name="c"></param>
    public void AddConnectionToGraph(Connection c)
    {
        if (this.Graph.Edges.Contains(c))
        {            
            Logger.Trace("A duplicate is being destroyed: " + c.gameObject.name, LogLevel.Warning);
            UnityEngine.GameObject.Destroy(c.gameObject);
            return;
        }
        
        this.Graph.Edges.Add(c);       

        // update matrix
        // ...
    }

    


}
