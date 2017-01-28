using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalUndirectedGraph
{
    private readonly Dictionary<string, PhysicalVertex> _vertices = new Dictionary<string, PhysicalVertex>();
    public IEnumerable<PhysicalVertex> PhysicalVertices
    {
        get { return _vertices.Values; }
    }
    private readonly Dictionary<string, PhysicalEdge> _edges = new Dictionary<string, PhysicalEdge>();
    public IEnumerable<PhysicalEdge> PhysicalEdges
    {
        get { return _edges.Values; }
    }
    
    /// <summary>
    /// Add a physical vertex to this physical graph
    /// </summary>
    public bool AddPhysicalVertex(PhysicalVertex physicalVertex)
    {
        var identifier = physicalVertex.Entry.GetIdentifier();
        if (this._vertices.ContainsKey(identifier))
        {
            Debug.LogError("Game object for " + identifier + " was already present in the physical graph");
            return false;
        }
        this._vertices[identifier] = physicalVertex;
        return true;
    }

    /// <summary>
    /// Get the physical vertex corresponding to the given entry
    /// </summary>
    public bool TryGetPhysicalVertex(Entry entry, out PhysicalVertex physicalVertex)
    {
        return this._vertices.TryGetValue(entry.GetIdentifier(), out physicalVertex);
    }

    /// <summary>
    /// Add a physical edge to this physical graph
    /// </summary>
    public bool AddPhysicalEdge(PhysicalEdge physicalEdge)
    {
        var identifier = getEdgeIdentifier(physicalEdge.Left.Entry, physicalEdge.Right.Entry, physicalEdge.ConnectionType);
        
        if (_edges.ContainsKey(identifier))
        {
            return false;
        }
        _vertices[physicalEdge.Left.Entry.GetIdentifier()] = physicalEdge.Left;
        _vertices[physicalEdge.Right.Entry.GetIdentifier()] = physicalEdge.Right;

        this._edges[identifier] = physicalEdge;
        
        return true;
    }

    /// <summary>
    /// Return wether or not the given connection has a corresponding physical edge
    /// </summary>
    public bool ContainsConnection(Entry fromEntry, Entry toEntry, ConnectionType connectionType)
    {
        return this._edges.ContainsKey(getEdgeIdentifier(fromEntry, toEntry, connectionType));
    }

    /// <summary>
    /// Get the physical edge corresponding to the given connection
    /// </summary>
    public bool TryGetPhysicalEdge(Entry fromEntry, Entry toEntry, ConnectionType connectionType, out PhysicalEdge physicalEdge)
    {
        return this._edges.TryGetValue(getEdgeIdentifier(fromEntry, toEntry, connectionType), out physicalEdge);
    }
        
    private string getEdgeIdentifier(Entry fromEntry, Entry toEntry, ConnectionType connectionType)
    {
        string connectionId;
        var fromId = fromEntry.GetIdentifier();
        var toId = toEntry.GetIdentifier();
        int comparator = string.Compare(fromId, toId, StringComparison.Ordinal);
        if (comparator > 0)
        {
            connectionId = toId + "-" + connectionType + "-" + fromId;
        }
        else
        {
            connectionId = fromId + "-" + connectionType + "-" + toId;
        }

        return connectionId;
    }
}