using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GraphBuilder
{
    private int idCounter = 1;

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
        entry.gameObject.SetActive(false);
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
        c.gameObject.SetActive(false);

        // update matrix
        // ...
    }

    
    public void UnfoldStart()
    {        
        // init the connections
        foreach (Connection c in this.Graph.Edges)
        {
            c.InitEntries(this.Graph.Vertices);
            c.gameObject.SetActive(false);
        }

        // unfold initialization 
        Int64 seed = UnityEngine.PlayerPrefs.GetInt(Constants.PP_MOVIE_ID);
        Entry seedEntry = null;
        foreach (Entry e in this.Graph.Vertices)
        {
            if (e.DatabaseId == seed)
            {
                seedEntry = e;
                break;
            }
        }
        if (seedEntry == null)
        {
            Logger.Trace("The seed movie with id " + seed + " was not found in the graph!",LogLevel.Error);
            return;
        }
        
        // unfold loop
        List<Entry> toUnfold = new List<Entry>();
        List<Entry> nextStep = new List<Entry>();
        toUnfold.Add(seedEntry);

        int max = this.Graph.maxActiveVertices;
        while (max > 0 && toUnfold.Count>0)
        {            
            foreach (Entry e in toUnfold)
            {                
                if (max-- > 0)
                {
                    this.Graph.Activate(e);
                    foreach(Connection c in e.ConnectedEdges)
                    {
                        nextStep.Add(c.OppositeEntry(e));
                    }
                }
                else
                {
                    break;
                }
            }
            toUnfold.Clear();
            foreach(Entry e in nextStep)
            {
                toUnfold.Add(e);
            }
            nextStep.Clear();
        }
    }


}
