using UnityEngine;
using System.Collections.Generic;

public class ClusterAlgo : MonoBehaviour 
{
    public int verticesNumber = 10;
    
    private List<int> vertices = new List<int>();
    private List<Pair> edges = new List<Pair>();

    private List<Pair> mincut = new List<Pair>();
    private float mincutValue = Mathf.Infinity;


    void Start()
    {
        initialize();
    }

    // for tests
    private void initialize()
    {
        vertices.Clear();
        edges.Clear();
        for (int i = 1; i <= verticesNumber; i++)
        {
            vertices.Add(i);
        }

        for (int i = 1; i <= verticesNumber / 2; i++)
        {
            for (int j = 1; j <= verticesNumber / 2; j++)
            {
                if (i < j)
                {
                    edges.Add(new Pair(i, j));
                }
            }
        }

        for (int i = (verticesNumber / 2) + 1; i <= verticesNumber; i++)
        {
            for (int j = (verticesNumber / 2) + 1; j <= verticesNumber; j++)
            {
                if (i < j)
                {
                    edges.Add(new Pair(i, j));
                }
            }
        }

        edges.Add(new Pair(1, (verticesNumber / 2) + 1));
    }

    private List<Pair> kargerAlgo()
    {
        List<Pair> contractedEdges = new List<Pair>();

        //     if(this.edges.Count < 1 || this.vertices.Count < 2)
        

        while (this.vertices.Count > 2)
        {
            int randomEdge = Random.Range(0, this.edges.Count);
            contractedEdges.Add(this.edges[randomEdge]);

            int verticeFusion = this.edges[randomEdge].a;
            int verticeDelete = this.edges[randomEdge].b;
            
            List<Pair> edgeToRemove = new List<Pair>();
            List<Pair> edgeToAdd = new List<Pair>();
            foreach (Pair edge in this.edges)
            {         
                if (edge.a == verticeDelete)
                {
                    edgeToRemove.Add(edge);
                    if (verticeFusion != edge.b)
                    {
                        edgeToAdd.Add(new Pair(verticeFusion, edge.b));
                    }
                }
                else if (edge.b == verticeDelete)
                {
                    edgeToRemove.Add(edge);
                    if (verticeFusion != edge.a)
                    {
                        edgeToAdd.Add(new Pair(verticeFusion, edge.a));
                    }
                }
            }

            foreach (Pair edge in edgeToAdd)
            {
                this.edges.Add(edge);
            }
            edgeToAdd.Clear();

            foreach (Pair edge in edgeToRemove)
            {
                this.edges.Remove(edge);
            }
            edgeToRemove.Clear();

            this.vertices.Remove(verticeDelete);
        }
        Debug.Log(this.vertices[0] + " and " + this.vertices[1] + " are still here. Cut :" + this.edges.Count);

        return contractedEdges;
    }


    public void findMinCut(int kargerLoops)
    {
        Debug.Log("=== START ===");
        while (kargerLoops-- > 0 && mincutValue>1)
        {
            List<Pair> contracetdEdges = this.kargerAlgo();
            if (this.edges.Count < this.mincutValue)
            {
                this.mincutValue = edges.Count;
                this.mincut = contracetdEdges;
            }
            initialize();
        }
        Debug.Log("=== END ===");
        foreach (Pair edge in this.mincut)
        {
            Debug.Log(edge);
        }
        Debug.Log("Mincut value :" + this.mincutValue);
    }


    private void print()
    {
        Debug.Log("[state of the tree]");
        Debug.Log("Vertices:");
        foreach (int v in this.vertices)
        {
            Debug.Log(v);
        }
        Debug.Log("Edges:");
        foreach (Pair p in this.edges)
        {
            Debug.Log(p);
        }
    }

    

	
}
