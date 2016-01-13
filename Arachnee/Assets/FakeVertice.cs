using UnityEngine;
using System.Collections;

public class FakeVertice : MonoBehaviour 
{
    public FakeEdge[] edges;
    public int level;
    public Rigidbody RigidBody { get; set; }

    void Start()
    {
        this.RigidBody = this.GetComponent<Rigidbody>();
    }

    internal void SetEdgeActive(bool value)
    {
        foreach (FakeEdge e in this.edges)
        {
            e.gameObject.SetActive(value);
        }
    }
}
