using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Graph
{
    private float coulombRepulsion = 1;
    private float hookeAttraction = 1;
    
    
    public Dictionary<uint, Entry> Vertices
    {
        get;
        set;
    }

    public List<Connection> Edges
    {
        get;
        set;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="coulombRepulsion"></param>
    /// <param name="hookeAttraction"></param>
    public Graph(float coulombRepulsion, float hookeAttraction)
    {
        this.Vertices = new Dictionary<uint, Entry>();
        this.Edges = new List<Connection>();

        this.coulombRepulsion = Math.Max(coulombRepulsion, 0);
        this.hookeAttraction = Math.Max(hookeAttraction, 0);
    }


    /// <summary>
    /// Update the force between the entries
    /// </summary>
    public void UpdateForces()
    {
        
        foreach (Entry entry in this.Vertices.Values)
        {
            // repulsion
            foreach (Entry otherEntry in this.Vertices.Values)
            {
                if (entry.GraphId != otherEntry.GraphId)
                {
                    float distance = Vector3.Distance(entry.transform.position, otherEntry.transform.position);
                    if (distance < 100)
                    {
                        Vector3 repulsion = this.coulombRepulsion * (entry.transform.position - otherEntry.transform.position) * (float)Math.Pow((1F / distance), 2);
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
        foreach (Connection connect in this.Edges)
        {
            Vector3 attraction = this.hookeAttraction * (connect.Left.transform.position - connect.Right.transform.position);
            if (connect.Left.RigidBody != null && connect.Right.RigidBody != null)
            {
                connect.Left.RigidBody.AddForce(-attraction);
                connect.Right.RigidBody.AddForce(attraction);
            }
        }

        
    }

}
