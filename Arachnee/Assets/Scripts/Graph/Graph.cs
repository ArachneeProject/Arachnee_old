using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public float coulombRepulsion = 150;
    public float hookeAttraction = 1;
    public int maxActiveVertices = 300;

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
        // init event handling
        foreach (Entry e in this.Vertices)
        {
            e.EntryClickedEvent += new Entry.EntryClickHandler(SelectEntry);
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



    /// <summary>
    /// Fold the entries that have only one active connection to the provided entry
    /// </summary>
    /// <param name="e"></param>
    public void FoldSingleConnected(Entry e)
    {
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


    /// <summary>
    /// Return the number of edges that are active for the provided entry
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
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

    public void Activate(Entry e)
    {
        e.gameObject.SetActive(true);
        this.activeVertices.Add(e);
        foreach (Connection c in e.ConnectedEdges)
        {
            Entry opposite = c.OppositeEntry(e);
            if(this.activeVertices.Contains(opposite))
            {
                this.activeEdges.Add(c);
                c.gameObject.SetActive(true);
            }            
        }
    }

    public void FoldEntry(Entry e)
    {
        this.FoldSingleConnected(e);
        foreach (Connection c in e.ConnectedEdges)
        {
            this.activeEdges.Remove(c);
            c.gameObject.SetActive(false);
        }
        this.activeVertices.Remove(e);
        e.gameObject.SetActive(false);
    }

    public void UnfoldEntry(Entry e)
    {
        e.gameObject.SetActive(true);
        this.activeVertices.Add(e);
        foreach (Connection c in e.ConnectedEdges)
        {
            Entry opposite = c.OppositeEntry(e);
            opposite.gameObject.SetActive(true);
            this.activeVertices.Add(opposite);

            this.activeEdges.Add(c);
            c.gameObject.SetActive(true);
        }
    }


    void OnGUI()
    {
        if (this.selectedEntry == null)
        {
            return;
        }

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 70, 400, 40), "Unfold all connected to " + selectedEntry.GUI.text))
        {
            this.UnfoldEntry(this.selectedEntry);
        }
        if (GUI.Button(new Rect(10, Screen.height - 70, 400, 40), "Fold single connected to " + selectedEntry.GUI.text))
        {
            this.FoldSingleConnected(this.selectedEntry);
        }
        if (GUI.Button(new Rect(Screen.width - 410, Screen.height - 70, 400, 40), "Fold " + selectedEntry.GUI.text))
        {
            this.FoldEntry(this.selectedEntry);
            this.selectedEntry = null;
        }
    }


    private Entry selectedEntry;

    void SelectEntry(Entry e)
    {
        Debug.Log("You clicked on " + e.GUI.text);
        this.selectedEntry = e;
    }
}
