using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour 
{
    public Edge(int left, int right)
    {
        this.left = left;
        this.right = right;
    }

    public int left
    {
        get;
        private set;
    }

    public int right
    {
        get;
        private set;
    }

    public LineRenderer line
    {
        get;
        private set;
    }

    internal void setUp(int left, int right, LineRenderer lineRend)
    {
        this.left = left;
        this.right = right;
        lineRend.SetPosition(0, Graph.vertices[left].position);
        lineRend.SetPosition(1, Graph.vertices[right].position);
        this.line = lineRend;
        this.gameObject.SetActive(false);
    }
}
