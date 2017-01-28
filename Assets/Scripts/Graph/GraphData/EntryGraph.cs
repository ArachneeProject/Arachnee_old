using System.Collections.Generic;
using System.Linq;

public class EntryGraph
{
    // vertices
    private readonly HashSet<Entry> _vertices = new HashSet<Entry>();

    public IEnumerable<Entry> Vertices
    {
        get { return this._vertices; }
    }

    // edges 
    /* Example:
     * 
     * My Lucky Stars
     *      Sammo Hung
     *          Actor,
     *          Director
     *      Jackie Chan
     *          Actor
     * */
    private readonly Dictionary<Entry, Dictionary<Entry, HashSet<ConnectionType>>> _edges =
        new Dictionary<Entry, Dictionary<Entry, HashSet<ConnectionType>>>();

    public Dictionary<Entry, Dictionary<Entry, HashSet<ConnectionType>>> Edges
    {
        get { return this._edges; }
    }

    /// <summary>
    /// Return wether or not the entry was added to the vertices of the graph. 
    /// If already present, the entry is ignored (there is no overwrite).
    /// </summary>
    public bool AddVertex(Entry entry)
    {
        if (Entry.IsNullOrDefault(entry))
        {
            return false;
        }
        if (!_vertices.Add(entry))
        {
            return false;
        }
        this._edges.Add(entry, new Dictionary<Entry, HashSet<ConnectionType>>());
        return true;
    }

    /// <summary>
    /// Return wether or not the connection was added to the vertices of the graph. 
    /// If one of the entry is not present in the grapg vertices collection, the connection is ignored.
    /// </summary>
    public bool AddConnection(Entry entryA, Entry entryB, ConnectionType connectionType, bool isDirectedConnection = false)
    {
        if (Entry.IsNullOrDefault(entryA) || Entry.IsNullOrDefault(entryB)
        || !_vertices.Contains(entryA) || !_vertices.Contains(entryB))
        {
            return false;
        }
        
        // add A -> B connection
        var connectedToA = _edges[entryA];
        if (!connectedToA.ContainsKey(entryB))
        {
            connectedToA[entryB] = new HashSet<ConnectionType>();
        }
        bool correctlyAddedA = connectedToA[entryB].Add(connectionType);

        if (isDirectedConnection)
        {
            return correctlyAddedA;
        }

        // add B -> A connection
        var connectedToB = _edges[entryB];
        if (!connectedToB.ContainsKey(entryA))
        {
            connectedToB[entryA] = new HashSet<ConnectionType>();
        }

        return correctlyAddedA && connectedToB[entryA].Add(connectionType);
    }

    /// <summary>
    /// Add to this graph all vertices and edges from the given graph
    /// </summary>
    public void MergeWith(EntryGraph graph)
    {
        foreach (var vertex in graph.Vertices)
        {
            this.AddVertex(vertex);
        }
        foreach (var fromEntry in graph.Edges.Keys)
        {
            foreach (var toEntry in graph.Edges[fromEntry].Keys)
            {
                foreach (var connectionType in graph.Edges[fromEntry][toEntry])
                {
                    this.AddConnection(fromEntry, toEntry, connectionType);
                }
            }
        }
    }

    /// <summary>
    /// Get number of edges in this graph
    /// </summary>
    public int EdgesCount()
    {
        return _edges.Values.Sum(key => key.Values.Count);
    }
}

