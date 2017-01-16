using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphBuilder : MonoBehaviour
{
    public int buildingRadius = 50;
    public int maxVertices = 1000;

    public PhysicalGraphEngine graphEngine;
    public GraphUI graphUi;

    public PhysicalVertex moviePrefab;
    public PhysicalVertex artistPrefab;
    public PhysicalEdge ActorConnectionPrefab;
    public PhysicalEdge DirectorConnectionPrefab;

    private Dictionary<Type, PhysicalVertex> _vertexAssociation = new Dictionary<Type, PhysicalVertex>();
    private Dictionary<ConnectionType,PhysicalEdge> _edgeAssociation = new Dictionary<ConnectionType, PhysicalEdge>();

    private DatabaseDialoger _dbDialoger;
    
    void Start()
    {
        this._dbDialoger = new DatabaseDialoger();

        InitPrefabAssociation();

        var graph = BuildGraph(maxVertices);
        
        var physicalVertices = this.BuildVertexGameObjects(graph).ToList();
        var physicalEdges = this.BuildEdgeGameObjects(graph, physicalVertices);

        this.graphEngine.AddPhysicalVertices(physicalVertices);
        this.graphEngine.AddPhysicalEdges(physicalEdges);

        this.graphUi.AddPhysicalVertices(physicalVertices);
        this.graphUi.AddPhysicalEdges(physicalEdges);
    }

    private void InitPrefabAssociation()
    {
        _vertexAssociation.Add(typeof(Movie), moviePrefab);
        _vertexAssociation.Add(typeof(Artist), artistPrefab);

        _edgeAssociation.Add(ConnectionType.Actor, ActorConnectionPrefab);
        _edgeAssociation.Add(ConnectionType.Director, DirectorConnectionPrefab);
    }

    private EntryGraph BuildGraph(int allowedVerticesCounter)
    {
        EntryGraph entryGraph = new EntryGraph();

        // get seed entry identifier
        var seedIdentifier = PlayerPrefs.GetString(Constants.PP_SeedVertexIdentifier);
        if (string.IsNullOrEmpty(seedIdentifier))
        {
            Debug.LogError("PlayerPref \"" + Constants.PP_SeedVertexIdentifier + "\" gave an empty string.");
            return entryGraph;
        }

        // get corresponding entry
        var seedEntry = this._dbDialoger.GetEntry(seedIdentifier);
        if (seedEntry == Entry.DefaultEntry)
        {
            Debug.LogError("The seed vertex " + seedIdentifier + " gave the default entry");
            return entryGraph;
        }

        // build graph
        entryGraph.AddVertex(seedEntry);
        allowedVerticesCounter--;

        HashSet<Entry> entriesToConnect = new HashSet<Entry> { seedEntry };
        
        while (entriesToConnect.Any() && allowedVerticesCounter > 0)
        {
            Debug.Log("Building graph... (" + entryGraph.Vertices.Count() + " vertices done)");
            
            HashSet<Entry> nextStep = new HashSet<Entry>();

            foreach (var entryToConnect in entriesToConnect)
            {
                var connections = this._dbDialoger.GetEntriesConnectedTo(entryToConnect);

                foreach (var connectedEntry in connections.Keys)
                {
                    if (allowedVerticesCounter > 0)
                    {
                        if (entryGraph.AddVertex(connectedEntry))
                        {
                            allowedVerticesCounter--;
                            nextStep.Add(connectedEntry);
                        }
                    }
                    foreach (var connectionType in connections[connectedEntry])
                    {
                        entryGraph.AddConnection(entryToConnect, connectedEntry, connectionType);
                    }
                }
            }

            entriesToConnect.Clear();
            foreach (var entry in nextStep)
            {
                entriesToConnect.Add(entry);
            }
        }

        Debug.Log("Graph built, " +
                  "Vertices: " + entryGraph.Vertices.Count() + ", " +
                  " Edges: " + entryGraph.EdgesCount());
        
        return entryGraph;
    }
    
    private IEnumerable<PhysicalVertex> BuildVertexGameObjects(EntryGraph entryGraph)
    {
        List<PhysicalVertex> physicalVertices = new List<PhysicalVertex>();
        
        foreach (var entry in entryGraph.Vertices)
        {
            PhysicalVertex prefab;
            if (!this._vertexAssociation.TryGetValue(entry.GetType(), out prefab))
            {
                Debug.LogError(entry.GetType() + " doesn't have an associated prefab, cannot build " + entry + " game object");
                continue;
            }

            var physicalVertex = Instantiate(prefab);
            physicalVertex.Entry = entry;
            physicalVertices.Add(physicalVertex);
        }

        if (physicalVertices.Count != entryGraph.Vertices.Count())
        {
            Debug.LogWarning("Graph contains " + entryGraph.Vertices.Count() + " " + typeof(Entry).Name + " elements, " +
                             " but " + physicalVertices.Count + " " + typeof(PhysicalVertex).Name + " elements were built");
        }

        return physicalVertices;
    }

    private IEnumerable<PhysicalEdge> BuildEdgeGameObjects(EntryGraph entryGraph, IEnumerable<PhysicalVertex> physicalVertices)
    {
        Dictionary<Entry, PhysicalVertex> invertedDictionary = new Dictionary<Entry, PhysicalVertex>();
        foreach (var physicalVertex in physicalVertices)
        {
            if (invertedDictionary.ContainsKey(physicalVertex.Entry))
            {
                Debug.LogError(physicalVertex.Entry + " is already associated to the vertex \"" + physicalVertex + "\"");
                continue;
            }
            invertedDictionary[physicalVertex.Entry] = physicalVertex;
        }

        List<PhysicalEdge> physicalEdges = new List<PhysicalEdge>();
        HashSet<string> connectionIds = new HashSet<string>();

        foreach (var fromEntry in entryGraph.Edges.Keys)
        {
            foreach (var toEntry in entryGraph.Edges[fromEntry].Keys)
            {
                foreach (var connectionType in entryGraph.Edges[fromEntry][toEntry])
                {
                    // check if undirected connection gameobject already exist
                    string connectionId;
                    var fromId = fromEntry.GetIdentifier();
                    var toId = toEntry.GetIdentifier();
                    int comparator = string.Compare(fromId, toId, StringComparison.Ordinal);
                    if (comparator > 0)
                    {
                        connectionId = toId + "-" + connectionType + "-" + fromId;
                    }
                    else if (comparator < 0)
                    {
                        connectionId = fromId + "-" + connectionType + "-" + toId;
                    }
                    else
                    {
                        Debug.LogError(fromEntry + " and " + toEntry + " seems to be the same entry");
                        continue;
                    }

                    if (connectionIds.Contains(connectionId))
                    {
                        continue;
                    }

                    // get vertices gameobjects
                    PhysicalVertex left;
                    PhysicalVertex right;
                    if (!invertedDictionary.TryGetValue(fromEntry, out left)
                     || !invertedDictionary.TryGetValue(toEntry, out right))
                    {
                        Debug.LogError("The gameobject of the " + connectionType + " connection " +
                                       "from " + fromEntry + " to " + toEntry + " cannot be created " +
                                       "because at least one of them doesn't have an associated gameobject");
                        continue;
                    }

                    // instantiate physical edge
                    PhysicalEdge prefab;
                    if (!this._edgeAssociation.TryGetValue(connectionType, out prefab))
                    {
                        Debug.LogError(connectionType + " connection type doesn't have an associated prefab, " +
                                       "cannot build the connection between " + fromEntry + " and " + toEntry);
                        continue;
                    }

                    var edge = Instantiate(prefab);
                    edge.Left = left;
                    edge.Right = right;

                    physicalEdges.Add(edge);
                    connectionIds.Add(connectionId);
                }
            }

        }

        return physicalEdges;
    }

    /*
    
    /// <summary>
    /// Retrive poster for each entry
    /// </summary>
    /// <returns></returns>
    private IEnumerator retrievePosters()
    {
        OnlineRetriever onlret = new OnlineRetriever();

        List<Entry> lowPriorityEntries = new List<Entry>();

        
        foreach (Entry e in this.GraphManager.Graph.Vertices)
        {
            if (e.PosterPath == "null")
            {
                e.gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load(Constants.Res_DefaultImage) as Texture;
                continue;
            }

            if (!e.gameObject.activeInHierarchy)
            {
                lowPriorityEntries.Add(e);
                continue;
            }

            yield return StartCoroutine(onlret.RetrievePoster(e.PosterPath));
            if (onlret.Texture != null)
            {
                e.gameObject.GetComponent<Renderer>().material.mainTexture = onlret.Texture;
            }
            else
            {
                e.gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load(Constants.Res_DefaultImage) as Texture;
            }
        }
        

        foreach (Entry e in lowPriorityEntries)
        {
            if (e.PosterPath == "null")
            {
                e.gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load(Constants.Res_DefaultImage) as Texture;
                continue;
            }

            yield return StartCoroutine(onlret.RetrievePoster(e.PosterPath));
            if (onlret.Texture != null)
            {
                e.gameObject.GetComponent<Renderer>().material.mainTexture = onlret.Texture;
            }
            else
            {
                e.gameObject.GetComponent<Renderer>().material.mainTexture = Resources.Load(Constants.Res_DefaultImage) as Texture;
            }
        }
    }
    #endregion Entries
    */
}
