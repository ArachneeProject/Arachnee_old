using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Graph
{
    private float coulombRepulsion = 1;
    private float hookeAttraction = 1;
    
    
    public List<Entry> Vertices
    {
        get;
        set;
    }

    public List<Connection> Edges
    {
        get;
        set;
    }

    private List<Entry> activeVertices = new List<Entry>(); // entries affected by physics
    private List<Connection> activeEdges = new List<Connection>(); // connections affecting physics


    public int ActiveVerticesCount
    {
        get { return this.activeVertices.Count; }
        private set {  }
    }
    

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="coulombRepulsion"></param>
    /// <param name="hookeAttraction"></param>
    public Graph(float coulombRepulsion, float hookeAttraction)
    {
        this.Vertices = new List<Entry>();
        this.Edges = new List<Connection>();

        this.coulombRepulsion = Math.Max(coulombRepulsion, 0);
        this.hookeAttraction = Math.Max(hookeAttraction, 0);
    }


    /// <summary>
    /// Update the force between the entries
    /// </summary>
    public void UpdateForces()
    {        
        foreach (Entry entry in this.activeVertices)
        {
            // repulsion
            foreach (Entry otherEntry in this.activeVertices)
            {
                if (entry.GraphId != otherEntry.GraphId)
                {
                    float squaredDistance = MiniMath.getSquaredDistance(entry.transform.position, otherEntry.transform.position);
                    if (squaredDistance < 10000)
                    {
                        Vector3 repulsion = this.coulombRepulsion * (entry.transform.position - otherEntry.transform.position) * (1F / squaredDistance);
                        if (entry.RigidBody != null)
                        {
                            entry.RigidBody.AddForce(repulsion);
                            
                        }
                    }                                        
                }
            }

            // attraction to center
            if (entry.RigidBody != null)
            {
                entry.RigidBody.AddForce(-entry.transform.position);
            }  
        }

        // attraction
        foreach (Connection connect in this.activeEdges)
        {
            Vector3 attraction = this.hookeAttraction * (connect.Left.transform.position - connect.Right.transform.position);
            if (connect.Left.RigidBody != null && connect.Right.RigidBody != null)
            {
                connect.Left.RigidBody.AddForce(-attraction);
                connect.Right.RigidBody.AddForce(attraction);
            }
        }

        
    }


    public void ActiveConnectedVertices(int minimumConnectivity)
    {
        this.activeVertices.Clear();
        this.activeEdges.Clear();
        foreach(Connection c in this.Edges)
        {
            if (c.Left.ConnectedEdges.Count >= minimumConnectivity && c.Right.ConnectedEdges.Count >= minimumConnectivity)
            {
                c.Left.gameObject.SetActive(true);
                c.Right.gameObject.SetActive(true);
                c.gameObject.SetActive(true);
                this.activeVertices.Add(c.Left);
                this.activeVertices.Add(c.Right);
                this.activeEdges.Add(c);
            }
            else
            {
                c.gameObject.SetActive(false);
                if (c.Left.ConnectedEdges.Count < minimumConnectivity)
                {
                    c.Left.gameObject.SetActive(false);
                }
                if (c.Right.ConnectedEdges.Count < minimumConnectivity)
                {
                    c.Right.gameObject.SetActive(false);
                }
            }
        }
    }

}
