using System.Collections.Generic;
using UnityEngine;

/**
 * positive id for artist; negative id for film
 * */
public static class Graph
{
    // vertices
    public static Dictionary<int, Transform> vertices = new Dictionary<int, Transform>();

    // edges
    public static Dictionary<int,List<Edge>> edges = new Dictionary<int,List<Edge>>();

    // highlighted edges
    private static List<Edge> highlighted = new List<Edge>();



    /// <summary>
    /// highlight all edges connected to this entry
    /// </summary>
    /// <param name="entryId"></param>
    internal static void highlightEdges(int entryId)
    {
        List<Edge> edgesToHighlight = Graph.edges[entryId];
        if (edgesToHighlight.Count == 0)
        {
            Debug.Log("ah bah y a pas de connexion hein");
        }
        foreach(Edge edge in edgesToHighlight)
        {
            edge.gameObject.SetActive(true);
            Graph.highlighted.Add(edge);
        }
    }

    /// <summary>
    /// mask all edges
    /// </summary>
    internal static void maskAll()
    {
        foreach (Edge edge in Graph.highlighted)
        {
            edge.gameObject.SetActive(false);
        }
        Graph.highlighted.Clear();
    }

    /// <summary>
    /// clear the graph
    /// </summary>
    internal static void clearGraph()
    {
        Graph.highlighted.Clear();
        Graph.edges.Clear();
        Graph.vertices.Clear();
    }


    internal static void addEdge(Edge edge)
    {
        Graph.edges[edge.left].Add(edge);
        Graph.edges[edge.right].Add(edge);
    }
}