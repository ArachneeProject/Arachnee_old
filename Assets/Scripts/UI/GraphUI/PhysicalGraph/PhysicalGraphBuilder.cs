using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicalGraphBuilder : MonoBehaviour
{
    public PhysicalEdge ActorConnectionPrefab;
    public PhysicalEdge DirectorConnectionPrefab;
    public PhysicalEntryBuilder VertexBuilder;
    private readonly Dictionary<ConnectionType,PhysicalEdge> _edgeAssociation = new Dictionary<ConnectionType, PhysicalEdge>();
    
    void Start()
    {
        _edgeAssociation.Add(ConnectionType.Actor, ActorConnectionPrefab);
        _edgeAssociation.Add(ConnectionType.Director, DirectorConnectionPrefab);
    }
    
    /// <summary>
    /// Instantiate gameobjects representing the given graph. Returns the list of instantiated gameobjects.
    /// </summary>
    public PhysicalUndirectedGraph BuildPhysicalGraph(EntryGraph entryGraph, PhysicalVertex.EntryClickHandler entryClickHandler)
    {
        var physicalVertices = this.VertexBuilder.BuildEntries(entryGraph.Vertices, entryClickHandler).ToList();

        PhysicalUndirectedGraph physicalUndirectedGraph = new PhysicalUndirectedGraph();
        foreach (var physicalVertex in physicalVertices)
        {
            physicalUndirectedGraph.AddPhysicalVertex(physicalVertex);
        }

        foreach (var fromEntry in entryGraph.Edges.Keys)
        {
            foreach (var toEntry in entryGraph.Edges[fromEntry].Keys)
            {
                foreach (var connectionType in entryGraph.Edges[fromEntry][toEntry])
                {
                    if (physicalUndirectedGraph.ContainsConnection(fromEntry, toEntry, connectionType))
                    {
                        continue;
                    }

                    // get vertices gameobjects
                    PhysicalVertex left;
                    PhysicalVertex right;
                    if (!physicalUndirectedGraph.TryGetPhysicalVertex(fromEntry, out left)
                     || !physicalUndirectedGraph.TryGetPhysicalVertex(toEntry, out right))
                    {
                        Debug.LogError("entry is not linked to any game object");
                        continue;
                    }
                    
                    // instantiate physical edge
                    PhysicalEdge prefab;
                    if (!this._edgeAssociation.TryGetValue(connectionType, out prefab))
                    {
                        Debug.LogError(connectionType + " connection doesn't have an associated prefab, " +
                                       "cannot build the connection between " + fromEntry + " and " + toEntry);
                        continue;
                    }
                    
                    var edge = Instantiate(prefab);
                    edge.Left = left;
                    edge.Right = right;
                    edge.ConnectionType = connectionType;

                    physicalUndirectedGraph.AddPhysicalEdge(edge);
                }
            }
        }
        return physicalUndirectedGraph;
    }
}
