using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalGraphEngine : MonoBehaviour
{
    public float coulombRepulsion = 150;
    public float hookeAttraction = 1;
    public float coulombRepulsionSquaredMaxDistance = 10000;
    public int maxActiveVertices = 300;
    
    private readonly List<PhysicalVertex> _activeVertices = new List<PhysicalVertex>();
    private readonly List<PhysicalEdge> _activeEdges = new List<PhysicalEdge>();

    public void AddPhysicalVertices(IEnumerable<PhysicalVertex> physicalVertices)
    {
        _activeVertices.AddRange(physicalVertices);
    }

    public void AddPhysicalEdges(IEnumerable<PhysicalEdge> physicalEdges)
    {
        _activeEdges.AddRange(physicalEdges);
    }

    /// <summary>
    /// Update the force between the entries
    /// </summary>
    void FixedUpdate()
    {
        foreach (var vertex in this._activeVertices)
        {
            // repulsion
            foreach (var otherVertex in this._activeVertices)
            {
                if (vertex != otherVertex)
                {
                    float squaredDistance = MiniMath.getSquaredDistance(vertex.transform.position, otherVertex.transform.position);
                    if (squaredDistance < coulombRepulsionSquaredMaxDistance)
                    {
                        Vector3 repulsion = this.coulombRepulsion * (vertex.transform.position - otherVertex.transform.position) * (1F / squaredDistance);
                        if (vertex.RigidBody != null)
                        {
                            vertex.RigidBody.AddForce(repulsion);
                        }
                    }
                }
            }

            // attraction to center
            if (vertex.RigidBody != null)
            {
                vertex.RigidBody.AddForce(-vertex.transform.position);
            }
        }

        // attraction
        foreach (var edge in this._activeEdges)
        {
            Vector3 attraction = this.hookeAttraction * (edge.Left.transform.position - edge.Right.transform.position);
            if (edge.Left.RigidBody != null && edge.Right.RigidBody != null)
            {
                edge.Left.RigidBody.AddForce(-attraction);
                edge.Right.RigidBody.AddForce(attraction);
            }
        }
    }

    /*

    /// <summary>
    /// Fold entries having only one active connection to the given entry
    /// </summary>
    private void foldSingleConnected(Entry e)
    {
        foreach (Edge c in e.ConnectedEdges)
        {
            if (this.activeEdges.Contains(c))
            {
                Entry oppositeEntry = c.OppositeEntry(e);
                if (numberOfActiveEdges(oppositeEntry) == 1)
                {
                    this._activeVertices.Remove(oppositeEntry);
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
    private int numberOfActiveEdges(Entry e)
    {
        int counter = 0;
        foreach (Edge c in e.ConnectedEdges)
        {
            if (this.activeEdges.Contains(c))
            {
                counter++;
            }
        }
        return counter;
    }

    public void Activate(Vertex v)
    {
        v.gameObject.SetActive(true);
        this._activeVertices.Add(v);
        foreach (Edge c in v.ConnectedEdges)
        {
            Entry opposite = c.OppositeEntry(v);
            if (this._activeVertices.Contains(opposite))
            {
                this.activeEdges.Add(c);
                c.gameObject.SetActive(true);
            }
        }
    }

    public void MaskSelectedEntry()
    {
        if (selectedEntry == null)
        {
            return;
        }

        this.foldSingleConnected(selectedEntry);
        foreach (Edge c in selectedEntry.ConnectedEdges)
        {
            this.activeEdges.Remove(c);
            c.gameObject.SetActive(false);
        }
        this._activeVertices.Remove(selectedEntry);
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
        this._activeVertices.Add(selectedEntry);
        foreach (Edge c in selectedEntry.ConnectedEdges)
        {
            Entry opposite = c.OppositeEntry(selectedEntry);
            opposite.gameObject.SetActive(true);
            this._activeVertices.Add(opposite);

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

    */
}
