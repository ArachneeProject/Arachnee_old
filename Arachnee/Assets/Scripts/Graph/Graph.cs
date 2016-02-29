using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public float coulombRepulsion = 150;
    public float hookeAttraction = 1;
    public int maxVertices = 200;

    private HashSet<Entry> activeVertices = new HashSet<Entry>(); // entries affected by physics
    private HashSet<Connection> activeEdges = new HashSet<Connection>(); // connections affecting physics


    private List<Entry> _vertices = new List<Entry>();
    public List<Entry> Vertices
    {
        get { return _vertices; }
        set { _vertices = value; }
    }

    private List<Connection> _edges = new List<Connection>();
    public List<Connection> Edges
    {
        get { return _edges; }
        set { _edges = value; }
    }
    
    void Start()
    {
        

        // init the connections
        foreach (Connection c in this.Edges)
        {
            c.InitEntries(this.Vertices);
            c.gameObject.SetActive(false);       
        }

        List<Entry> toUnfold = new List<Entry>();
        uint seedMovieId = (uint)PlayerPrefs.GetInt("MovieID");
        foreach (Entry e in this.Vertices)
        {
            // init the event handling on entries
            e.EntryClickedEvent += new Entry.EntryClickHandler(FoldEntry);
            if (e is Movie && e.DatabaseId == seedMovieId)
            {
                toUnfold.Add(e);
            }
            e.gameObject.SetActive(false);
        }

        int counter = 0;

        while (counter < this.maxVertices)
        {
            List<Entry> nextStep = new List<Entry>();
            foreach (Entry e in toUnfold)
            {                
                if (counter++ < this.maxVertices)
                {
                    e.gameObject.SetActive(true);
                    this.activeVertices.Add(e);
                }
                foreach (Connection c in e.ConnectedEdges)
                {
                    nextStep.Add(c.OppositeEntry(e));
                }
            }
            toUnfold = nextStep;
        }
        
        foreach (Entry e in this.activeVertices)
        {
            foreach (Connection c in e.ConnectedEdges)
            {
                if (this.activeVertices.Contains(c.OppositeEntry(e)))
                {
                    c.gameObject.SetActive(true);
                    this.activeEdges.Add(c);
                }
            }
        }

        
    }

    
    /// <summary>
    /// Update the force between the entries
    /// </summary>
    void FixedUpdate()
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


    private int numberOfActiveEdges(Entry e)
    {
        int counter = 0;
        foreach (Connection c in e.ConnectedEdges)
        {
            if (this.activeEdges.Contains(c))
            {
                counter++;
            }
        }
        return counter;
    }

    public void FoldEntry(Entry e)
    {
        Debug.Log("Entry clicked");
        foreach (Connection c in e.ConnectedEdges)
        {
            if (this.activeEdges.Contains(c))
            {
                Entry oppositeEntry = c.OppositeEntry(e);
                if (numberOfActiveEdges(oppositeEntry) == 1)
                {
                    this.activeVertices.Remove(oppositeEntry);
                    oppositeEntry.gameObject.SetActive(false);
                    this.activeEdges.Remove(c);
                    c.gameObject.SetActive(false);
                }
            }
        }
    }

    public void UnfoldEntry(Entry e)
    {
        
    }

}
