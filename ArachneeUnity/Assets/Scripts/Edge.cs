using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour 
{
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
        lineRend.SetWidth(0.5F, 0.5F);
        this.line = lineRend;
        this.gameObject.SetActive(false);
    }
}
