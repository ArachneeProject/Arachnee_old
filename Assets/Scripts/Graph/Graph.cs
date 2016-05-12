using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public float coulombRepulsion = 150;
    public float hookeAttraction = 1;
    public int maxActiveVertices = 300;

    private HashSet<Entry> activeVertices = new HashSet<Entry>(); // entries affected by physics
    private HashSet<Connection> activeEdges = new HashSet<Connection>(); // connections affecting physics


    public Text selectedEntryName;
    public Button foldupButton;
    public Button maskButton;
    public Button unfoldButton;

    private Entry selectedEntry;

    
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

        // init gui
        this.selectedEntryName.gameObject.SetActive(false);
        this.foldupButton.gameObject.SetActive(false);
        this.unfoldButton.gameObject.SetActive(false);
        this.maskButton.gameObject.SetActive(false);
        
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
    private void foldSingleConnected(Entry e)
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



    #region OnGUI

    void SelectEntry(Entry e)
    {
        this.selectedEntry = e;
        selectedEntryName.text = e.GUI.text;

        // active gui
        this.selectedEntryName.gameObject.SetActive(true);
        this.foldupButton.gameObject.SetActive(true);
        this.unfoldButton.gameObject.SetActive(true);
        this.maskButton.gameObject.SetActive(true);
    }

    public void MaskSelectedEntry()
    {
        if (selectedEntry == null)
        {
            return;
        }

        this.foldSingleConnected(selectedEntry);
        foreach (Connection c in selectedEntry.ConnectedEdges)
        {
            this.activeEdges.Remove(c);
            c.gameObject.SetActive(false);
        }
        this.activeVertices.Remove(selectedEntry);
        selectedEntry.gameObject.SetActive(false);

        selectedEntry = null;

        // mask gui
        this.selectedEntryName.gameObject.SetActive(false);
        this.foldupButton.gameObject.SetActive(false);
        this.unfoldButton.gameObject.SetActive(false);
        this.maskButton.gameObject.SetActive(false);
    }

    public void UnfoldSelectedEntry()
    {
        if (selectedEntry == null)
        {
            return;
        }

        this.selectedEntry.gameObject.SetActive(true);
        this.activeVertices.Add(selectedEntry);
        foreach (Connection c in selectedEntry.ConnectedEdges)
        {
            Entry opposite = c.OppositeEntry(selectedEntry);
            opposite.gameObject.SetActive(true);
            this.activeVertices.Add(opposite);

            this.activeEdges.Add(c);
            c.gameObject.SetActive(true);
        }
    }

    public void FoldUpSelectedEntry()
    {
        if (selectedEntry == null)
        {
            return;
        }

        this.foldSingleConnected(this.selectedEntry);
    }


    /*
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
    */
    #endregion OnGUI

}
