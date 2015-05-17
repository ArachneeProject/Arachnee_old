using System.Collections.Generic;
using UnityEngine;

/**
 * An artist has his/her own id, but the id of a movie is defined as "number of artist + real id of the film", in order to have unique id for each entry.
 * */
public static class Graph
{
    // vertices
    public static Dictionary<int, Transform> vertices = new Dictionary<int, Transform>();

    // edges
    public static List<Edge> edges = new List<Edge>();

    // highlighted edges
    private static List<Edge> highlighted = new List<Edge>();

    public static GameObject yolo;


    /// <summary>
    /// highlight all edges connected to this entry
    /// </summary>
    /// <param name="entryId"></param>
    internal static void highlightEdges(int entryId)
    {
        foreach (Edge edge in Graph.edges)
        {
            if (edge.right == entryId || edge.left == entryId)
            {
                edge.gameObject.SetActive(true);
                Graph.highlighted.Add(edge);
            }
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
    
}